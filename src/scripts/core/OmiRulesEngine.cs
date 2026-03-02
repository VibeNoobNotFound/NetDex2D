using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NetDex.Core.Commands;
using NetDex.Core.Config;
using NetDex.Core.Enums;
using NetDex.Core.Models;
using NetDex.Core.Serialization;

namespace NetDex.Core.Rules;

public sealed class OmiRulesEngine : IGameRulesEngine
{
    public OmiMatchState CreateInitialMatchState(SeatPosition hostSeat, int initialCredits = 10)
    {
        return new OmiMatchState
        {
            GameType = GameType.Omi,
            Phase = OmiPhase.LobbySeating,
            HostSeat = hostSeat,
            ShufflerSeat = hostSeat,
            CurrentStake = 1,
            TeamCredits = new[] { initialCredits, initialCredits }
        };
    }

    public MatchCommandResult ApplyCommand(OmiMatchState state, MatchCommand command)
    {
        return command.Type switch
        {
            MatchCommandType.StartRound => StartRound(state, command.Seed),
            MatchCommandType.StartNextRound => StartRound(state, command.Seed),
            MatchCommandType.ShuffleAgain => ShuffleAgain(state, command),
            MatchCommandType.FinishShuffle => FinishShuffle(state, command),
            MatchCommandType.CutDeck => CutDeck(state, command),
            MatchCommandType.CompleteFirstDeal => CompleteFirstDeal(state),
            MatchCommandType.SelectTrump => SelectTrump(state, command),
            MatchCommandType.CompleteSecondDeal => CompleteSecondDeal(state),
            MatchCommandType.PlayCard => PlayCard(state, command),
            MatchCommandType.ResolveCurrentTrick => ResolveCurrentTrick(state),
            MatchCommandType.KapothiPropose => KapothiPropose(state, command),
            MatchCommandType.KapothiSkip => KapothiSkip(state, command),
            MatchCommandType.KapothiAccept => KapothiAccept(state, command),
            MatchCommandType.KapothiReject => KapothiReject(state, command),
            MatchCommandType.ForfeitTeam => ForfeitTeam(state, command.TeamIndex),
            _ => MatchCommandResult.Fail("Unknown command")
        };
    }

    public VisibleMatchState GetVisibleStateForPeer(OmiMatchState state, SeatPosition? viewerSeat, ParticipantRole role)
    {
        var visible = new VisibleMatchState
        {
            SourceState = state,
            ViewerRole = role,
            ViewerSeat = viewerSeat
        };

        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            if (role == ParticipantRole.Spectator || viewerSeat == seat)
            {
                visible.VisibleHands[seat].AddRange(state.Hands[seat]);
            }
        }

