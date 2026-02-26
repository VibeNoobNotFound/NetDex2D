using System.Collections.Generic;
using System.Linq;
using NetDex.Lobby;

namespace NetDex.Networking.Stores;

public sealed class DiscoveryStore : IDiscoveryStore
{
    private readonly Dictionary<string, RoomAdvertisement> _rooms = new();

    public bool Upsert(RoomAdvertisement advertisement)
    {
        var key = advertisement.StableKey;
        if (_rooms.TryGetValue(key, out var existing) && existing.Equals(advertisement))
        {
            return false;
        }

        _rooms[key] = advertisement;
        return true;
    }

    public bool RemoveExpired(double nowUnixSeconds, double expirySeconds)
    {
        var expired = _rooms
            .Where(pair => nowUnixSeconds - pair.Value.LastSeenUnixSeconds > expirySeconds)
            .Select(pair => pair.Key)
            .ToList();

        if (expired.Count == 0)
        {
            return false;
        }

        foreach (var key in expired)
        {
            _rooms.Remove(key);
        }

        return true;
    }

    public IReadOnlyList<RoomAdvertisement> GetAll()
    {
        return _rooms.Values
            .OrderByDescending(room => room.LastSeenUnixSeconds)
            .ToList();
    }

    public string ComputeFingerprint()
    {
        return string.Join('|', _rooms.Values
            .OrderBy(room => room.StableKey)
            .Select(room => $"{room.StableKey}:{room.PlayerCount}:{room.SpectatorCount}:{room.MatchState}:{room.RoomName}:{room.HostName}:{room.ProtocolVersion}"));
    }

    public void Clear()
    {
        _rooms.Clear();
    }
}
