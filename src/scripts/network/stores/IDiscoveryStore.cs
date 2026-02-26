using System.Collections.Generic;
using NetDex.Lobby;

namespace NetDex.Networking.Stores;

public interface IDiscoveryStore
{
    bool Upsert(RoomAdvertisement advertisement);
    bool RemoveExpired(double nowUnixSeconds, double expirySeconds);
    IReadOnlyList<RoomAdvertisement> GetAll();
    string ComputeFingerprint();
    void Clear();
}
