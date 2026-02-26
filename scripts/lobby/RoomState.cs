using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public enum RoomMatchLifecycle
{
    Lobby = 0,
    InMatch = 1,
    PausedReconnect = 2,
    MatchEnded = 3
}

public sealed class ParticipantInfo
{
    public int PeerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; }
    public string ReconnectToken { get; set; } = string.Empty;
    public bool IsConnected { get; set; } = true;
    public bool IsHost { get; set; }
    public SeatPosition? Seat { get; set; }

    public Godot.Collections.Dictionary ToDictionary(bool includeReconnectToken)
    {
        var dict = new Godot.Collections.Dictionary
        {
            ["peerId"] = PeerId,
            ["name"] = Name,
            ["role"] = (int)Role,
            ["isConnected"] = IsConnected,
            ["isHost"] = IsHost,
            ["seat"] = Seat?.ToString() ?? string.Empty
        };

        if (includeReconnectToken)
        {
            dict["reconnectToken"] = ReconnectToken;
        }

        return dict;
    }

    public static ParticipantInfo FromDictionary(Godot.Collections.Dictionary dict)
    {
        return new ParticipantInfo
        {
            PeerId = dict.TryGetValue("peerId", out var peerId) ? peerId.AsInt32() : 0,
            Name = dict.TryGetValue("name", out var name) ? name.AsString() : string.Empty,
            Role = dict.TryGetValue("role", out var role) ? (ParticipantRole)role.AsInt32() : ParticipantRole.Spectator,
            IsConnected = dict.TryGetValue("isConnected", out var connected) && connected.AsBool(),
            IsHost = dict.TryGetValue("isHost", out var host) && host.AsBool(),
            Seat = dict.TryGetValue("seat", out var seatValue) ? SeatPositionExtensions.Parse(seatValue.AsString()) : null,
            ReconnectToken = dict.TryGetValue("reconnectToken", out var token) ? token.AsString() : string.Empty
        };
    }
}

public readonly record struct RoomAdvertisement(
    string RoomName,
    string HostName,
    string GameType,
    int Port,
    int PlayerCount,
    int SpectatorCount,
    string MatchState,
    string HostAddress,
    double LastSeenUnixSeconds
)
{
    public Godot.Collections.Dictionary ToDictionary()
    {
        return new Godot.Collections.Dictionary
        {
            ["roomName"] = RoomName,
            ["hostName"] = HostName,
            ["gameType"] = GameType,
            ["port"] = Port,
            ["playerCount"] = PlayerCount,
            ["spectatorCount"] = SpectatorCount,
            ["matchState"] = MatchState,
            ["hostAddress"] = HostAddress,
            ["lastSeen"] = LastSeenUnixSeconds
        };
    }

    public static RoomAdvertisement FromDictionary(Godot.Collections.Dictionary dict)
    {
        return new RoomAdvertisement(
            RoomName: dict.TryGetValue("roomName", out var roomName) ? roomName.AsString() : string.Empty,
            HostName: dict.TryGetValue("hostName", out var hostName) ? hostName.AsString() : string.Empty,
            GameType: dict.TryGetValue("gameType", out var gameType) ? gameType.AsString() : nameof(global::GameType.Omi),
            Port: dict.TryGetValue("port", out var port) ? port.AsInt32() : NetworkManager.GamePort,
            PlayerCount: dict.TryGetValue("playerCount", out var playerCount) ? playerCount.AsInt32() : 0,
            SpectatorCount: dict.TryGetValue("spectatorCount", out var spectatorCount) ? spectatorCount.AsInt32() : 0,
            MatchState: dict.TryGetValue("matchState", out var matchState) ? matchState.AsString() : RoomMatchLifecycle.Lobby.ToString(),
            HostAddress: dict.TryGetValue("hostAddress", out var hostAddress) ? hostAddress.AsString() : string.Empty,
            LastSeenUnixSeconds: dict.TryGetValue("lastSeen", out var lastSeen) ? lastSeen.AsDouble() : Time.GetUnixTimeFromSystem()
        );
    }
}

public sealed class RoomState
{
    public string RoomName { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public int HostPeerId { get; set; } = 1;
    public GameType GameType { get; set; } = GameType.Omi;
    public RoomMatchLifecycle MatchLifecycle { get; set; } = RoomMatchLifecycle.Lobby;

    public Dictionary<int, ParticipantInfo> Participants { get; } = new();
    public Dictionary<SeatPosition, int?> SeatAssignments { get; } = new()
    {
        [SeatPosition.Bottom] = null,
        [SeatPosition.Right] = null,
        [SeatPosition.Top] = null,
        [SeatPosition.Left] = null
    };

    public bool IsMatchRunning => MatchLifecycle is RoomMatchLifecycle.InMatch or RoomMatchLifecycle.PausedReconnect;

    public int PlayerCount => Participants.Values.Count(p => p.Role == ParticipantRole.Player);
    public int SpectatorCount => Participants.Values.Count(p => p.Role == ParticipantRole.Spectator);

    public int ConnectedPlayerCount => Participants.Values.Count(p => p.Role == ParticipantRole.Player && p.IsConnected);

    public bool TryGetSeatForPeer(int peerId, out SeatPosition seat)
    {
        foreach (var pair in SeatAssignments)
        {
            if (pair.Value == peerId)
            {
                seat = pair.Key;
                return true;
            }
        }

        seat = SeatPosition.Bottom;
        return false;
    }

    public bool IsSeatTaken(SeatPosition seat)
    {
        return SeatAssignments.TryGetValue(seat, out var occupant) && occupant.HasValue;
    }

    public SeatPosition? FirstEmptySeat()
    {
        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            if (!IsSeatTaken(seat))
            {
                return seat;
            }
        }

        return null;
    }

