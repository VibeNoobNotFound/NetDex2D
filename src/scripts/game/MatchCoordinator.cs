using System;
using Godot;
using NetDex.Core.Commands;
using NetDex.Core.Enums;
using NetDex.Core.Models;
using NetDex.Core.Rules;
using NetDex.Networking;

namespace NetDex.Lobby;

public partial class MatchCoordinator : Node
{
    public static MatchCoordinator Instance { get; private set; } = null!;

    private readonly IGameRulesEngine _rulesEngine = GameTypeRegistry.Resolve(GameType.Omi);
    private OmiMatchState? _state;
    private OmiPhase _phaseBeforeReconnectPause = OmiPhase.TrickPlay;
    public int StateVersion { get; private set; }

    [Signal]
    public delegate void MatchStateAdvancedEventHandler();

    [Signal]
    public delegate void MatchInfoEventHandler(string message);

    public bool IsMatchRunning => _state != null && _state.Phase != OmiPhase.MatchEnd;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _Process(double delta)
    {
        if (!Multiplayer.IsServer() || _state == null)
        {
            return;
        }

        if (_state.IsPausedForReconnect)
        {
            if (!_state.ReconnectPeerId.HasValue)
            {
                return;
            }

            if (Time.GetUnixTimeFromSystem() < _state.ReconnectDeadlineUnixSeconds)
            {
                return;
            }

            var room = LobbyManager.Instance.CurrentRoom;
            if (room == null)
            {
                return;
            }

            if (!room.Participants.TryGetValue(_state.ReconnectPeerId.Value, out var participant) || !participant.Seat.HasValue)
            {
                return;
            }

            ApplyServerCommand(MatchCommand.ForfeitTeam(participant.Seat.Value.TeamIndex()));
            return;
        }

        if (_state.PhaseDeadlineUnixSeconds <= 0 || Time.GetUnixTimeFromSystem() < _state.PhaseDeadlineUnixSeconds)
        {
            return;
        }

        switch (_state.Phase)
        {
            case OmiPhase.FirstDeal:
                ApplyServerCommand(MatchCommand.CompleteFirstDeal());
                return;
            case OmiPhase.SecondDeal:
                ApplyServerCommand(MatchCommand.CompleteSecondDeal());
                return;
            case OmiPhase.TrickResolveHold:
                ApplyServerCommand(MatchCommand.ResolveCurrentTrick());
                return;
            case OmiPhase.KapothiProposal:
                ApplyKapothiTimeoutAction(isProposalPhase: true);
                return;
            case OmiPhase.KapothiResponse:
                ApplyKapothiTimeoutAction(isProposalPhase: false);
                return;
        }
    }

    public bool ServerStartMatch(int requesterPeerId, out string error)
    {
        error = string.Empty;

        if (!Multiplayer.IsServer())
        {
            error = "Only server can start match";
            return false;
        }

        var room = LobbyManager.Instance.CurrentRoom;
        if (room == null)
        {
            error = "No room available";
            return false;
        }

        if (requesterPeerId != room.HostPeerId)
        {
            error = "Only host can start match";
            return false;
        }

        if (!LobbyManager.Instance.ServerPreparePlayersForMatch(out error))
        {
            return false;
        }

        if (!LobbyManager.Instance.ServerCanStartMatch(out error))
        {
            return false;
        }

        if (!room.TryGetSeatForPeer(room.HostPeerId, out var hostSeat))
        {
            hostSeat = SeatPosition.Bottom;
        }

        _state = _rulesEngine.CreateInitialMatchState(hostSeat);
        StateVersion += 1;

        var startResult = _rulesEngine.ApplyCommand(_state, MatchCommand.StartRound((int)GD.Randi()));
        if (!startResult.Success)
        {
            error = startResult.Error;
            _state = null;
            return false;
        }

        room.MatchLifecycle = RoomMatchLifecycle.InMatch;

        PushEventNotifications(startResult);
        LobbyManager.Instance.BroadcastRoomState();
        LobbyManager.Instance.BroadcastMatchSnapshotToAll();

        EmitSignal(SignalName.MatchStateAdvanced);
        return true;
    }

    public OmiMatchState? GetAuthoritativeState()
    {
        if (!Multiplayer.IsServer())
        {
            return null;
        }

        return _state;
    }

    public bool ServerHandleCutDeck(int peerId, int cutIndex, out string error)
    {
        if (!TryResolveSeat(peerId, out var seat, out error))
        {
            return false;
        }

        return ApplyServerCommand(MatchCommand.CutDeck(seat, peerId, cutIndex), out error);
    }

    public bool ServerHandleShuffleAgain(int peerId, int seed, out string error)
    {
        if (!TryResolveSeat(peerId, out var seat, out error))
        {
            return false;
        }

        return ApplyServerCommand(MatchCommand.ShuffleAgain(seat, peerId, seed), out error);
    }

