using System;
using System.Linq;
using Godot;
using NetDex.AI;
using NetDex.Core.Enums;
using NetDex.Core.Serialization;
using NetDex.Lobby.Stores;
using NetDex.Networking;

namespace NetDex.Lobby;

public partial class LobbyManager : Node
{
    private const int BotPeerIdStart = -1000;

    public static LobbyManager Instance { get; private set; } = null!;

    [Signal]
    public delegate void RoomStateChangedEventHandler();

    [Signal]
    public delegate void MatchSnapshotChangedEventHandler();

    [Signal]
    public delegate void InfoMessageEventHandler(string message);

    private readonly IRoomStateStore _roomStore = new RoomStateStore();
    private readonly IMatchStateStore _matchStateStore = new MatchStateStore();

    public RoomState? CurrentRoom => _roomStore.Current;
    public Godot.Collections.Dictionary LocalMatchSnapshot => _matchStateStore.Snapshot;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public bool IsInRoom => CurrentRoom != null;

    public bool IsHostAuthority
    {
        get
        {
            if (CurrentRoom == null)
            {
                return false;
            }

            return Multiplayer.IsServer() && CurrentRoom.HostPeerId == Multiplayer.GetUniqueId();
        }
    }

    public ParticipantRole GetLocalRole()
    {
        if (CurrentRoom == null)
        {
            return ParticipantRole.Spectator;
        }

        var peerId = Multiplayer.GetUniqueId();
        return CurrentRoom.Participants.TryGetValue(peerId, out var participant)
            ? participant.Role
            : ParticipantRole.Spectator;
    }

    public SeatPosition? GetLocalSeat()
    {
        if (CurrentRoom == null)
        {
            return null;
        }

        var peerId = Multiplayer.GetUniqueId();
        return CurrentRoom.Participants.TryGetValue(peerId, out var participant)
            ? participant.Seat
            : null;
    }

    public void CreateHostedRoom(string roomName, string hostName, string reconnectToken)
    {
        var sanitizedRoomName = SanitizeRoomName(roomName);
        var sanitizedHostName = SanitizePlayerName(hostName);
        var hostPeerId = Multiplayer.GetUniqueId();

        var room = new RoomState
        {
            RoomName = sanitizedRoomName,
            HostName = sanitizedHostName,
            HostPeerId = hostPeerId,
            RoomInstanceId = Guid.NewGuid().ToString("N"),
            GameType = GameType.Omi,
            MatchLifecycle = RoomMatchLifecycle.Lobby
        };

        var hostParticipant = new ParticipantInfo
        {
            PeerId = hostPeerId,
            Name = sanitizedHostName,
            Role = ParticipantRole.Player,
            IsConnected = true,
            IsHost = true,
            ReconnectToken = reconnectToken,
            Seat = SeatPosition.Bottom,
            Kind = ParticipantKind.Human,
            BotDifficulty = null
        };

        room.Participants[hostPeerId] = hostParticipant;
        room.SetSeat(SeatPosition.Bottom, hostPeerId);

        _roomStore.Set(room);
        _matchStateStore.Clear();

        EmitSignal(SignalName.RoomStateChanged);
        EmitSignal(SignalName.InfoMessage, $"Room '{sanitizedRoomName}' created");
    }

    public void LeaveRoomLocally(string reason = "Left room")
    {
        _roomStore.Clear();
        _matchStateStore.Clear();
        EmitSignal(SignalName.RoomStateChanged);
        EmitSignal(SignalName.MatchSnapshotChanged);
        EmitSignal(SignalName.InfoMessage, reason);
    }