    public void SetSeat(SeatPosition seat, int? peerId)
    {
        if (SeatAssignments.TryGetValue(seat, out var previousPeerId) && previousPeerId.HasValue && Participants.TryGetValue(previousPeerId.Value, out var previousParticipant))
        {
            previousParticipant.Seat = null;
        }

        if (peerId.HasValue)
        {
            foreach (var key in SeatAssignments.Keys.ToList())
            {
                if (SeatAssignments[key] == peerId)
                {
                    SeatAssignments[key] = null;
                }
            }

            if (Participants.TryGetValue(peerId.Value, out var participant))
            {
                participant.Seat = seat;
                participant.Role = ParticipantRole.Player;
            }
        }

        SeatAssignments[seat] = peerId;
    }

    public int[] GetTeamPeerIds(int teamIndex)
    {
        if (teamIndex == 0)
        {
            return new[]
            {
                SeatAssignments[SeatPosition.Bottom] ?? -1,
                SeatAssignments[SeatPosition.Top] ?? -1
            };
        }

        return new[]
        {
            SeatAssignments[SeatPosition.Right] ?? -1,
            SeatAssignments[SeatPosition.Left] ?? -1
        };
    }

    public Godot.Collections.Dictionary ToSnapshotDictionary(bool includeReconnectToken = false)
    {
        var participants = new Godot.Collections.Array();
        foreach (var participant in Participants.Values.OrderBy(p => p.PeerId))
        {
            participants.Add(participant.ToDictionary(includeReconnectToken));
        }

        var seats = new Godot.Collections.Dictionary();
        foreach (var pair in SeatAssignments)
        {
            seats[pair.Key.ToString()] = pair.Value ?? -1;
        }

        return new Godot.Collections.Dictionary
        {
            ["roomName"] = RoomName,
            ["hostName"] = HostName,
            ["hostPeerId"] = HostPeerId,
            ["gameType"] = (int)GameType,
            ["matchLifecycle"] = (int)MatchLifecycle,
            ["participants"] = participants,
            ["seats"] = seats,
            ["playerCount"] = PlayerCount,
            ["spectatorCount"] = SpectatorCount,
            ["connectedPlayerCount"] = ConnectedPlayerCount
        };
    }

    public static RoomState FromSnapshotDictionary(Godot.Collections.Dictionary dict)
    {
        var state = new RoomState
        {
            RoomName = dict.TryGetValue("roomName", out var roomName) ? roomName.AsString() : string.Empty,
            HostName = dict.TryGetValue("hostName", out var hostName) ? hostName.AsString() : string.Empty,
            HostPeerId = dict.TryGetValue("hostPeerId", out var hostPeerId) ? hostPeerId.AsInt32() : 1,
            GameType = dict.TryGetValue("gameType", out var gameType) ? (GameType)gameType.AsInt32() : GameType.Omi,
            MatchLifecycle = dict.TryGetValue("matchLifecycle", out var lifecycle)
                ? (RoomMatchLifecycle)lifecycle.AsInt32()
                : RoomMatchLifecycle.Lobby
        };

        if (dict.TryGetValue("participants", out var participantsVariant) && participantsVariant.VariantType == Variant.Type.Array)
        {
            foreach (var item in participantsVariant.AsGodotArray())
            {
                if (item.VariantType != Variant.Type.Dictionary)
                {
                    continue;
                }

                var participant = ParticipantInfo.FromDictionary(item.AsGodotDictionary());
                state.Participants[participant.PeerId] = participant;
            }
        }

        if (dict.TryGetValue("seats", out var seatsVariant) && seatsVariant.VariantType == Variant.Type.Dictionary)
        {
            var seatsDict = seatsVariant.AsGodotDictionary();
            foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
            {
                var key = seat.ToString();
                if (!seatsDict.TryGetValue(key, out var peerIdVariant))
                {
                    continue;
                }

                var peerId = peerIdVariant.AsInt32();
                state.SeatAssignments[seat] = peerId > 0 ? peerId : null;
                if (peerId > 0 && state.Participants.TryGetValue(peerId, out var participant))
                {
                    participant.Seat = seat;
                    participant.Role = ParticipantRole.Player;
                }
            }
        }

        return state;
    }
}