    public bool ServerHandleFinishShuffle(int peerId, out string error)
    {
        if (!TryResolveSeat(peerId, out var seat, out error))
        {
            return false;
        }

        return ApplyServerCommand(MatchCommand.FinishShuffle(seat, peerId), out error);
    }

    public bool ServerHandleSelectTrump(int peerId, CardSuit suit, out string error)
    {
        if (!TryResolveSeat(peerId, out var seat, out error))
        {
            return false;
        }

        return ApplyServerCommand(MatchCommand.SelectTrump(seat, peerId, suit), out error);
    }

    public bool ServerHandlePlayCard(int peerId, string cardId, out string error)
    {
        if (!TryResolveSeat(peerId, out var seat, out error))
        {
            return false;
        }

        return ApplyServerCommand(MatchCommand.PlayCard(seat, peerId, cardId), out error);
    }

    public bool ServerHandleKapothiPropose(int peerId, out string error)
    {
        if (!TryResolveSeat(peerId, out var seat, out error))
        {
            return false;
        }

        return ApplyServerCommand(MatchCommand.KapothiPropose(seat, peerId), out error);
    }

    public bool ServerHandleKapothiSkip(int peerId, out string error)
    {
        if (!TryResolveSeat(peerId, out var seat, out error))
        {
            return false;
        }

        return ApplyServerCommand(MatchCommand.KapothiSkip(seat, peerId), out error);
    }

    public bool ServerHandleKapothiAccept(int peerId, out string error)
    {
        if (!TryResolveSeat(peerId, out var seat, out error))
        {
            return false;
        }

        return ApplyServerCommand(MatchCommand.KapothiAccept(seat, peerId), out error);
    }

    public bool ServerHandleKapothiReject(int peerId, out string error)
    {
        if (!TryResolveSeat(peerId, out var seat, out error))
        {
            return false;
        }

        return ApplyServerCommand(MatchCommand.KapothiReject(seat, peerId), out error);
    }

    public bool ServerHandleAiCommand(MatchCommand command, out string error)
    {
        error = string.Empty;

        if (!Multiplayer.IsServer())
        {
            error = "Only server can submit AI command";
            return false;
        }

        if (_state == null)
        {
            error = "Match state is not initialized";
            return false;
        }

        if (!command.ActorSeat.HasValue)
        {
            error = "AI command requires actor seat";
            return false;
        }

        var room = LobbyManager.Instance.CurrentRoom;
        if (room == null)
        {
            error = "No active room";
            return false;
        }

        if (!room.SeatAssignments.TryGetValue(command.ActorSeat.Value, out var peerId) || !peerId.HasValue)
        {
            error = "AI seat is not occupied";
            return false;
        }

        if (!room.Participants.TryGetValue(peerId.Value, out var participant) || !participant.IsBot)
        {
            error = "Seat is not controlled by AI";
            return false;
        }

        return ApplyServerCommand(command, out error);
    }

    public void ServerPauseForReconnect(int peerId, double timeoutSeconds)
    {
        if (_state == null || _state.Phase == OmiPhase.MatchEnd)
        {
            return;
        }

        _phaseBeforeReconnectPause = _state.Phase;
        _state.IsPausedForReconnect = true;
        _state.ReconnectPeerId = peerId;
        _state.ReconnectDeadlineUnixSeconds = Time.GetUnixTimeFromSystem() + timeoutSeconds;
        _state.Phase = OmiPhase.PausedReconnect;
        StateVersion += 1;

        NetworkRpc.Instance.BroadcastPausedForReconnect(peerId, _state.ReconnectDeadlineUnixSeconds);
        LobbyManager.Instance.SetMatchLifecycle(RoomMatchLifecycle.PausedReconnect);
        LobbyManager.Instance.BroadcastRoomState();
        LobbyManager.Instance.BroadcastMatchSnapshotToAll();

        EmitSignal(SignalName.MatchInfo, "Match paused for player reconnect");
        EmitSignal(SignalName.MatchStateAdvanced);
    }

    public void ServerHandlePlayerReconnected(int peerId)
    {
        if (_state == null)
        {
            return;
        }

        if (!_state.IsPausedForReconnect || _state.ReconnectPeerId != peerId)
        {
            return;
        }

        _state.IsPausedForReconnect = false;
        _state.ReconnectPeerId = null;
        _state.ReconnectDeadlineUnixSeconds = 0;
        _state.Phase = _phaseBeforeReconnectPause;
        StateVersion += 1;

        LobbyManager.Instance.SetMatchLifecycle(RoomMatchLifecycle.InMatch);
        LobbyManager.Instance.BroadcastRoomState();
        LobbyManager.Instance.BroadcastMatchSnapshotToAll();

        EmitSignal(SignalName.MatchInfo, "Player reconnected, match resumed");
        EmitSignal(SignalName.MatchStateAdvanced);
    }

