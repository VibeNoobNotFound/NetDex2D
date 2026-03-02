using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NetDex.Core.Commands;
using NetDex.Core.Enums;
using NetDex.Core.Models;
using NetDex.Core.Rules;

namespace NetDex.AI;

public sealed class OmiBotPolicy : IOmiBotPolicy
{
    private sealed record SearchSettings(
        int TimeBudgetMs,
        int MinSamplesPerCommand,
        int TrumpRolloutsPerSuit,
        int MaxReshuffles,
        double ExtraShuffleChance);

    private sealed record OutcomeEstimate(
        double AverageUtility,
        double WinRate,
        int Samples);

    public MatchCommand ChooseCommand(BotPerceptionState perception, IGameRulesEngine rulesEngine, CancellationToken cancellationToken)
    {
        var state = perception.PublicState;
        var legal = rulesEngine.EnumerateLegalCommands(state, perception.BotSeat, perception.BotPeerId);
        if (legal.Count == 0)
        {
            throw new InvalidOperationException($"No legal bot command for phase {state.Phase} at seat {perception.BotSeat}");
        }

        var settings = ResolveSettings(perception.Difficulty);
        var random = new Random(perception.DecisionSeed);

        return state.Phase switch
        {
            OmiPhase.Shuffle => ChooseShuffleCommand(legal, perception, settings, random),
            OmiPhase.Cut => ChooseCutCommand(legal, random, perception.Difficulty),
            OmiPhase.TrumpSelect => ChooseTrumpCommand(legal, perception, rulesEngine, settings, random, cancellationToken),
            OmiPhase.TrickPlay => ChooseTrickPlayCommand(legal, perception, rulesEngine, settings, random, cancellationToken),
            OmiPhase.KapothiProposal => ChooseKapothiProposalCommand(legal, perception, rulesEngine, settings, random, cancellationToken),
            OmiPhase.KapothiResponse => ChooseKapothiResponseCommand(legal, perception, rulesEngine, settings, random, cancellationToken),
            _ => legal[0]
        };
    }

    private static SearchSettings ResolveSettings(AiDifficulty difficulty)
    {
        return difficulty switch
        {
            AiDifficulty.Easy => new SearchSettings(
                TimeBudgetMs: 120,
                MinSamplesPerCommand: 4,
                TrumpRolloutsPerSuit: 4,
                MaxReshuffles: 0,
                ExtraShuffleChance: 0.15),
            AiDifficulty.Normal => new SearchSettings(
                TimeBudgetMs: 320,
                MinSamplesPerCommand: 10,
                TrumpRolloutsPerSuit: 10,
                MaxReshuffles: 1,
                ExtraShuffleChance: 0.45),
            _ => new SearchSettings(
                TimeBudgetMs: 650,
                MinSamplesPerCommand: 22,
                TrumpRolloutsPerSuit: 22,
                MaxReshuffles: 2,
                ExtraShuffleChance: 0.65)
        };
    }

    private static MatchCommand ChooseShuffleCommand(
        IReadOnlyList<MatchCommand> legal,
        BotPerceptionState perception,
        SearchSettings settings,
        Random random)
    {
        var finish = legal.FirstOrDefault(command => command.Type == MatchCommandType.FinishShuffle);
        var reshuffles = legal.Where(command => command.Type == MatchCommandType.ShuffleAgain).ToList();
        if (finish != null || reshuffles.Count == 0)
        {
            if (perception.ShuffleCountForRound >= settings.MaxReshuffles)
            {
                return finish ?? legal[0];
            }

            if (random.NextDouble() < settings.ExtraShuffleChance)
            {
                return reshuffles[random.Next(reshuffles.Count)];
            }
        }

        return finish ?? legal[0];
    }

