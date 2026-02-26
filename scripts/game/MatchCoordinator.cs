using Godot;
using NetDex.Core.Commands;
using NetDex.Core.Enums;
using NetDex.Core.Rules;
using NetDex.Networking;

namespace NetDex.Lobby;

public partial class MatchCoordinator : Node
{
    public static MatchCoordinator Instance { get; private set; } = null!;

    private readonly IGameRulesEngine _rulesEngine = GameTypeRegistry.Resolve(GameType.Omi);
    private NetDex.Core.Models.OmiMatchState? _state;
    private OmiPhase _phaseBeforeReconnectPause = OmiPhase.TrickPlay;

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

        if (!_state.IsPausedForReconnect || !_state.ReconnectPeerId.HasValue)
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

        if (!LobbyManager.Instance.ServerCanStartMatch(out error))
        {
            return false;
        }

        if (!room.TryGetSeatForPeer(room.HostPeerId, out var hostSeat))
        {
            hostSeat = SeatPosition.Bottom;
        }

        _state = _rulesEngine.CreateInitialMatchState(hostSeat);

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

        NetworkRpc.Instance.BroadcastPausedForReconnect(peerId, _state.ReconnectDeadlineUnixSeconds);
        LobbyManager.Instance.SetMatchLifecycle(RoomMatchLifecycle.PausedReconnect);
        LobbyManager.Instance.BroadcastRoomState();
        LobbyManager.Instance.BroadcastMatchSnapshotToAll();

        EmitSignal(SignalName.MatchInfo, "Match paused for player reconnect");
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

        LobbyManager.Instance.SetMatchLifecycle(RoomMatchLifecycle.InMatch);
        LobbyManager.Instance.BroadcastRoomState();
        LobbyManager.Instance.BroadcastMatchSnapshotToAll();

        EmitSignal(SignalName.MatchInfo, "Player reconnected, match resumed");
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

        PushEventNotifications(result);

        if (_state.Phase == OmiPhase.RoundScore)
        {
            var startNext = _rulesEngine.ApplyCommand(_state, MatchCommand.StartNextRound((int)GD.Randi()));
            if (startNext.Success)
            {
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
