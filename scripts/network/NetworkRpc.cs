using Godot;
using NetDex.Core.Enums;
using NetDex.Lobby;

namespace NetDex.Networking;

public partial class NetworkRpc : Node
{
    public static NetworkRpc Instance { get; private set; } = null!;

    [Signal]
    public delegate void ServerEventReceivedEventHandler(string eventType, string payloadJson);

    [Signal]
    public delegate void ServerMessageEventHandler(string message);

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public void SendJoinRequest(string playerName, ParticipantRole role, string reconnectToken)
    {
        if (Multiplayer.IsServer())
        {
            var peerId = Multiplayer.GetUniqueId();
            LobbyManager.Instance.ServerHandleJoinRequest(peerId, playerName, role, reconnectToken, out _);
            LobbyManager.Instance.BroadcastRoomState();
            return;
        }

        RpcId(1, nameof(RequestJoinRoom), playerName, (int)role, reconnectToken);
    }

    public void SendSeatChangeRequest(SeatPosition targetSeat, int targetPeerId)
    {
        if (Multiplayer.IsServer())
        {
            LobbyManager.Instance.ServerHandleSeatChange(Multiplayer.GetUniqueId(), targetSeat, targetPeerId, out _);
            return;
        }

        RpcId(1, nameof(RequestSeatChange), (int)targetSeat, targetPeerId);
    }

    public void SendStartMatchRequest()
    {
        if (Multiplayer.IsServer())
        {
            MatchCoordinator.Instance.ServerStartMatch(Multiplayer.GetUniqueId(), out _);
            return;
        }

        RpcId(1, nameof(RequestStartMatch));
    }

    public void SendCutDeckRequest(int cutIndex)
    {
        if (Multiplayer.IsServer())
        {
            MatchCoordinator.Instance.ServerHandleCutDeck(Multiplayer.GetUniqueId(), cutIndex, out _);
            return;
        }

        RpcId(1, nameof(RequestCutDeck), cutIndex);
    }

    public void SendSelectTrumpRequest(CardSuit suit)
    {
        if (Multiplayer.IsServer())
        {
            MatchCoordinator.Instance.ServerHandleSelectTrump(Multiplayer.GetUniqueId(), suit, out _);
            return;
        }

        RpcId(1, nameof(RequestSelectTrump), (int)suit);
    }

    public void SendPlayCardRequest(string cardId)
    {
        if (Multiplayer.IsServer())
        {
            MatchCoordinator.Instance.ServerHandlePlayCard(Multiplayer.GetUniqueId(), cardId, out _);
            return;
        }

        RpcId(1, nameof(RequestPlayCard), cardId);
    }