    private static MatchCommand ChooseCutCommand(IReadOnlyList<MatchCommand> legal, Random random, AiDifficulty difficulty)
    {
        var cuts = legal
            .Where(command => command.Type == MatchCommandType.CutDeck)
            .OrderBy(command => command.CutIndex)
            .ToList();
        if (cuts.Count == 0)
        {
            return legal[0];
        }

        if (difficulty == AiDifficulty.Easy)
        {
            return cuts[random.Next(cuts.Count)];
        }

        var middle = cuts[cuts.Count / 2].CutIndex;
        var jitter = difficulty == AiDifficulty.Normal
            ? random.Next(-4, 5)
            : random.Next(-2, 3);
        var target = middle + jitter;

        return cuts
            .OrderBy(command => Math.Abs(command.CutIndex - target))
            .ThenBy(command => command.CutIndex)
            .First();
    }

    private MatchCommand ChooseTrumpCommand(
        IReadOnlyList<MatchCommand> legal,
        BotPerceptionState perception,
        IGameRulesEngine rulesEngine,
        SearchSettings settings,
        Random random,
        CancellationToken cancellationToken)
    {
        var botHand = perception.PublicState.Hands[perception.BotSeat];
        var suitScores = new Dictionary<CardSuit, double>();

        foreach (var command in legal.Where(command => command.Type == MatchCommandType.SelectTrump))
        {
            var suit = command.TrumpSuit;
            var score = ScoreTrumpFromHand(botHand, suit);

            var rolloutScore = 0.0;
            var rolloutCount = 0;
            for (var i = 0; i < settings.TrumpRolloutsPerSuit; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var simulated = SampleDeterminizedState(perception, random, strictVoidConstraints: false);
                var applied = rulesEngine.ApplyCommand(simulated, command);
                if (!applied.Success)
                {
                    continue;
                }

                RolloutToRoundEnd(simulated, perception.BotSeat, rulesEngine, random, cancellationToken);
                rolloutScore += EvaluateRoundUtility(simulated, perception.BotSeat.TeamIndex());
                rolloutCount += 1;
            }

            if (rolloutCount > 0)
            {
                score += rolloutScore / rolloutCount;
            }

            suitScores[suit] = score;
        }

        if (suitScores.Count == 0)
        {
            return legal[0];
        }

        var bestSuit = suitScores
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => pair.Key)
            .First()
            .Key;

