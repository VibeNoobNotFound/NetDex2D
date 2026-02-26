using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class LobbyManager : Node
{
    public static LobbyManager Instance { get; private set; } = null!;

    [Signal]
    public delegate void RoomStateChangedEventHandler();

    [Signal]
    public delegate void MatchSnapshotChangedEventHandler();

    [Signal]
    public delegate void InfoMessageEventHandler(string message);

    public RoomState? CurrentRoom { get; private set; }

    public Godot.Collections.Dictionary LocalMatchSnapshot { get; private set; } = new();

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

            if (!Multiplayer.IsServer())
            {
                return false;
            }

            return CurrentRoom.HostPeerId == Multiplayer.GetUniqueId();
        }
    }

    public ParticipantRole GetLocalRole()
    {
        if (CurrentRoom == null)
        {
            return ParticipantRole.Spectator;
        }

        var peerId = Multiplayer.GetUniqueId();
        if (CurrentRoom.Participants.TryGetValue(peerId, out var participant))
        {
            return participant.Role;
        }

        return ParticipantRole.Spectator;
    }

    public SeatPosition? GetLocalSeat()
    {
        if (CurrentRoom == null)
        {
            return null;
        }

        var peerId = Multiplayer.GetUniqueId();
        return CurrentRoom.Participants.TryGetValue(peerId, out var participant) ? participant.Seat : null;
    }

    public ParticipantInfo? GetParticipant(int peerId)
    {
        if (CurrentRoom == null)
        {
            return null;
        }

        return CurrentRoom.Participants.TryGetValue(peerId, out var participant) ? participant : null;
    }

    public void CreateHostedRoom(string roomName, string hostName, string reconnectToken)
    {
        var sanitizedRoomName = SanitizeRoomName(roomName);
        var sanitizedHostName = SanitizePlayerName(hostName);
        var hostPeerId = Multiplayer.GetUniqueId();

        CurrentRoom = new RoomState
        {
            RoomName = sanitizedRoomName,
            HostName = sanitizedHostName,
            HostPeerId = hostPeerId,
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
            Seat = SeatPosition.Bottom
        };

        CurrentRoom.Participants[hostPeerId] = hostParticipant;
        CurrentRoom.SetSeat(SeatPosition.Bottom, hostPeerId);

        LocalMatchSnapshot = new Godot.Collections.Dictionary();

        EmitSignal(SignalName.RoomStateChanged);
        EmitSignal(SignalName.InfoMessage, $"Room '{sanitizedRoomName}' created");
    }

    public void LeaveRoomLocally(string reason = "Left room")
    {
        CurrentRoom = null;
        LocalMatchSnapshot = new Godot.Collections.Dictionary();
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

        var sanitizedName = SanitizePlayerName(playerName);

        var reconnectCandidate = CurrentRoom.Participants.Values
            .FirstOrDefault(participant =>
                !participant.IsConnected &&
                !string.IsNullOrWhiteSpace(reconnectToken) &&
                participant.ReconnectToken == reconnectToken);

        if (reconnectCandidate != null)
        {
            CurrentRoom.Participants.Remove(reconnectCandidate.PeerId);

            var previousPeerId = reconnectCandidate.PeerId;
            reconnectCandidate.PeerId = peerId;
            reconnectCandidate.Name = sanitizedName;
            reconnectCandidate.IsConnected = true;
            CurrentRoom.Participants[peerId] = reconnectCandidate;

            foreach (var seat in CurrentRoom.SeatAssignments.Keys.ToList())
            {
                if (CurrentRoom.SeatAssignments[seat] == previousPeerId)
                {
                    CurrentRoom.SeatAssignments[seat] = peerId;
                }
            }

            if (MatchCoordinator.Instance.IsMatchRunning)
            {
                MatchCoordinator.Instance.ServerHandlePlayerReconnected(peerId);
            }

            BroadcastRoomState();
            return true;
        }

        if (CurrentRoom.Participants.ContainsKey(peerId))
        {
            var existing = CurrentRoom.Participants[peerId];
            existing.Name = sanitizedName;
            existing.IsConnected = true;
            existing.Role = requestedRole;
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
            Seat = null
        };

        CurrentRoom.Participants[peerId] = participant;

        if (!CurrentRoom.IsMatchRunning && requestedRole == ParticipantRole.Player)
        {
            var firstSeat = CurrentRoom.FirstEmptySeat();
            if (firstSeat.HasValue)
            {
                CurrentRoom.SetSeat(firstSeat.Value, peerId);
            }
            else
            {
                participant.Role = ParticipantRole.Spectator;
            }
        }

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

        if (targetPeerId <= 0)
        {
            CurrentRoom.SetSeat(targetSeat, null);
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

        BroadcastRoomState();
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

            if (!CurrentRoom.Participants.TryGetValue(peerId.Value, out var participant) || !participant.IsConnected)
            {
                error = "All seated players must be connected";
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
        CurrentRoom = RoomState.FromSnapshotDictionary(dict);
        EmitSignal(SignalName.RoomStateChanged);
    }

    public void ApplyRemoteMatchSnapshot(string snapshotJson)
    {
        LocalMatchSnapshot = MatchSnapshotSerializer.ParseJson(snapshotJson);
        EmitSignal(SignalName.MatchSnapshotChanged);
    }

    public string BuildRoomSnapshotJson(bool includeReconnectToken = false)
    {
        if (CurrentRoom == null)
        {
            return "{}";
        }

        return Json.Stringify(CurrentRoom.ToSnapshotDictionary(includeReconnectToken));
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
                Time.GetUnixTimeFromSystem());
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
            Time.GetUnixTimeFromSystem());
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
            if (!participant.IsConnected)
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
            ApplyRemoteMatchSnapshot(MatchCoordinator.Instance.BuildSnapshotForPeer(localPeerId, localParticipant.Role));
        }
    }

    private void ServerRemoveOrDisconnectParticipant(int peerId, bool removeDuringLobby)
    {
        if (CurrentRoom == null || !CurrentRoom.Participants.TryGetValue(peerId, out var participant))
        {
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