    public bool ServerHandleJoinRequest(int peerId, string playerName, ParticipantRole requestedRole, string reconnectToken, out string error)
    {
        error = string.Empty;

        if (!IsHostAuthority || CurrentRoom == null)
        {
            error = "Room is not hosted on this peer";
            return false;
        }

        var room = CurrentRoom;
        var sanitizedName = SanitizePlayerName(playerName);

        var reconnectCandidate = room.Participants.Values
            .FirstOrDefault(participant =>
                !participant.IsConnected &&
                !string.IsNullOrWhiteSpace(reconnectToken) &&
                participant.ReconnectToken == reconnectToken);

        if (reconnectCandidate != null)
        {
            room.Participants.Remove(reconnectCandidate.PeerId);

            var previousPeerId = reconnectCandidate.PeerId;
            reconnectCandidate.PeerId = peerId;
            reconnectCandidate.Name = sanitizedName;
            reconnectCandidate.IsConnected = true;
            room.Participants[peerId] = reconnectCandidate;

            foreach (var seat in room.SeatAssignments.Keys.ToList())
            {
                if (room.SeatAssignments[seat] == previousPeerId)
                {
                    room.SeatAssignments[seat] = peerId;
                }
            }

            if (MatchCoordinator.Instance.IsMatchRunning)
            {
                MatchCoordinator.Instance.ServerHandlePlayerReconnected(peerId);
            }

            var reconnectSnapshot = BuildRoomSnapshotJson();
            NetworkRpc.Instance.SendRoomSnapshot(peerId, reconnectSnapshot);
            BroadcastRoomState();
            return true;
        }

        if (room.Participants.ContainsKey(peerId))
        {
            var existing = room.Participants[peerId];
            existing.Name = sanitizedName;
            existing.IsConnected = true;
            existing.Role = requestedRole;
            existing.Kind = ParticipantKind.Human;
            existing.BotDifficulty = null;

            var existingSnapshot = BuildRoomSnapshotJson();
            NetworkRpc.Instance.SendRoomSnapshot(peerId, existingSnapshot);
            BroadcastRoomState();
            return true;
        }

        var participant = new ParticipantInfo
        {
            PeerId = peerId,
            Name = sanitizedName,
            Role = requestedRole,
            ReconnectToken = reconnectToken,
            IsConnected = true,
            IsHost = false,
            Seat = null,
            Kind = ParticipantKind.Human,
            BotDifficulty = null
        };

        room.Participants[peerId] = participant;

        if (!room.IsMatchRunning && requestedRole == ParticipantRole.Player)
        {
            var firstSeat = room.FirstEmptySeat();
            if (firstSeat.HasValue)
            {
                room.SetSeat(firstSeat.Value, peerId);
            }
            else
            {
                participant.Role = ParticipantRole.Spectator;
            }
        }

        RemoveUnseatedLobbyBots();

        var snapshot = BuildRoomSnapshotJson();
        NetworkRpc.Instance.SendRoomSnapshot(peerId, snapshot);
        BroadcastRoomState();
        return true;
    }

    public void ServerHandleLeaveRequest(int peerId)
    {
        if (!IsHostAuthority || CurrentRoom == null)
        {
            return;
        }

        ServerRemoveOrDisconnectParticipant(peerId, removeDuringLobby: true);
        BroadcastRoomState();
    }

    public void ServerHandlePeerDisconnected(int peerId)
    {
        if (CurrentRoom == null)
        {
            return;
        }

        if (!CurrentRoom.Participants.TryGetValue(peerId, out var participant))
        {
            return;
        }

        participant.IsConnected = false;

        if (participant.IsHost)
        {
            LeaveRoomLocally("Host disconnected");
            return;
        }

        if (participant.Seat.HasValue && MatchCoordinator.Instance.IsMatchRunning)
        {
            CurrentRoom.MatchLifecycle = RoomMatchLifecycle.PausedReconnect;
            MatchCoordinator.Instance.ServerPauseForReconnect(peerId, NetworkManager.ReconnectTimeoutSeconds);
        }
        else
        {
            ServerRemoveOrDisconnectParticipant(peerId, removeDuringLobby: true);
        }

        BroadcastRoomState();
    }

    public bool ServerHandleSeatChange(int requesterPeerId, SeatPosition targetSeat, int targetPeerId, out string error)
    {
        error = string.Empty;

        if (!IsHostAuthority || CurrentRoom == null)
        {
            error = "Only host can change seats";
            return false;
        }

        if (requesterPeerId != CurrentRoom.HostPeerId)
        {
            error = "Only host can change seats";
            return false;
        }

        if (CurrentRoom.IsMatchRunning)
        {
            error = "Cannot change seats while match is running";
            return false;
        }

        if (targetPeerId == -1)
        {
            CurrentRoom.SetSeat(targetSeat, null);
            RemoveUnseatedLobbyBots();
            BroadcastRoomState();
            return true;
        }

        if (!CurrentRoom.Participants.TryGetValue(targetPeerId, out var participant) || !participant.IsConnected)
        {
            error = "Target player is not available";
            return false;
        }

        participant.Role = ParticipantRole.Player;
        CurrentRoom.SetSeat(targetSeat, targetPeerId);
        RemoveUnseatedLobbyBots();

        BroadcastRoomState();
        return true;
    }

    public bool ServerSetAiOptions(int requesterPeerId, bool autoFill, AiDifficulty difficulty, out string error)
    {
        error = string.Empty;

        if (!IsHostAuthority || CurrentRoom == null)
        {
            error = "Only host can change AI settings";
            return false;
        }

        if (requesterPeerId != CurrentRoom.HostPeerId)
        {
            error = "Only host can change AI settings";
            return false;
        }

        if (CurrentRoom.IsMatchRunning)
        {
            error = "Cannot change AI settings while match is running";
            return false;
        }

        CurrentRoom.AiAutoFillEnabled = autoFill;
        CurrentRoom.SelectedAiDifficulty = difficulty;

        foreach (var participant in CurrentRoom.Participants.Values.Where(p => p.IsBot))
        {
            participant.BotDifficulty = difficulty;
        }

        BroadcastRoomState();
        return true;
    }

