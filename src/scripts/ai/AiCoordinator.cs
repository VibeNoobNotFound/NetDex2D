using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using NetDex.Core.Commands;
using NetDex.Core.Enums;
using NetDex.Core.Models;
using NetDex.Core.Rules;
using NetDex.Lobby;

namespace NetDex.AI;

public partial class AiCoordinator : Node
{
    private sealed record PendingDecision(
        int StateVersion,
        MatchCommand Command,
        int BotPeerId,
        SeatPosition BotSeat,
        int RoundNumber,
        double ExecuteAtUnixSeconds);

    public static AiCoordinator Instance { get; private set; } = null!;

    private readonly IOmiBotPolicy _policy = new OmiBotPolicy();
    private readonly IGameRulesEngine _rulesEngine = GameTypeRegistry.Resolve(GameType.Omi);
    private readonly Dictionary<string, int> _shuffleCounts = new();

    private CancellationTokenSource? _activeSearchCts;
    private Task<MatchCommand>? _activeSearchTask;
    private int _activeSearchVersion = -1;
    private PendingDecision? _pendingDecision;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;

        MatchCoordinator.Instance.MatchStateAdvanced += OnMatchStateAdvanced;
        LobbyManager.Instance.RoomStateChanged += OnRoomStateChanged;
    }

    public override void _ExitTree()
    {
        if (MatchCoordinator.Instance != null)
        {
            MatchCoordinator.Instance.MatchStateAdvanced -= OnMatchStateAdvanced;
        }

        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.RoomStateChanged -= OnRoomStateChanged;
        }

        CancelActiveSearch();
    }

    public override void _Process(double delta)
    {
        PollSearchCompletion();
        TryExecutePendingDecision();
    }

    private void OnMatchStateAdvanced()
    {
        ScheduleIfNeeded();
    }

    private void OnRoomStateChanged()
    {
        ScheduleIfNeeded();
    }

    private void ScheduleIfNeeded()
    {
        if (!Multiplayer.IsServer())
        {
            CancelActiveSearch();
            _pendingDecision = null;
            return;
        }

        var room = LobbyManager.Instance.CurrentRoom;
        var coordinator = MatchCoordinator.Instance;
        var state = coordinator.GetAuthoritativeState();

        if (room == null || state == null || !coordinator.IsMatchRunning)
        {
            CancelActiveSearch();
            _pendingDecision = null;
            return;
        }

        if (state.IsPausedForReconnect || state.Phase is
                OmiPhase.LobbySeating or
                OmiPhase.FirstDeal or
                OmiPhase.SecondDeal or
                OmiPhase.TrickResolveHold or
                OmiPhase.RoundScore or
                OmiPhase.MatchScore or
                OmiPhase.MatchEnd or
                OmiPhase.PausedReconnect)
        {
            CancelActiveSearch();
            _pendingDecision = null;
            return;
        }

        PruneShuffleCounters(state.RoundNumber);

        if (!room.SeatAssignments.TryGetValue(state.CurrentTurnSeat, out var actorPeerId) || !actorPeerId.HasValue)
        {
            CancelActiveSearch();
            _pendingDecision = null;
            return;
        }

        if (!room.Participants.TryGetValue(actorPeerId.Value, out var actor) || !actor.IsBot)
        {
            CancelActiveSearch();
            _pendingDecision = null;
            return;
        }

        if (_pendingDecision != null && _pendingDecision.StateVersion == coordinator.StateVersion)
        {
            return;
        }

        if (_activeSearchTask != null && !_activeSearchTask.IsCompleted && _activeSearchVersion == coordinator.StateVersion)
        {
            return;
        }

        var perception = BuildPerception(state, actorPeerId.Value, actor.BotDifficulty ?? room.SelectedAiDifficulty, coordinator.StateVersion);
        StartSearch(perception, coordinator.StateVersion);
    }

    private BotPerceptionState BuildPerception(OmiMatchState state, int botPeerId, AiDifficulty difficulty, int stateVersion)
    {
        var botSeat = state.CurrentTurnSeat;
        var publicState = _rulesEngine.CloneState(state);

        var handCounts = new Dictionary<SeatPosition, int>();
        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            handCounts[seat] = state.Hands[seat].Count;
            if (seat != botSeat)
            {
                publicState.Hands[seat].Clear();
            }
        }

        var voidTracker = VoidSuitTracker.BuildFromTricks(state.CompletedTricks, state.CurrentTrickCards);

        return new BotPerceptionState
        {
            PublicState = publicState,
            BotSeat = botSeat,
            BotPeerId = botPeerId,
            Difficulty = difficulty,
            DecisionSeed = BuildDecisionSeed(state, stateVersion, botPeerId),
            ShuffleCountForRound = GetShuffleCount(state.RoundNumber, botSeat),
            RemainingDeckCount = Math.Max(0, state.Deck.Count - state.DeckCursor),
            PublicHandCounts = handCounts,
            VoidTracker = voidTracker
        };
    }

    private void StartSearch(BotPerceptionState perception, int stateVersion)
    {
        CancelActiveSearch();

        _activeSearchVersion = stateVersion;
        _activeSearchCts = new CancellationTokenSource();
        var token = _activeSearchCts.Token;

        _activeSearchTask = Task.Run(() => _policy.ChooseCommand(perception, _rulesEngine, token), token);
    }

    private void PollSearchCompletion()
    {
        if (_activeSearchTask == null || !_activeSearchTask.IsCompleted)
        {
            return;
        }

        var finishedTask = _activeSearchTask;
        var version = _activeSearchVersion;
        var cts = _activeSearchCts;
        _activeSearchTask = null;
        _activeSearchCts = null;

        if (finishedTask.IsCanceled || cts?.IsCancellationRequested == true)
        {
            return;
        }

        if (finishedTask.IsFaulted)
        {
            GD.PrintErr($"AI search failed: {finishedTask.Exception?.GetBaseException().Message}");
            return;
        }

        MatchCommand command;
        try
        {
            command = finishedTask.Result;
        }
        catch (Exception exception)
        {
            GD.PrintErr($"AI search result read failed: {exception.Message}");
            return;
        }

        var state = MatchCoordinator.Instance.GetAuthoritativeState();
        if (state == null)
        {
            return;
        }

        var delay = ResolveDelaySeconds(command);
        _pendingDecision = new PendingDecision(
            StateVersion: version,
            Command: command,
            BotPeerId: command.ActorPeerId,
            BotSeat: command.ActorSeat ?? state.CurrentTurnSeat,
            RoundNumber: state.RoundNumber,
            ExecuteAtUnixSeconds: Time.GetUnixTimeFromSystem() + delay);
    }

    private void TryExecutePendingDecision()
    {
        if (_pendingDecision == null)
        {
            return;
        }

        if (Time.GetUnixTimeFromSystem() < _pendingDecision.ExecuteAtUnixSeconds)
        {
            return;
        }

        var decision = _pendingDecision;
        _pendingDecision = null;
        if (decision == null || !IsDecisionStillValid(decision))
        {
            ScheduleIfNeeded();
            return;
        }

        if (MatchCoordinator.Instance.ServerHandleAiCommand(decision.Command, out var error))
        {
            if (decision.Command.Type == MatchCommandType.ShuffleAgain)
            {
                IncrementShuffleCount(decision.RoundNumber, decision.BotSeat);
            }
            else if (decision.Command.Type == MatchCommandType.FinishShuffle)
            {
                ResetShuffleCount(decision.RoundNumber, decision.BotSeat);
            }
        }
        else
        {
            GD.PrintErr($"AI command rejected: {error}");
        }

        ScheduleIfNeeded();
    }

    private bool IsDecisionStillValid(PendingDecision decision)
    {
        if (!Multiplayer.IsServer())
        {
            return false;
        }

        var room = LobbyManager.Instance.CurrentRoom;
        var state = MatchCoordinator.Instance.GetAuthoritativeState();
        if (room == null || state == null)
        {
            return false;
        }

        if (MatchCoordinator.Instance.StateVersion != decision.StateVersion)
        {
            return false;
        }

        if (state.RoundNumber != decision.RoundNumber)
        {
            return false;
        }

        if (state.CurrentTurnSeat != decision.BotSeat)
        {
            return false;
        }

        if (!room.SeatAssignments.TryGetValue(decision.BotSeat, out var seatPeerId) || seatPeerId != decision.BotPeerId)
        {
            return false;
        }

        if (!room.Participants.TryGetValue(decision.BotPeerId, out var participant) || !participant.IsBot)
        {
            return false;
        }

        return true;
    }

    private void CancelActiveSearch()
    {
        if (_activeSearchCts != null)
        {
            try
            {
                _activeSearchCts.Cancel();
            }
            catch
            {
                // Ignore cancellation races.
            }

            _activeSearchCts.Dispose();
            _activeSearchCts = null;
        }

        _activeSearchTask = null;
        _activeSearchVersion = -1;
    }

    private static int BuildDecisionSeed(OmiMatchState state, int stateVersion, int peerId)
    {
        unchecked
        {
            var seed = 23;
            seed = seed * 31 + state.RoundNumber;
            seed = seed * 31 + state.CompletedTricksCount;
            seed = seed * 31 + (int)state.Phase;
            seed = seed * 31 + (int)state.CurrentTurnSeat;
            seed = seed * 31 + stateVersion;
            seed = seed * 31 + peerId;
            return seed;
        }
    }

    private static double ResolveDelaySeconds(MatchCommand command)
    {
        var baseDelay = command.Type switch
        {
            MatchCommandType.PlayCard => 0.55,
            MatchCommandType.SelectTrump => 0.75,
            MatchCommandType.CutDeck => 0.45,
            MatchCommandType.ShuffleAgain => 0.35,
            _ => 0.3
        };

        return baseDelay;
    }

    private int GetShuffleCount(int roundNumber, SeatPosition seat)
    {
        return _shuffleCounts.TryGetValue(BuildShuffleKey(roundNumber, seat), out var count) ? count : 0;
    }

    private void IncrementShuffleCount(int roundNumber, SeatPosition seat)
    {
        var key = BuildShuffleKey(roundNumber, seat);
        _shuffleCounts.TryGetValue(key, out var count);
        _shuffleCounts[key] = count + 1;
    }

    private void ResetShuffleCount(int roundNumber, SeatPosition seat)
    {
        _shuffleCounts.Remove(BuildShuffleKey(roundNumber, seat));
    }

    private void PruneShuffleCounters(int activeRound)
    {
        var staleKeys = new List<string>();
        foreach (var key in _shuffleCounts.Keys)
        {
            var separator = key.IndexOf(':');
            if (separator <= 0)
            {
                staleKeys.Add(key);
                continue;
            }

            if (!int.TryParse(key[..separator], out var round) || round != activeRound)
            {
                staleKeys.Add(key);
            }
        }

        foreach (var key in staleKeys)
        {
            _shuffleCounts.Remove(key);
        }
    }

    private static string BuildShuffleKey(int roundNumber, SeatPosition seat)
    {
        return $"{roundNumber}:{(int)seat}";
    }
}
