using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NetDex.Core.Enums;

namespace NetDex.Lobby;

public sealed class RoomState
{
    public string RoomName { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public int HostPeerId { get; set; } = 1;
    public string RoomInstanceId { get; set; } = Guid.NewGuid().ToString("N");
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
            ["roomInstanceId"] = RoomInstanceId,
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
            RoomInstanceId = dict.TryGetValue("roomInstanceId", out var roomInstanceId)
                ? roomInstanceId.AsString()
                : Guid.NewGuid().ToString("N"),
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