    public string BuildSnapshotForPeer(int peerId, ParticipantRole role)
    {
        if (_state == null)
        {
            return "{}";
        }

        SeatPosition? seat = null;
        var room = LobbyManager.Instance.CurrentRoom;
        if (room != null && room.TryGetSeatForPeer(peerId, out var resolvedSeat))
        {
            seat = resolvedSeat;
        }

        return NetDex.Core.Serialization.MatchSnapshotSerializer.SerializeJson(
            _rulesEngine.GetVisibleStateForPeer(_state, seat, role));
    }

    private bool ApplyServerCommand(MatchCommand command)
    {
        return ApplyServerCommand(command, out _);
    }

    private bool ApplyServerCommand(MatchCommand command, out string error)
    {
        error = string.Empty;

        if (_state == null)
        {
            error = "Match state is not initialized";
            return false;
        }

        var result = _rulesEngine.ApplyCommand(_state, command);
        if (!result.Success)
        {
            error = result.Error;
            return false;
        }
        StateVersion += 1;

        PushEventNotifications(result);

        if (_state.Phase == OmiPhase.RoundScore)
        {
            var startNext = _rulesEngine.ApplyCommand(_state, MatchCommand.StartNextRound((int)GD.Randi()));
            if (startNext.Success)
            {
                StateVersion += 1;
                PushEventNotifications(startNext);
            }
        }

        if (_state.Phase == OmiPhase.MatchEnd)
        {
            LobbyManager.Instance.SetMatchLifecycle(RoomMatchLifecycle.MatchEnded);
        }
        else if (_state.Phase == OmiPhase.PausedReconnect)
        {
            LobbyManager.Instance.SetMatchLifecycle(RoomMatchLifecycle.PausedReconnect);
        }
        else
        {
            LobbyManager.Instance.SetMatchLifecycle(RoomMatchLifecycle.InMatch);
        }

        LobbyManager.Instance.BroadcastRoomState();
        LobbyManager.Instance.BroadcastMatchSnapshotToAll();

        EmitSignal(SignalName.MatchStateAdvanced);
        return true;
    }

    private static void PushEventNotifications(MatchCommandResult result)
    {
        foreach (var matchEvent in result.Events)
        {
            var payloadJson = Json.Stringify(matchEvent.Payload);
            switch (matchEvent.Type)
            {
                case "round_started":
                    NetworkRpc.Instance.BroadcastMatchStarted(payloadJson);
                    break;
                case "card_played":
                    NetworkRpc.Instance.BroadcastCardPlayed(payloadJson);
                    break;
                case "trick_resolved":
                    NetworkRpc.Instance.BroadcastTrickResolved(payloadJson);
                    break;
                case "round_resolved":
                    NetworkRpc.Instance.BroadcastRoundResolved(payloadJson);
                    break;
                case "credits_updated":
                    NetworkRpc.Instance.BroadcastCreditsUpdated(payloadJson);
                    break;
                case "match_ended":
                    NetworkRpc.Instance.BroadcastMatchEnded(payloadJson);
                    break;
            }
        }
    }

    private void ApplyKapothiTimeoutAction(bool isProposalPhase)
    {
        if (_state == null)
        {
            return;
        }

        var room = LobbyManager.Instance.CurrentRoom;
        if (room == null)
        {
            return;
        }

        var actingTeam = isProposalPhase ? _state.KapothiEligibleTeam : _state.KapothiTargetTeam;
        if (actingTeam is < 0 or > 1)
        {
            return;
        }

        if (!TryResolveAnySeatForTeam(room, actingTeam, out var seat, out var peerId))
        {
            return;
        }

        var command = isProposalPhase
            ? MatchCommand.KapothiSkip(seat, peerId)
            : MatchCommand.KapothiReject(seat, peerId);

        ApplyServerCommand(command);
    }

    private static bool TryResolveAnySeatForTeam(RoomState room, int teamIndex, out SeatPosition seat, out int peerId)
    {
        seat = SeatPosition.Bottom;
        peerId = 0;

        foreach (SeatPosition candidateSeat in Enum.GetValues(typeof(SeatPosition)))
        {
            if (candidateSeat.TeamIndex() != teamIndex)
            {
                continue;
            }

            if (!room.SeatAssignments.TryGetValue(candidateSeat, out var assignedPeerId) || !assignedPeerId.HasValue)
            {
                continue;
            }

            if (!room.Participants.ContainsKey(assignedPeerId.Value))
            {
                continue;
            }

            seat = candidateSeat;
            peerId = assignedPeerId.Value;
            return true;
        }

        return false;
    }

    private bool TryResolveSeat(int peerId, out SeatPosition seat, out string error)
    {
        seat = SeatPosition.Bottom;
        error = string.Empty;

        var room = LobbyManager.Instance.CurrentRoom;
        if (room == null)
        {
            error = "No active room";
            return false;
        }

        if (!room.TryGetSeatForPeer(peerId, out seat))
        {
            error = "Peer is not seated";
            return false;
        }

        return true;
    }
}