    public bool ServerPreparePlayersForMatch(out string error)
    {
        error = string.Empty;
        if (!IsHostAuthority || CurrentRoom == null)
        {
            error = "Not hosting room";
            return false;
        }

        if (CurrentRoom.IsMatchRunning)
        {
            error = "Match already running";
            return false;
        }

        if (CurrentRoom.AiAutoFillEnabled)
        {
            ServerAutoFillEmptySeatsWithBots();
        }

        return true;
    }

    public bool ServerCanStartMatch(out string error)
    {
        error = string.Empty;

        if (!IsHostAuthority || CurrentRoom == null)
        {
            error = "Not hosting room";
            return false;
        }

        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            if (!CurrentRoom.SeatAssignments.TryGetValue(seat, out var peerId) || !peerId.HasValue)
            {
                error = "All 4 seats must be assigned";
                return false;
            }

            if (!CurrentRoom.Participants.TryGetValue(peerId.Value, out var participant))
            {
                error = "All seated players must be valid";
                return false;
            }

            if (!participant.IsBot && !participant.IsConnected)
            {
                error = "All seated human players must be connected";
                return false;
            }
        }

        return true;
    }

    public void SetMatchLifecycle(RoomMatchLifecycle lifecycle)
    {
        if (CurrentRoom == null)
        {
            return;
        }

        CurrentRoom.MatchLifecycle = lifecycle;
        EmitSignal(SignalName.RoomStateChanged);
    }

    public void ApplyRemoteRoomSnapshot(string snapshotJson)
    {
        var dict = ParseSnapshotJson(snapshotJson);
        _roomStore.Set(RoomState.FromSnapshotDictionary(dict));
        EmitSignal(SignalName.RoomStateChanged);
    }

    public void ApplyRemoteMatchSnapshot(string snapshotJson)
    {
        _matchStateStore.Set(MatchSnapshotSerializer.ParseJson(snapshotJson));
        EmitSignal(SignalName.MatchSnapshotChanged);
    }

    public string BuildRoomSnapshotJson(bool includeReconnectToken = false)
    {
        return CurrentRoom == null
            ? "{}"
            : Json.Stringify(CurrentRoom.ToSnapshotDictionary(includeReconnectToken));
    }

    public RoomAdvertisement BuildAdvertisement(string hostAddress)
    {
        if (CurrentRoom == null)
        {
            return new RoomAdvertisement(
                string.Empty,
                string.Empty,
                nameof(GameType.Omi),
                NetworkManager.GamePort,
                0,
                0,
                RoomMatchLifecycle.Lobby.ToString(),
                hostAddress,
                Time.GetUnixTimeFromSystem(),
                string.Empty,
                NetworkManager.DiscoveryProtocolVersion);
        }

        return new RoomAdvertisement(
            CurrentRoom.RoomName,
            CurrentRoom.HostName,
            CurrentRoom.GameType.ToString(),
            NetworkManager.GamePort,
            CurrentRoom.PlayerCount,
            CurrentRoom.SpectatorCount,
            CurrentRoom.MatchLifecycle.ToString(),
            hostAddress,
            Time.GetUnixTimeFromSystem(),
            CurrentRoom.RoomInstanceId,
            NetworkManager.DiscoveryProtocolVersion);
    }

    public void BroadcastRoomState()
    {
        if (CurrentRoom == null)
        {
            return;
        }

        var snapshot = BuildRoomSnapshotJson();
        NetworkRpc.Instance.BroadcastRoomSnapshot(snapshot);
        EmitSignal(SignalName.RoomStateChanged);
    }

    public void BroadcastMatchSnapshotToAll()
    {
        if (!IsHostAuthority || CurrentRoom == null)
        {
            return;
        }

        var peerIds = CurrentRoom.Participants.Keys.ToList();
        foreach (var peerId in peerIds)
        {
            var participant = CurrentRoom.Participants[peerId];
            if (participant.IsBot || peerId <= 0 || !participant.IsConnected)
            {
                continue;
            }

            var snapshot = MatchCoordinator.Instance.BuildSnapshotForPeer(peerId, participant.Role);
            NetworkRpc.Instance.SendMatchSnapshot(peerId, snapshot);
            if (participant.Role == ParticipantRole.Spectator)
            {
                NetworkRpc.Instance.SendSpectatorHands(peerId, snapshot);
            }
            else
            {
                NetworkRpc.Instance.SendPrivateHand(peerId, snapshot);
            }
        }

        var localPeerId = Multiplayer.GetUniqueId();
        if (CurrentRoom.Participants.TryGetValue(localPeerId, out var localParticipant))
        {
            _matchStateStore.Set(MatchSnapshotSerializer.ParseJson(MatchCoordinator.Instance.BuildSnapshotForPeer(localPeerId, localParticipant.Role)));
            EmitSignal(SignalName.MatchSnapshotChanged);
        }
    }

    private void ServerRemoveOrDisconnectParticipant(int peerId, bool removeDuringLobby)
    {
        if (CurrentRoom == null || !CurrentRoom.Participants.TryGetValue(peerId, out var participant))
        {
            return;
        }

        if (participant.IsBot)
        {
            if (participant.Seat.HasValue)
            {
                CurrentRoom.SetSeat(participant.Seat.Value, null);
            }

            CurrentRoom.Participants.Remove(peerId);
            return;
        }

        if (participant.Seat.HasValue && CurrentRoom.MatchLifecycle == RoomMatchLifecycle.Lobby)
        {
            CurrentRoom.SetSeat(participant.Seat.Value, null);
        }

        if (removeDuringLobby || participant.Role == ParticipantRole.Spectator)
        {
            if (participant.Seat.HasValue)
            {
                CurrentRoom.SetSeat(participant.Seat.Value, null);
            }

            CurrentRoom.Participants.Remove(peerId);
            return;
        }

        participant.IsConnected = false;
    }

    private void ServerAutoFillEmptySeatsWithBots()
    {
        if (CurrentRoom == null)
        {
            return;
        }

        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            if (CurrentRoom.IsSeatTaken(seat))
            {
                continue;
            }

            var botPeerId = AllocateNextBotPeerId();
            var bot = CreateBotParticipantForSeat(botPeerId, seat, CurrentRoom.SelectedAiDifficulty);
            CurrentRoom.Participants[botPeerId] = bot;
            CurrentRoom.SetSeat(seat, botPeerId);
        }

        RemoveUnseatedLobbyBots();
    }

    private int AllocateNextBotPeerId()
    {
        if (CurrentRoom == null)
        {
            return BotPeerIdStart;
        }

        var cursor = BotPeerIdStart;
        while (CurrentRoom.Participants.ContainsKey(cursor))
        {
            cursor -= 1;
        }

        return cursor;
    }

    private ParticipantInfo CreateBotParticipantForSeat(int peerId, SeatPosition seat, AiDifficulty difficulty)
    {
        return new ParticipantInfo
        {
            PeerId = peerId,
            Name = $"Bot-{seat}",
            Role = ParticipantRole.Player,
            IsConnected = true,
            IsHost = false,
            ReconnectToken = string.Empty,
            Seat = seat,
            Kind = ParticipantKind.Bot,
            BotDifficulty = difficulty
        };
    }

    private void RemoveUnseatedLobbyBots()
    {
        if (CurrentRoom == null || CurrentRoom.MatchLifecycle != RoomMatchLifecycle.Lobby)
        {
            return;
        }

        var staleBotIds = CurrentRoom.Participants.Values
            .Where(participant => participant.IsBot && !participant.Seat.HasValue)
            .Select(participant => participant.PeerId)
            .ToList();

        foreach (var botPeerId in staleBotIds)
        {
            CurrentRoom.Participants.Remove(botPeerId);
        }
    }

    private static string SanitizePlayerName(string name)
    {
        var input = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();
        var safe = new string(input.Where(ch => char.IsLetterOrDigit(ch) || ch == ' ' || ch == '_' || ch == '-').ToArray());
        if (string.IsNullOrWhiteSpace(safe))
        {
            safe = "Player";
        }

        return safe.Length > 20 ? safe[..20] : safe;
    }

    private static string SanitizeRoomName(string name)
    {
        var input = string.IsNullOrWhiteSpace(name) ? "Omi Room" : name.Trim();
        var safe = new string(input.Where(ch => char.IsLetterOrDigit(ch) || ch == ' ' || ch == '_' || ch == '-').ToArray());
        if (string.IsNullOrWhiteSpace(safe))
        {
            safe = "Omi Room";
        }

        return safe.Length > 30 ? safe[..30] : safe;
    }

    private static Godot.Collections.Dictionary ParseSnapshotJson(string json)
    {
        var parsed = Json.ParseString(json);
        if (parsed.VariantType == Variant.Type.Dictionary)
        {
            return parsed.AsGodotDictionary();
        }

        return new Godot.Collections.Dictionary();
    }
}