    public void SendLeaveRoomRequest()
    {
        if (Multiplayer.IsServer())
        {
            LobbyManager.Instance.ServerHandleLeaveRequest(Multiplayer.GetUniqueId());
            return;
        }

        RpcId(1, nameof(RequestLeaveRoom));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RequestJoinRoom(string playerName, int requestedRole, string reconnectToken)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        var sender = (int)Multiplayer.GetRemoteSenderId();
        var ok = LobbyManager.Instance.ServerHandleJoinRequest(sender, playerName, (ParticipantRole)requestedRole, reconnectToken, out var error);
        if (!ok)
        {
            RpcId(sender, nameof(PushServerMessage), error);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RequestSeatChange(int targetSeat, int targetPeerId = -1)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        var sender = (int)Multiplayer.GetRemoteSenderId();
        var ok = LobbyManager.Instance.ServerHandleSeatChange(sender, (SeatPosition)targetSeat, targetPeerId, out var error);
        if (!ok)
        {
            RpcId(sender, nameof(PushServerMessage), error);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RequestStartMatch()
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        var sender = (int)Multiplayer.GetRemoteSenderId();
        var ok = MatchCoordinator.Instance.ServerStartMatch(sender, out var error);
        if (!ok)
        {
            RpcId(sender, nameof(PushServerMessage), error);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RequestCutDeck(int cutIndex)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        var sender = (int)Multiplayer.GetRemoteSenderId();
        var ok = MatchCoordinator.Instance.ServerHandleCutDeck(sender, cutIndex, out var error);
        if (!ok)
        {
            RpcId(sender, nameof(PushServerMessage), error);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RequestSelectTrump(int suit)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        var sender = (int)Multiplayer.GetRemoteSenderId();
        var ok = MatchCoordinator.Instance.ServerHandleSelectTrump(sender, (CardSuit)suit, out var error);
        if (!ok)
        {
            RpcId(sender, nameof(PushServerMessage), error);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RequestPlayCard(string cardId)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        var sender = (int)Multiplayer.GetRemoteSenderId();
        var ok = MatchCoordinator.Instance.ServerHandlePlayCard(sender, cardId, out var error);
        if (!ok)
        {
            RpcId(sender, nameof(PushServerMessage), error);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RequestLeaveRoom()
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        var sender = (int)Multiplayer.GetRemoteSenderId();
        LobbyManager.Instance.ServerHandleLeaveRequest(sender);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushRoomSnapshot(string snapshotJson)
    {
        LobbyManager.Instance.ApplyRemoteRoomSnapshot(snapshotJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushSeatMap(string seatSnapshotJson)
    {
        LobbyManager.Instance.ApplyRemoteRoomSnapshot(seatSnapshotJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushMatchStarted(string payloadJson)
    {
        EmitSignal(SignalName.ServerEventReceived, "PushMatchStarted", payloadJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushPrivateHand(string payloadJson)
    {
        EmitSignal(SignalName.ServerEventReceived, "PushPrivateHand", payloadJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushSpectatorHands(string payloadJson)
    {
        EmitSignal(SignalName.ServerEventReceived, "PushSpectatorHands", payloadJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushCardPlayed(string payloadJson)
    {
        EmitSignal(SignalName.ServerEventReceived, "PushCardPlayed", payloadJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushTrickResolved(string payloadJson)
    {
        EmitSignal(SignalName.ServerEventReceived, "PushTrickResolved", payloadJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushRoundResolved(string payloadJson)
    {
        EmitSignal(SignalName.ServerEventReceived, "PushRoundResolved", payloadJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushCreditsUpdated(string payloadJson)
    {
        EmitSignal(SignalName.ServerEventReceived, "PushCreditsUpdated", payloadJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushPausedForReconnect(int disconnectedPeerId, double reconnectDeadlineUnixSeconds)
    {
        var payload = new Godot.Collections.Dictionary
        {
            ["disconnectedPeerId"] = disconnectedPeerId,
            ["reconnectDeadlineUnixSeconds"] = reconnectDeadlineUnixSeconds
        };

        EmitSignal(SignalName.ServerEventReceived, "PushPausedForReconnect", Json.Stringify(payload));
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushMatchEnded(string payloadJson)
    {
        EmitSignal(SignalName.ServerEventReceived, "PushMatchEnded", payloadJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushMatchSnapshot(string snapshotJson)
    {
        LobbyManager.Instance.ApplyRemoteMatchSnapshot(snapshotJson);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PushServerMessage(string message)
    {
        EmitSignal(SignalName.ServerMessage, message);
    }

    public void BroadcastRoomSnapshot(string snapshotJson)
    {
        Rpc(nameof(PushRoomSnapshot), snapshotJson);
        Rpc(nameof(PushSeatMap), snapshotJson);
    }

    public void SendRoomSnapshot(int peerId, string snapshotJson)
    {
        if (peerId == Multiplayer.GetUniqueId())
        {
            LobbyManager.Instance.ApplyRemoteRoomSnapshot(snapshotJson);
            return;
        }

        RpcId(peerId, nameof(PushRoomSnapshot), snapshotJson);
    }

    public void SendMatchSnapshot(int peerId, string snapshotJson)
    {
        if (peerId == Multiplayer.GetUniqueId())
        {
            LobbyManager.Instance.ApplyRemoteMatchSnapshot(snapshotJson);
            return;
        }

        RpcId(peerId, nameof(PushMatchSnapshot), snapshotJson);
    }

    public void SendPrivateHand(int peerId, string payloadJson)
    {
        if (peerId == Multiplayer.GetUniqueId())
        {
            EmitSignal(SignalName.ServerEventReceived, "PushPrivateHand", payloadJson);
            return;
        }

        RpcId(peerId, nameof(PushPrivateHand), payloadJson);
    }

    public void SendSpectatorHands(int peerId, string payloadJson)
    {
        if (peerId == Multiplayer.GetUniqueId())
        {
            EmitSignal(SignalName.ServerEventReceived, "PushSpectatorHands", payloadJson);
            return;
        }

        RpcId(peerId, nameof(PushSpectatorHands), payloadJson);
    }

    public void BroadcastMatchStarted(string payloadJson)
    {
        Rpc(nameof(PushMatchStarted), payloadJson);
    }

    public void BroadcastCardPlayed(string payloadJson)
    {
        Rpc(nameof(PushCardPlayed), payloadJson);
    }

    public void BroadcastTrickResolved(string payloadJson)
    {
        Rpc(nameof(PushTrickResolved), payloadJson);
    }

    public void BroadcastRoundResolved(string payloadJson)
    {
        Rpc(nameof(PushRoundResolved), payloadJson);
    }

    public void BroadcastCreditsUpdated(string payloadJson)
    {
        Rpc(nameof(PushCreditsUpdated), payloadJson);
    }

    public void BroadcastPausedForReconnect(int disconnectedPeerId, double reconnectDeadlineUnixSeconds)
    {
        Rpc(nameof(PushPausedForReconnect), disconnectedPeerId, reconnectDeadlineUnixSeconds);
    }

    public void BroadcastMatchEnded(string payloadJson)
    {
        Rpc(nameof(PushMatchEnded), payloadJson);
    }

    public void BroadcastServerMessage(string message)
    {
        Rpc(nameof(PushServerMessage), message);
    }
}