        return visible;
    }

    public Godot.Collections.Dictionary SerializeSnapshot(VisibleMatchState visibleState)
    {
        return MatchSnapshotSerializer.Serialize(visibleState);
    }

    public IReadOnlyList<MatchCommand> EnumerateLegalCommands(OmiMatchState state, SeatPosition actorSeat, int actorPeerId)
    {
        var commands = new List<MatchCommand>();

        switch (state.Phase)
        {
            case OmiPhase.Shuffle:
                if (actorSeat != state.ShufflerSeat)
                {
                    return commands;
                }

                commands.Add(MatchCommand.FinishShuffle(actorSeat, actorPeerId));
                for (var i = 0; i < 3; i++)
                {
                    commands.Add(MatchCommand.ShuffleAgain(actorSeat, actorPeerId, BuildDeterministicActionSeed(state, actorSeat, i)));
                }

                return commands;
            case OmiPhase.Cut:
                if (actorSeat != state.CutterSeat)
                {
                    return commands;
                }

                for (var cutIndex = 1; cutIndex < Math.Max(2, state.Deck.Count); cutIndex++)
                {
                    commands.Add(MatchCommand.CutDeck(actorSeat, actorPeerId, cutIndex));
                }

                return commands;
            case OmiPhase.TrumpSelect:
                if (actorSeat != state.TrumpSelectorSeat)
                {
                    return commands;
                }

                foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                {
                    commands.Add(MatchCommand.SelectTrump(actorSeat, actorPeerId, suit));
                }

                return commands;
            case OmiPhase.TrickPlay:
                if (actorSeat != state.CurrentTurnSeat || !state.Hands.TryGetValue(actorSeat, out var hand))
                {
                    return commands;
                }

                foreach (var card in hand)
                {
                    if (IsCardPlayableByRule(state, hand, card))
                    {
                        commands.Add(MatchCommand.PlayCard(actorSeat, actorPeerId, card.Id));
                    }
                }

                return commands;
            case OmiPhase.KapothiProposal:
                if (state.KapothiEligibleTeam < 0 || actorSeat.TeamIndex() != state.KapothiEligibleTeam)
                {
                    return commands;
                }

                commands.Add(MatchCommand.KapothiPropose(actorSeat, actorPeerId));
                commands.Add(MatchCommand.KapothiSkip(actorSeat, actorPeerId));
                return commands;
            case OmiPhase.KapothiResponse:
                if (state.KapothiTargetTeam < 0 || !state.KapothiOfferedThisRound || actorSeat.TeamIndex() != state.KapothiTargetTeam)
                {
                    return commands;
                }

                commands.Add(MatchCommand.KapothiAccept(actorSeat, actorPeerId));
                commands.Add(MatchCommand.KapothiReject(actorSeat, actorPeerId));
                return commands;
            case OmiPhase.FirstDeal:
            case OmiPhase.SecondDeal:
            case OmiPhase.TrickResolveHold:
            default:
                return commands;
        }
    }

    public OmiMatchState CloneState(OmiMatchState state)
    {
        return state.DeepClone();
    }

    private static MatchCommandResult StartRound(OmiMatchState state, int seed)
    {
        if (state.Phase != OmiPhase.LobbySeating && state.Phase != OmiPhase.RoundScore)
        {
            return MatchCommandResult.Fail($"Cannot start round from phase {state.Phase}");
        }

        state.RoundNumber += 1;
        state.ResetRoundHandsAndBoard();

        state.ShufflerSeat = state.RoundNumber == 1 ? state.HostSeat : state.ShufflerSeat.Next();
        state.CutterSeat = state.ShufflerSeat.Previous();
        state.TrumpSelectorSeat = state.ShufflerSeat.Next();
        state.CurrentTurnSeat = state.ShufflerSeat;

        state.Phase = OmiPhase.Shuffle;
        state.PhaseDeadlineUnixSeconds = 0;
        state.Deck = DeckService.Shuffle(DeckService.BuildOmiDeck(), seed);
        state.DeckCursor = 0;

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "round_started",
            Payload = new Godot.Collections.Dictionary
            {
                ["roundNumber"] = state.RoundNumber,
                ["shufflerSeat"] = state.ShufflerSeat.ToString(),
                ["cutterSeat"] = state.CutterSeat.ToString(),
                ["trumpSelectorSeat"] = state.TrumpSelectorSeat.ToString(),
                ["phase"] = (int)state.Phase
            }
        });
    }

    private static MatchCommandResult ShuffleAgain(OmiMatchState state, MatchCommand command)
    {
        if (state.Phase != OmiPhase.Shuffle)
        {
            return MatchCommandResult.Fail($"Cannot shuffle during {state.Phase}");
        }

        if (!command.ActorSeat.HasValue || command.ActorSeat.Value != state.ShufflerSeat)
        {
            return MatchCommandResult.Fail("Only the shuffler can reshuffle");
        }

        var shuffledDeck = DeckService.Shuffle(state.Deck.Count > 0 ? state.Deck : DeckService.BuildOmiDeck(), command.Seed);
        state.Deck = shuffledDeck;
        state.DeckCursor = 0;

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "deck_shuffled",
            Payload = new Godot.Collections.Dictionary
            {
                ["phase"] = (int)state.Phase
            }
        });
    }

    private static MatchCommandResult FinishShuffle(OmiMatchState state, MatchCommand command)
    {
        if (state.Phase != OmiPhase.Shuffle)
        {
            return MatchCommandResult.Fail($"Cannot finish shuffle during {state.Phase}");
        }

        if (!command.ActorSeat.HasValue || command.ActorSeat.Value != state.ShufflerSeat)
        {
            return MatchCommandResult.Fail("Only the shuffler can finish shuffling");
        }

        state.Phase = OmiPhase.Cut;
        state.PhaseDeadlineUnixSeconds = 0;
        state.CurrentTurnSeat = state.CutterSeat;

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "shuffle_finished",
            Payload = new Godot.Collections.Dictionary
            {
                ["phase"] = (int)state.Phase,
                ["cutterSeat"] = state.CutterSeat.ToString()
            }
        });
    }

    private static MatchCommandResult CutDeck(OmiMatchState state, MatchCommand command)
    {
        if (state.Phase != OmiPhase.Cut)
        {
            return MatchCommandResult.Fail($"Cannot cut deck during {state.Phase}");
        }

        if (!command.ActorSeat.HasValue || command.ActorSeat.Value != state.CutterSeat)
        {
            return MatchCommandResult.Fail("Only the cutter can cut the deck");
        }

        state.Deck = DeckService.Cut(state.Deck, command.CutIndex);
        state.DeckCursor = 0;

        state.Phase = OmiPhase.FirstDeal;
        DealCardsRoundRobin(state, state.TrumpSelectorSeat, 4);
        state.PhaseDeadlineUnixSeconds = Time.GetUnixTimeFromSystem() + MatchTiming.FirstDealRevealSeconds;
        state.CurrentTurnSeat = state.TrumpSelectorSeat;

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "deck_cut",
            Payload = new Godot.Collections.Dictionary
            {
                ["cutIndex"] = command.CutIndex,
                ["phase"] = (int)state.Phase
            }
        });
    }

    private static MatchCommandResult CompleteFirstDeal(OmiMatchState state)
    {
        if (state.Phase != OmiPhase.FirstDeal)
        {
            return MatchCommandResult.Fail($"Cannot complete first deal during {state.Phase}");
        }

        state.Phase = OmiPhase.TrumpSelect;
        state.PhaseDeadlineUnixSeconds = 0;
        state.CurrentTurnSeat = state.TrumpSelectorSeat;
        return MatchCommandResult.Ok();
    }

    private static MatchCommandResult SelectTrump(OmiMatchState state, MatchCommand command)
    {
        if (state.Phase != OmiPhase.TrumpSelect)
        {
            return MatchCommandResult.Fail($"Cannot select trump during {state.Phase}");
        }

        if (!command.ActorSeat.HasValue || command.ActorSeat.Value != state.TrumpSelectorSeat)
        {
            return MatchCommandResult.Fail("Only the trump selector can choose trump");
        }

        state.TrumpSuit = command.TrumpSuit;
        state.TrumpTeamIndexThisRound = state.TrumpSelectorSeat.TeamIndex();
        state.Phase = OmiPhase.SecondDeal;
        DealCardsRoundRobin(state, state.TrumpSelectorSeat, 4);
        state.PhaseDeadlineUnixSeconds = Time.GetUnixTimeFromSystem() + MatchTiming.SecondDealRevealSeconds;
        state.CurrentTurnSeat = state.TrumpSelectorSeat;

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "trump_selected",
            Payload = new Godot.Collections.Dictionary
            {
                ["trumpSuit"] = (int)state.TrumpSuit.Value,
                ["phase"] = (int)state.Phase,
                ["startingSeat"] = state.CurrentTurnSeat.ToString()
            }
        });
    }

    private static MatchCommandResult CompleteSecondDeal(OmiMatchState state)
    {
        if (state.Phase != OmiPhase.SecondDeal)
        {
            return MatchCommandResult.Fail($"Cannot complete second deal during {state.Phase}");
        }

        state.Phase = OmiPhase.TrickPlay;
        state.PhaseDeadlineUnixSeconds = 0;
        state.CurrentTurnSeat = state.TrumpSelectorSeat;
        return MatchCommandResult.Ok();
    }

    private static MatchCommandResult PlayCard(OmiMatchState state, MatchCommand command)
    {
        if (state.Phase != OmiPhase.TrickPlay)
        {
            return MatchCommandResult.Fail($"Cannot play card during {state.Phase}");
        }

        if (!command.ActorSeat.HasValue)
        {
            return MatchCommandResult.Fail("Actor seat is required");
        }

        var actorSeat = command.ActorSeat.Value;
        if (actorSeat != state.CurrentTurnSeat)
        {
            return MatchCommandResult.Fail("It is not this player's turn");
        }

        var hand = state.Hands[actorSeat];
        var cardIndex = hand.FindIndex(card => card.Id == command.CardId);
        if (cardIndex < 0)
        {
            return MatchCommandResult.Fail("Card not found in hand");
        }

        var chosenCard = hand[cardIndex];

        if (state.CurrentTrickCards.Count > 0)
        {
            var leadSuit = state.CurrentTrickCards[0].Card.Suit;
            var hasLeadSuit = hand.Any(card => card.Suit == leadSuit);
            if (hasLeadSuit && chosenCard.Suit != leadSuit)
            {
                return MatchCommandResult.Fail("Must follow lead suit when possible");
            }
        }

        hand.RemoveAt(cardIndex);
        state.CurrentTrickCards.Add(new PlayedCard(actorSeat, chosenCard));

        var result = new MatchCommandResult { Success = true };
        result.Events.Add(new MatchEvent
        {
            Type = "card_played",
            Payload = new Godot.Collections.Dictionary
            {
                ["seat"] = actorSeat.ToString(),
                ["card"] = chosenCard.ToDictionary(),
                ["trickIndex"] = state.CompletedTricksCount
            }
        });

        if (state.CurrentTrickCards.Count < 4)
        {
            state.CurrentTurnSeat = state.CurrentTurnSeat.Next();
            return result;
        }

        state.Phase = OmiPhase.TrickResolveHold;
        state.PhaseDeadlineUnixSeconds = Time.GetUnixTimeFromSystem() + MatchTiming.TrickResolveHoldSeconds;
        return result;
    }

    private static MatchCommandResult ResolveCurrentTrick(OmiMatchState state)
    {
        if (state.Phase != OmiPhase.TrickResolveHold)
        {
            return MatchCommandResult.Fail($"Cannot resolve trick during {state.Phase}");
        }

        if (state.CurrentTrickCards.Count != 4)
        {
            return MatchCommandResult.Fail("Trick resolve requires 4 cards on desk");
        }

        var winningSeat = DetermineTrickWinner(state.CurrentTrickCards, state.TrumpSuit);
        var winnerTeam = winningSeat.TeamIndex();

        state.TeamTricks[winnerTeam] += 1;
        state.CompletedTricks.Add(state.CurrentTrickCards.ToList());
        state.CurrentTrickCards.Clear();
        state.CompletedTricksCount += 1;
        state.CurrentTurnSeat = winningSeat;
        state.PhaseDeadlineUnixSeconds = 0;

        var result = new MatchCommandResult { Success = true };

        result.Events.Add(new MatchEvent
        {
            Type = "trick_resolved",
            Payload = new Godot.Collections.Dictionary
            {
                ["winnerSeat"] = winningSeat.ToString(),
                ["winnerTeam"] = winnerTeam,
                ["completedTricksCount"] = state.CompletedTricksCount,
                ["teamTricks"] = new Godot.Collections.Array { state.TeamTricks[0], state.TeamTricks[1] }
            }
        });

        if (state.CompletedTricksCount < 8)
        {
            if (ShouldOpenKapothiWindow(state))
            {
                state.KapothiEligibleTeam = winnerTeam;
                state.KapothiTargetTeam = 1 - winnerTeam;
                state.KapothiWindowConsumed = true;
                state.KapothiOfferedThisRound = false;
                state.KapothiAcceptedThisRound = false;
                state.Phase = OmiPhase.KapothiProposal;
                state.PhaseDeadlineUnixSeconds = Time.GetUnixTimeFromSystem() + MatchTiming.KapothiProposalWindowSeconds;

                result.Events.Add(new MatchEvent
                {
                    Type = "kapothi_window_opened",
                    Payload = new Godot.Collections.Dictionary
                    {
                        ["eligibleTeam"] = state.KapothiEligibleTeam,
                        ["targetTeam"] = state.KapothiTargetTeam,
                        ["completedTricksCount"] = state.CompletedTricksCount
                    }
                });
                return result;
            }

            state.Phase = OmiPhase.TrickPlay;
            return result;
        }

        FinalizeRound(state, result);
        return result;
    }

    private static MatchCommandResult KapothiPropose(OmiMatchState state, MatchCommand command)
    {
        if (state.Phase != OmiPhase.KapothiProposal)
        {
            return MatchCommandResult.Fail($"Cannot propose Kapothi during {state.Phase}");
        }

        if (!command.ActorSeat.HasValue)
        {
            return MatchCommandResult.Fail("Actor seat is required");
        }

        if (state.KapothiEligibleTeam < 0 || command.ActorSeat.Value.TeamIndex() != state.KapothiEligibleTeam)
        {
            return MatchCommandResult.Fail("Only the eligible winning team can propose Kapothi");
        }

        state.KapothiOfferedThisRound = true;
        state.Phase = OmiPhase.KapothiResponse;
        state.PhaseDeadlineUnixSeconds = Time.GetUnixTimeFromSystem() + MatchTiming.KapothiResponseWindowSeconds;

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "kapothi_proposed",
            Payload = new Godot.Collections.Dictionary
            {
                ["eligibleTeam"] = state.KapothiEligibleTeam,
                ["targetTeam"] = state.KapothiTargetTeam,
                ["seat"] = command.ActorSeat.Value.ToString()
            }
        });
    }

    private static MatchCommandResult KapothiSkip(OmiMatchState state, MatchCommand command)
    {
        if (state.Phase != OmiPhase.KapothiProposal)
        {
            return MatchCommandResult.Fail($"Cannot skip Kapothi during {state.Phase}");
        }

        if (!command.ActorSeat.HasValue)
        {
            return MatchCommandResult.Fail("Actor seat is required");
        }

        if (state.KapothiEligibleTeam < 0 || command.ActorSeat.Value.TeamIndex() != state.KapothiEligibleTeam)
        {
            return MatchCommandResult.Fail("Only the eligible winning team can skip Kapothi");
        }

        var eligibleTeam = state.KapothiEligibleTeam;
        var targetTeam = state.KapothiTargetTeam;
        CloseKapothiWindow(state);

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "kapothi_skipped",
            Payload = new Godot.Collections.Dictionary
            {
                ["eligibleTeam"] = eligibleTeam,
                ["targetTeam"] = targetTeam,
                ["seat"] = command.ActorSeat.Value.ToString()
            }
        });
    }

    private static MatchCommandResult KapothiAccept(OmiMatchState state, MatchCommand command)
    {
        if (state.Phase != OmiPhase.KapothiResponse)
        {
            return MatchCommandResult.Fail($"Cannot accept Kapothi during {state.Phase}");
        }

        if (!command.ActorSeat.HasValue)
        {
            return MatchCommandResult.Fail("Actor seat is required");
        }

        if (state.KapothiTargetTeam < 0 || command.ActorSeat.Value.TeamIndex() != state.KapothiTargetTeam)
        {
            return MatchCommandResult.Fail("Only the target team can accept Kapothi");
        }

        var targetTeam = state.KapothiTargetTeam;
        state.KapothiAcceptedThisRound = true;
        CloseKapothiWindow(state);

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "kapothi_accepted",
            Payload = new Godot.Collections.Dictionary
            {
                ["targetTeam"] = targetTeam,
                ["seat"] = command.ActorSeat.Value.ToString()
            }
        });
    }

    private static MatchCommandResult KapothiReject(OmiMatchState state, MatchCommand command)
    {
        if (state.Phase != OmiPhase.KapothiResponse)
        {
            return MatchCommandResult.Fail($"Cannot reject Kapothi during {state.Phase}");
        }

        if (!command.ActorSeat.HasValue)
        {
            return MatchCommandResult.Fail("Actor seat is required");
        }

        if (state.KapothiTargetTeam < 0 || command.ActorSeat.Value.TeamIndex() != state.KapothiTargetTeam)
        {
            return MatchCommandResult.Fail("Only the target team can reject Kapothi");
        }

        var targetTeam = state.KapothiTargetTeam;
        state.KapothiAcceptedThisRound = false;
        CloseKapothiWindow(state);

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "kapothi_rejected",
            Payload = new Godot.Collections.Dictionary
            {
                ["targetTeam"] = targetTeam,
                ["seat"] = command.ActorSeat.Value.ToString()
            }
        });
    }

    private static MatchCommandResult ForfeitTeam(OmiMatchState state, int teamIndex)
    {
        if (teamIndex is < 0 or > 1)
        {
            return MatchCommandResult.Fail("Invalid team index");
        }

        var winner = 1 - teamIndex;
        state.MatchWinnerTeam = winner;
        state.RoundWinnerTeam = winner;
        state.TeamCredits[teamIndex] = 0;
        state.Phase = OmiPhase.MatchEnd;
        state.PhaseDeadlineUnixSeconds = 0;

        return MatchCommandResult.Ok(new MatchEvent
        {
            Type = "match_ended",
            Payload = new Godot.Collections.Dictionary
            {
                ["winnerTeam"] = winner,
                ["reason"] = "forfeit"
            }
        });
    }

    private static void FinalizeRound(OmiMatchState state, MatchCommandResult result)
    {
        var team0 = state.TeamTricks[0];
        var team1 = state.TeamTricks[1];

        if (team0 == team1)
        {
            state.RoundWinnerTeam = null;
            if (state.ConsecutiveDraws == 0)
            {
                state.PendingDrawBonusCredits = 1;
                state.ConsecutiveDraws = 1;
            }
            else
            {
                state.PendingDrawBonusCredits = 0;
                state.ConsecutiveDraws = 0;
            }

            state.CurrentStake = 1;
            state.Phase = OmiPhase.RoundScore;
            state.PhaseDeadlineUnixSeconds = 0;

            result.Events.Add(new MatchEvent
            {
                Type = "round_resolved",
                Payload = new Godot.Collections.Dictionary
                {
                    ["winnerTeam"] = -1,
                    ["isDraw"] = true,
                    ["isSweep"] = false,
                    ["teamTricks"] = new Godot.Collections.Array { team0, team1 },
                    ["trumpTeam"] = state.TrumpTeamIndexThisRound,
                    ["pendingDrawBonusNext"] = state.PendingDrawBonusCredits,
                    ["consecutiveDraws"] = state.ConsecutiveDraws
                }
            });
            return;
        }

        var winnerTeam = team0 > team1 ? 0 : 1;
        var loserTeam = 1 - winnerTeam;
        var isSweep = team0 == 8 || team1 == 8;
        var baseLoss = loserTeam == state.TrumpTeamIndexThisRound ? 2 : 1;
        var drawBonus = state.PendingDrawBonusCredits;
        var kapothiBonus = state.KapothiAcceptedThisRound ? 2 : 0;
        var totalLoss = baseLoss + drawBonus + kapothiBonus;

        state.RoundWinnerTeam = winnerTeam;

        state.TeamCredits[loserTeam] = Math.Max(0, state.TeamCredits[loserTeam] - totalLoss);
        state.PendingDrawBonusCredits = 0;
        state.ConsecutiveDraws = 0;
        state.CurrentStake = 1;

        result.Events.Add(new MatchEvent
        {
            Type = "round_resolved",
            Payload = new Godot.Collections.Dictionary
            {
                ["winnerTeam"] = winnerTeam,
                ["isDraw"] = false,
                ["isSweep"] = isSweep,
                ["teamTricks"] = new Godot.Collections.Array { team0, team1 },
                ["loserTeam"] = loserTeam,
                ["trumpTeam"] = state.TrumpTeamIndexThisRound,
                ["baseLoss"] = baseLoss,
                ["drawBonusApplied"] = drawBonus,
                ["kapothiBonusApplied"] = kapothiBonus,
                ["totalLoss"] = totalLoss,
                ["pendingDrawBonusNext"] = state.PendingDrawBonusCredits,
                ["consecutiveDraws"] = state.ConsecutiveDraws
            }
        });

        result.Events.Add(new MatchEvent
        {
            Type = "credits_updated",
            Payload = new Godot.Collections.Dictionary
            {
                ["teamCredits"] = new Godot.Collections.Array { state.TeamCredits[0], state.TeamCredits[1] },
                ["loserTeam"] = loserTeam,
                ["totalLoss"] = totalLoss
            }
        });

        if (state.TeamCredits[loserTeam] <= 0)
        {
            state.MatchWinnerTeam = winnerTeam;
            state.Phase = OmiPhase.MatchEnd;
            state.PhaseDeadlineUnixSeconds = 0;

            result.Events.Add(new MatchEvent
            {
                Type = "match_ended",
                Payload = new Godot.Collections.Dictionary
                {
                    ["winnerTeam"] = winnerTeam,
                    ["reason"] = "credits"
                }
            });
            return;
        }

        state.Phase = OmiPhase.RoundScore;
        state.PhaseDeadlineUnixSeconds = 0;
    }

    private static bool ShouldOpenKapothiWindow(OmiMatchState state)
    {
        if (state.KapothiWindowConsumed || state.CompletedTricksCount != 4)
        {
            return false;
        }

        return (state.TeamTricks[0] == 4 && state.TeamTricks[1] == 0) ||
               (state.TeamTricks[1] == 4 && state.TeamTricks[0] == 0);
    }

    private static void CloseKapothiWindow(OmiMatchState state)
    {
        state.Phase = OmiPhase.TrickPlay;
        state.PhaseDeadlineUnixSeconds = 0;
        state.KapothiEligibleTeam = -1;
        state.KapothiTargetTeam = -1;
    }

    private static bool IsCardPlayableByRule(OmiMatchState state, IReadOnlyList<CardModel> hand, CardModel candidate)
    {
        if (state.CurrentTrickCards.Count == 0)
        {
            return true;
        }

        var leadSuit = state.CurrentTrickCards[0].Card.Suit;
        var hasLeadSuit = hand.Any(card => card.Suit == leadSuit);
        return !hasLeadSuit || candidate.Suit == leadSuit;
    }

    private static int BuildDeterministicActionSeed(OmiMatchState state, SeatPosition seat, int actionIndex)
    {
        unchecked
        {
            var seed = 17;
            seed = seed * 31 + state.RoundNumber;
            seed = seed * 31 + state.CompletedTricksCount;
            seed = seed * 31 + (int)seat;
            seed = seed * 31 + actionIndex;
            seed = seed * 31 + state.TeamCredits[0];
            seed = seed * 31 + state.TeamCredits[1];
            return seed;
        }
    }

    private static SeatPosition DetermineTrickWinner(IReadOnlyList<PlayedCard> trickCards, CardSuit? trumpSuit)
    {
        if (trickCards.Count == 0)
        {
            throw new InvalidOperationException("Trick has no cards");
        }

        var leadSuit = trickCards[0].Card.Suit;

        var candidateCards = trickCards.AsEnumerable();
        if (trumpSuit.HasValue && trickCards.Any(card => card.Card.Suit == trumpSuit.Value))
        {
            candidateCards = trickCards.Where(card => card.Card.Suit == trumpSuit.Value);
        }
        else
        {
            candidateCards = trickCards.Where(card => card.Card.Suit == leadSuit);
        }

        return candidateCards
            .OrderByDescending(card => card.Card.Strength)
            .First()
            .Seat;
    }

    private static void DealCardsRoundRobin(OmiMatchState state, SeatPosition startSeat, int cardsPerSeat)
    {
        var order = startSeat.OrderedFrom();
        for (var i = 0; i < cardsPerSeat; i++)
        {
            foreach (var seat in order)
            {
                if (state.DeckCursor >= state.Deck.Count)
                {
                    throw new InvalidOperationException("Deck exhausted while dealing");
                }

                state.Hands[seat].Add(state.Deck[state.DeckCursor]);
                state.DeckCursor += 1;
            }
        }
    }
}
