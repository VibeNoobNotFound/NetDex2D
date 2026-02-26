using System;
using System.Collections.Generic;
using System.Linq;
using NetDex.Core.Commands;
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
            MatchCommandType.CutDeck => CutDeck(state, command),
            MatchCommandType.SelectTrump => SelectTrump(state, command),
            MatchCommandType.PlayCard => PlayCard(state, command),
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
        state.CurrentTurnSeat = state.CutterSeat;

        state.Phase = OmiPhase.Shuffle;
        state.Deck = DeckService.Shuffle(DeckService.BuildOmiDeck(), seed);
        state.DeckCursor = 0;

        state.Phase = OmiPhase.Cut;

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

        state.Phase = OmiPhase.TrumpSelect;
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
        state.Phase = OmiPhase.SecondDeal;
        DealCardsRoundRobin(state, state.TrumpSelectorSeat, 4);

        state.Phase = OmiPhase.TrickPlay;
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

        var winningSeat = DetermineTrickWinner(state.CurrentTrickCards, state.TrumpSuit);
        var winnerTeam = winningSeat.TeamIndex();

        state.TeamTricks[winnerTeam] += 1;
        state.CompletedTricks.Add(state.CurrentTrickCards.ToList());
        state.CurrentTrickCards.Clear();
        state.CompletedTricksCount += 1;
        state.CurrentTurnSeat = winningSeat;

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
            return result;
        }

        FinalizeRound(state, result);
        return result;
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

        var isSweep = team0 == 8 || team1 == 8;
        if (isSweep)
        {
            state.RoundWinnerTeam = team0 == 8 ? 0 : 1;
            state.MatchWinnerTeam = state.RoundWinnerTeam;
            state.Phase = OmiPhase.MatchEnd;

            result.Events.Add(new MatchEvent
            {
                Type = "round_resolved",
                Payload = new Godot.Collections.Dictionary
                {
                    ["winnerTeam"] = state.RoundWinnerTeam.Value,
                    ["isDraw"] = false,
                    ["isSweep"] = true,
                    ["teamTricks"] = new Godot.Collections.Array { team0, team1 }
                }
            });

            result.Events.Add(new MatchEvent
            {
                Type = "match_ended",
                Payload = new Godot.Collections.Dictionary
                {
                    ["winnerTeam"] = state.MatchWinnerTeam.Value,
                    ["reason"] = "sweep"
                }
            });
            return;
        }

        if (team0 == team1)
        {
            state.RoundWinnerTeam = null;
            state.CurrentStake = 2;
            state.Phase = OmiPhase.RoundScore;

            result.Events.Add(new MatchEvent
            {
                Type = "round_resolved",
                Payload = new Godot.Collections.Dictionary
                {
                    ["winnerTeam"] = -1,
                    ["isDraw"] = true,
                    ["isSweep"] = false,
                    ["nextStake"] = state.CurrentStake,
                    ["teamTricks"] = new Godot.Collections.Array { team0, team1 }
                }
            });
            return;
        }

        var winnerTeam = team0 > team1 ? 0 : 1;
        var loserTeam = 1 - winnerTeam;
        state.RoundWinnerTeam = winnerTeam;

        state.TeamCredits[loserTeam] = Math.Max(0, state.TeamCredits[loserTeam] - state.CurrentStake);
        state.CurrentStake = 1;

        result.Events.Add(new MatchEvent
        {
            Type = "round_resolved",
            Payload = new Godot.Collections.Dictionary
            {
                ["winnerTeam"] = winnerTeam,
                ["isDraw"] = false,
                ["isSweep"] = false,
                ["teamTricks"] = new Godot.Collections.Array { team0, team1 }
            }
        });

        result.Events.Add(new MatchEvent
        {
            Type = "credits_updated",
            Payload = new Godot.Collections.Dictionary
            {
                ["teamCredits"] = new Godot.Collections.Array { state.TeamCredits[0], state.TeamCredits[1] }
            }
        });

        if (state.TeamCredits[loserTeam] <= 0)
        {
            state.MatchWinnerTeam = winnerTeam;
            state.Phase = OmiPhase.MatchEnd;

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