        return legal.First(command => command.Type == MatchCommandType.SelectTrump && command.TrumpSuit == bestSuit);
    }

    private MatchCommand ChooseTrickPlayCommand(
        IReadOnlyList<MatchCommand> legal,
        BotPerceptionState perception,
        IGameRulesEngine rulesEngine,
        SearchSettings settings,
        Random random,
        CancellationToken cancellationToken)
    {
        var playableCards = legal.Where(command => command.Type == MatchCommandType.PlayCard).ToList();
        if (playableCards.Count == 0)
        {
            return legal[0];
        }

        if (playableCards.Count == 1 || perception.Difficulty == AiDifficulty.Easy)
        {
            return SelectBestImmediatePlay(perception.PublicState, playableCards, perception.BotSeat);
        }

        var accumulated = new Dictionary<string, (double total, int visits)>();
        foreach (var command in playableCards)
        {
            accumulated[command.CardId] = (0.0, 0);
        }

        var stopwatch = Stopwatch.StartNew();
        while (!cancellationToken.IsCancellationRequested && stopwatch.ElapsedMilliseconds < settings.TimeBudgetMs)
        {
            foreach (var command in playableCards)
            {
                if (cancellationToken.IsCancellationRequested || stopwatch.ElapsedMilliseconds >= settings.TimeBudgetMs)
                {
                    break;
                }

                var score = SimulateCandidate(command, perception, rulesEngine, random, cancellationToken);
                var current = accumulated[command.CardId];
                accumulated[command.CardId] = (current.total + score, current.visits + 1);
            }
        }

        var best = playableCards[0];
        var bestValue = double.MinValue;

        foreach (var command in playableCards)
        {
            var data = accumulated[command.CardId];
            if (data.visits < settings.MinSamplesPerCommand)
            {
                // Blend with deterministic immediate heuristic to keep output stable at low sample counts.
                var immediate = ScoreImmediatePlay(perception.PublicState, command, perception.BotSeat);
                var value = immediate + (data.visits == 0 ? 0 : data.total / data.visits);
                if (value > bestValue)
                {
                    bestValue = value;
                    best = command;
                }

                continue;
            }

            var avg = data.total / data.visits;
            if (avg > bestValue)
            {
                bestValue = avg;
                best = command;
            }
        }

        return best;
    }

    private MatchCommand ChooseKapothiProposalCommand(
        IReadOnlyList<MatchCommand> legal,
        BotPerceptionState perception,
        IGameRulesEngine rulesEngine,
        SearchSettings settings,
        Random random,
        CancellationToken cancellationToken)
    {
        var propose = legal.FirstOrDefault(command => command.Type == MatchCommandType.KapothiPropose);
        var skip = legal.FirstOrDefault(command => command.Type == MatchCommandType.KapothiSkip);
        if (propose == null)
        {
            return skip ?? legal[0];
        }

        if (skip == null)
        {
            return propose;
        }

        var sampleCount = Math.Max(6, settings.MinSamplesPerCommand);
        var proposeOutcome = EstimateOutcome(propose, perception, rulesEngine, random, cancellationToken, sampleCount);
        var skipOutcome = EstimateOutcome(skip, perception, rulesEngine, random, cancellationToken, sampleCount);
        if (proposeOutcome.Samples == 0)
        {
            return skip;
        }

        if (skipOutcome.Samples == 0)
        {
            return propose;
        }

        var aggressivenessBias = perception.Difficulty switch
        {
            AiDifficulty.Easy => -4.0,
            AiDifficulty.Normal => -1.0,
            _ => 1.5
        };

        return proposeOutcome.AverageUtility + aggressivenessBias >= skipOutcome.AverageUtility
            ? propose
            : skip;
    }

    private MatchCommand ChooseKapothiResponseCommand(
        IReadOnlyList<MatchCommand> legal,
        BotPerceptionState perception,
        IGameRulesEngine rulesEngine,
        SearchSettings settings,
        Random random,
        CancellationToken cancellationToken)
    {
        var accept = legal.FirstOrDefault(command => command.Type == MatchCommandType.KapothiAccept);
        var reject = legal.FirstOrDefault(command => command.Type == MatchCommandType.KapothiReject);
        if (accept == null)
        {
            return reject ?? legal[0];
        }

        if (reject == null)
        {
            return accept;
        }

        var sampleCount = Math.Max(8, settings.MinSamplesPerCommand + 2);
        var acceptOutcome = EstimateOutcome(accept, perception, rulesEngine, random, cancellationToken, sampleCount);
        var rejectOutcome = EstimateOutcome(reject, perception, rulesEngine, random, cancellationToken, sampleCount);
        if (acceptOutcome.Samples == 0)
        {
            return reject;
        }

        if (rejectOutcome.Samples == 0)
        {
            return accept;
        }

        var comebackThreshold = perception.Difficulty switch
        {
            AiDifficulty.Easy => 0.28,
            AiDifficulty.Normal => 0.42,
            _ => 0.56
        };

        var acceptMargin = perception.Difficulty switch
        {
            AiDifficulty.Easy => 8.0,
            AiDifficulty.Normal => 3.0,
            _ => 0.0
        };

        var shouldAccept = acceptOutcome.WinRate >= comebackThreshold &&
                           acceptOutcome.AverageUtility + acceptMargin >= rejectOutcome.AverageUtility;
        return shouldAccept ? accept : reject;
    }

    private double SimulateCandidate(
        MatchCommand candidate,
        BotPerceptionState perception,
        IGameRulesEngine rulesEngine,
        Random random,
        CancellationToken cancellationToken)
    {
        var simulated = SampleDeterminizedState(perception, random, strictVoidConstraints: true);
        var first = rulesEngine.ApplyCommand(simulated, candidate);
        if (!first.Success)
        {
            return -1000.0;
        }

        RolloutToRoundEnd(simulated, perception.BotSeat, rulesEngine, random, cancellationToken);
        return EvaluateRoundUtility(simulated, perception.BotSeat.TeamIndex());
    }

    private OutcomeEstimate EstimateOutcome(
        MatchCommand initialCommand,
        BotPerceptionState perception,
        IGameRulesEngine rulesEngine,
        Random random,
        CancellationToken cancellationToken,
        int sampleCount)
    {
        var team = perception.BotSeat.TeamIndex();
        var total = 0.0;
        var wins = 0;
        var samples = 0;

        for (var i = 0; i < sampleCount; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var simulated = SampleDeterminizedState(perception, random, strictVoidConstraints: true);
            var applied = rulesEngine.ApplyCommand(simulated, initialCommand);
            if (!applied.Success)
            {
                continue;
            }

            RolloutToRoundEnd(simulated, perception.BotSeat, rulesEngine, random, cancellationToken);
            total += EvaluateRoundUtility(simulated, team);
            if (simulated.RoundWinnerTeam.HasValue && simulated.RoundWinnerTeam.Value == team)
            {
                wins += 1;
            }

            samples += 1;
        }

        if (samples == 0)
        {
            return new OutcomeEstimate(double.NegativeInfinity, 0.0, 0);
        }

        return new OutcomeEstimate(total / samples, wins / (double)samples, samples);
    }

    private void RolloutToRoundEnd(
        OmiMatchState state,
        SeatPosition botSeat,
        IGameRulesEngine rulesEngine,
        Random random,
        CancellationToken cancellationToken)
    {
        const int safetyCap = 256;
        var safety = safetyCap;
        while (!cancellationToken.IsCancellationRequested && safety-- > 0)
        {
            if (state.Phase is OmiPhase.RoundScore or OmiPhase.MatchEnd)
            {
                return;
            }

            if (state.Phase == OmiPhase.FirstDeal)
            {
                var transition = rulesEngine.ApplyCommand(state, MatchCommand.CompleteFirstDeal());
                if (!transition.Success)
                {
                    return;
                }

                continue;
            }

            if (state.Phase == OmiPhase.SecondDeal)
            {
                var transition = rulesEngine.ApplyCommand(state, MatchCommand.CompleteSecondDeal());
                if (!transition.Success)
                {
                    return;
                }

                continue;
            }

            if (state.Phase == OmiPhase.TrickResolveHold)
            {
                var transition = rulesEngine.ApplyCommand(state, MatchCommand.ResolveCurrentTrick());
                if (!transition.Success)
                {
                    return;
                }

                continue;
            }

            if (!TryResolveRolloutActor(state, botSeat, rulesEngine, out var actorSeat, out var legal))
            {
                return;
            }

            var command = ChooseRolloutCommand(state, actorSeat, legal, random);
            var result = rulesEngine.ApplyCommand(state, command);
            if (!result.Success)
            {
                result = rulesEngine.ApplyCommand(state, legal[0]);
                if (!result.Success)
                {
                    return;
                }
            }
        }
    }

    private MatchCommand ChooseRolloutCommand(
        OmiMatchState state,
        SeatPosition actorSeat,
        IReadOnlyList<MatchCommand> legal,
        Random random)
    {
        if (state.Phase != OmiPhase.TrickPlay)
        {
            return state.Phase switch
            {
                OmiPhase.Shuffle => legal.FirstOrDefault(command => command.Type == MatchCommandType.FinishShuffle) ?? legal[0],
                OmiPhase.Cut => legal[legal.Count / 2],
                OmiPhase.TrumpSelect => ChooseBestTrumpForSeat(state, legal, actorSeat),
                OmiPhase.KapothiProposal => random.NextDouble() < 0.55
                    ? legal.FirstOrDefault(command => command.Type == MatchCommandType.KapothiPropose) ?? legal[0]
                    : legal.FirstOrDefault(command => command.Type == MatchCommandType.KapothiSkip) ?? legal[0],
                OmiPhase.KapothiResponse => random.NextDouble() < 0.72
                    ? legal.FirstOrDefault(command => command.Type == MatchCommandType.KapothiReject) ?? legal[0]
                    : legal.FirstOrDefault(command => command.Type == MatchCommandType.KapothiAccept) ?? legal[0],
                _ => legal[0]
            };
        }

        var plays = legal.Where(command => command.Type == MatchCommandType.PlayCard).ToList();
        if (plays.Count == 0)
        {
            return legal[0];
        }

        if (plays.Count == 1)
        {
            return plays[0];
        }

        // Rollout policy is intentionally noisy to avoid deterministic traps.
        if (random.NextDouble() < 0.2)
        {
            return plays[random.Next(plays.Count)];
        }

        return SelectBestImmediatePlay(state, plays, actorSeat);
    }

    private static MatchCommand ChooseBestTrumpForSeat(OmiMatchState state, IReadOnlyList<MatchCommand> legal, SeatPosition seat)
    {
        var hand = state.Hands[seat];
        var suits = legal.Where(command => command.Type == MatchCommandType.SelectTrump).ToList();
        if (suits.Count == 0)
        {
            return legal[0];
        }

        return suits
            .OrderByDescending(command => ScoreTrumpFromHand(hand, command.TrumpSuit))
            .ThenBy(command => command.TrumpSuit)
            .First();
    }

    private static MatchCommand SelectBestImmediatePlay(OmiMatchState state, IReadOnlyList<MatchCommand> playable, SeatPosition seat)
    {
        return playable
            .OrderByDescending(command => ScoreImmediatePlay(state, command, seat))
            .ThenBy(command => command.CardId, StringComparer.Ordinal)
            .First();
    }

    private static double ScoreImmediatePlay(OmiMatchState state, MatchCommand command, SeatPosition seat)
    {
        var hand = state.Hands[seat];
        var card = hand.FirstOrDefault(model => model.Id == command.CardId);
        if (string.IsNullOrWhiteSpace(card.Id))
        {
            return -1000.0;
        }

        var score = (double)card.Rank;
        if (state.TrumpSuit.HasValue && card.Suit == state.TrumpSuit.Value)
        {
            score += 8.0;
        }

        if (state.CurrentTrickCards.Count > 0)
        {
            var leadSuit = state.CurrentTrickCards[0].Card.Suit;
            if (card.Suit == leadSuit)
            {
                score += 2.0;
            }

            var winnerAfterPlay = DetermineWinnerWithCandidate(state.CurrentTrickCards, card, seat, state.TrumpSuit);
            if (winnerAfterPlay == seat)
            {
                score += 12.0;
            }

            var teammateSeat = seat.Next().Next();
            if (winnerAfterPlay == teammateSeat)
            {
                score -= Math.Max(0, (int)card.Rank - (int)CardRank.Nine);
            }
        }

        return score;
    }

    private static double ScoreTrumpFromHand(IReadOnlyList<CardModel> hand, CardSuit suit)
    {
        var score = 0.0;
        foreach (var card in hand)
        {
            if (card.Suit == suit)
            {
                score += 4.0;
                score += card.Rank switch
                {
                    CardRank.Ace => 6.0,
                    CardRank.King => 4.0,
                    CardRank.Queen => 3.0,
                    CardRank.Jack => 2.0,
                    _ => 1.0
                };
            }
            else if (card.Rank == CardRank.Ace)
            {
                score += 1.5;
            }
        }

        return score;
    }

    private static SeatPosition ResolveActorSeat(OmiMatchState state)
    {
        return state.Phase switch
        {
            OmiPhase.Shuffle => state.ShufflerSeat,
            OmiPhase.Cut => state.CutterSeat,
            OmiPhase.TrumpSelect => state.TrumpSelectorSeat,
            OmiPhase.KapothiProposal => ResolveTeamAnchorSeat(state.KapothiEligibleTeam),
            OmiPhase.KapothiResponse => ResolveTeamAnchorSeat(state.KapothiTargetTeam),
            _ => state.CurrentTurnSeat
        };
    }

    private static SeatPosition ResolveTeamAnchorSeat(int teamIndex)
    {
        return teamIndex switch
        {
            0 => SeatPosition.Bottom,
            1 => SeatPosition.Right,
            _ => SeatPosition.Bottom
        };
    }

    private static bool TryResolveRolloutActor(
        OmiMatchState state,
        SeatPosition botSeat,
        IGameRulesEngine rulesEngine,
        out SeatPosition actorSeat,
        out IReadOnlyList<MatchCommand> legalCommands)
    {
        actorSeat = ResolveActorSeat(state);
        legalCommands = rulesEngine.EnumerateLegalCommands(state, actorSeat, BuildRolloutPeerId(actorSeat, botSeat));
        if (legalCommands.Count > 0)
        {
            return true;
        }

        foreach (SeatPosition candidate in Enum.GetValues(typeof(SeatPosition)))
        {
            if (candidate == actorSeat)
            {
                continue;
            }

            var candidateLegal = rulesEngine.EnumerateLegalCommands(state, candidate, BuildRolloutPeerId(candidate, botSeat));
            if (candidateLegal.Count == 0)
            {
                continue;
            }

            actorSeat = candidate;
            legalCommands = candidateLegal;
            return true;
        }

        legalCommands = Array.Empty<MatchCommand>();
        return false;
    }

    private static int BuildRolloutPeerId(SeatPosition seat, SeatPosition botSeat)
    {
        return seat == botSeat ? -60000 : -70000 - (int)seat;
    }

    private static SeatPosition DetermineWinnerWithCandidate(
        IReadOnlyList<PlayedCard> currentTrick,
        CardModel candidate,
        SeatPosition candidateSeat,
        CardSuit? trumpSuit)
    {
        var allCards = new List<PlayedCard>(currentTrick.Count + 1);
        allCards.AddRange(currentTrick);
        allCards.Add(new PlayedCard(candidateSeat, candidate));

        var leadSuit = allCards[0].Card.Suit;
        IEnumerable<PlayedCard> candidates = allCards;
        if (trumpSuit.HasValue && allCards.Any(card => card.Card.Suit == trumpSuit.Value))
        {
            candidates = allCards.Where(card => card.Card.Suit == trumpSuit.Value);
        }
        else
        {
            candidates = allCards.Where(card => card.Card.Suit == leadSuit);
        }

        return candidates.OrderByDescending(card => card.Card.Strength).First().Seat;
    }

    private static double EvaluateRoundUtility(OmiMatchState state, int botTeam)
    {
        var opponent = 1 - botTeam;
        var trickDelta = state.TeamTricks[botTeam] - state.TeamTricks[opponent];
        var utility = trickDelta * 9.0;

        if (state.RoundWinnerTeam.HasValue)
        {
            utility += state.RoundWinnerTeam.Value == botTeam ? 120.0 : -120.0;
        }

        if (state.MatchWinnerTeam.HasValue)
        {
            utility += state.MatchWinnerTeam.Value == botTeam ? 180.0 : -180.0;
        }

        utility += (state.TeamCredits[botTeam] - state.TeamCredits[opponent]) * 3.0;

        if (trickDelta < 0)
        {
            utility -= state.PendingDrawBonusCredits * 6.0;
            if (state.KapothiAcceptedThisRound)
            {
                utility -= 12.0;
            }
        }
        else if (trickDelta > 0)
        {
            utility += state.PendingDrawBonusCredits * 2.0;
            if (state.KapothiAcceptedThisRound)
            {
                utility += 5.0;
            }
        }

        if (state.TrumpTeamIndexThisRound == botTeam && trickDelta < 0)
        {
            utility -= 4.0;
        }

        return utility;
    }

    private static OmiMatchState SampleDeterminizedState(BotPerceptionState perception, Random random, bool strictVoidConstraints)
    {
        var clone = perception.PublicState.DeepClone();
        var knownCards = new HashSet<string>(StringComparer.Ordinal);

        foreach (var card in clone.Hands[perception.BotSeat])
        {
            knownCards.Add(card.Id);
        }

        foreach (var played in clone.CurrentTrickCards)
        {
            knownCards.Add(played.Card.Id);
        }

        foreach (var trick in clone.CompletedTricks)
        {
            foreach (var played in trick)
            {
                knownCards.Add(played.Card.Id);
            }
        }

        var unknownPool = DeckService.BuildOmiDeck()
            .Where(card => !knownCards.Contains(card.Id))
            .ToList();

        var nonBotSeats = Enum.GetValues<SeatPosition>()
            .Where(seat => seat != perception.BotSeat)
            .ToList();

        var requiredSeatCards = nonBotSeats.ToDictionary(
            seat => seat,
            seat => perception.PublicHandCounts.TryGetValue(seat, out var count) ? count : 0);

        var requiredForHands = requiredSeatCards.Values.Sum();
        var remainingDeckCount = Math.Max(0, perception.RemainingDeckCount);
        if (requiredForHands + remainingDeckCount != unknownPool.Count)
        {
            remainingDeckCount = Math.Max(0, unknownPool.Count - requiredForHands);
        }

        if (!TryDistributeUnknownCards(
                unknownPool,
                nonBotSeats,
                requiredSeatCards,
                remainingDeckCount,
                perception.VoidTracker,
                strictVoidConstraints,
                random,
                out var seatHands,
                out var deckRemainder))
        {
            TryDistributeUnknownCards(
                unknownPool,
                nonBotSeats,
                requiredSeatCards,
                remainingDeckCount,
                perception.VoidTracker,
                strictVoidConstraints: false,
                random,
                out seatHands,
                out deckRemainder);
        }

        foreach (var seat in nonBotSeats)
        {
            clone.Hands[seat].Clear();
            if (seatHands.TryGetValue(seat, out var cards))
            {
                clone.Hands[seat].AddRange(cards);
            }
        }

        var prefix = clone.Deck.Take(clone.DeckCursor).ToList();
        prefix.AddRange(deckRemainder);
        clone.Deck = prefix;

        return clone;
    }

    private static bool TryDistributeUnknownCards(
        IReadOnlyList<CardModel> unknownPool,
        IReadOnlyList<SeatPosition> seats,
        IReadOnlyDictionary<SeatPosition, int> requiredBySeat,
        int remainingDeckCount,
        VoidSuitTracker voidTracker,
        bool strictVoidConstraints,
        Random random,
        out Dictionary<SeatPosition, List<CardModel>> seatHands,
        out List<CardModel> deckRemainder)
    {
        seatHands = new Dictionary<SeatPosition, List<CardModel>>();
        deckRemainder = new List<CardModel>();

        for (var attempt = 0; attempt < 64; attempt++)
        {
            var pool = unknownPool.ToList();
            ShuffleInPlace(pool, random);

            var seatsOrder = seats.ToList();
            ShuffleInPlace(seatsOrder, random);

            var ok = true;
            var distribution = new Dictionary<SeatPosition, List<CardModel>>();
            foreach (var seat in seatsOrder)
            {
                var required = requiredBySeat.TryGetValue(seat, out var count) ? count : 0;
                var cards = new List<CardModel>(required);
                for (var i = 0; i < required; i++)
                {
                    var index = pool.FindIndex(card =>
                        !strictVoidConstraints || voidTracker.CanSeatHoldSuit(seat, card.Suit));
                    if (index < 0)
                    {
                        ok = false;
                        break;
                    }

                    cards.Add(pool[index]);
                    pool.RemoveAt(index);
                }

                if (!ok)
                {
                    break;
                }

                distribution[seat] = cards;
            }

            if (!ok || pool.Count < remainingDeckCount)
            {
                continue;
            }

            seatHands = distribution;
            deckRemainder = pool.Take(remainingDeckCount).ToList();
            return true;
        }

        return false;
    }

    private static void ShuffleInPlace<T>(IList<T> values, Random random)
    {
        for (var i = values.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }
    }
}
