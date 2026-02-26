using Godot;
using NetDex.Core.Enums;

namespace NetDex.Lobby;

public readonly record struct RoomAdvertisement(
    string RoomName,
    string HostName,
    string GameType,
    int Port,
    int PlayerCount,
    int SpectatorCount,
    string MatchState,
    string HostAddress,
    double LastSeenUnixSeconds,
    string RoomInstanceId,
    int ProtocolVersion)
{
    public string StableKey => !string.IsNullOrWhiteSpace(RoomInstanceId)
        ? RoomInstanceId
        : $"{HostAddress}:{Port}:{RoomName}";

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
            ["lastSeen"] = LastSeenUnixSeconds,
            ["roomInstanceId"] = RoomInstanceId,
            ["protocolVersion"] = ProtocolVersion
        };
    }

    public static RoomAdvertisement FromDictionary(Godot.Collections.Dictionary dict)
    {
        return new RoomAdvertisement(
            RoomName: dict.TryGetValue("roomName", out var roomName) ? roomName.AsString() : string.Empty,
            HostName: dict.TryGetValue("hostName", out var hostName) ? hostName.AsString() : string.Empty,
            GameType: dict.TryGetValue("gameType", out var gameType) ? gameType.AsString() : nameof(Core.Enums.GameType.Omi),
            Port: dict.TryGetValue("port", out var port) ? port.AsInt32() : 7777,
            PlayerCount: dict.TryGetValue("playerCount", out var playerCount) ? playerCount.AsInt32() : 0,
            SpectatorCount: dict.TryGetValue("spectatorCount", out var spectatorCount) ? spectatorCount.AsInt32() : 0,
            MatchState: dict.TryGetValue("matchState", out var matchState) ? matchState.AsString() : RoomMatchLifecycle.Lobby.ToString(),
            HostAddress: dict.TryGetValue("hostAddress", out var hostAddress) ? hostAddress.AsString() : string.Empty,
            LastSeenUnixSeconds: dict.TryGetValue("lastSeen", out var lastSeen) ? lastSeen.AsDouble() : Time.GetUnixTimeFromSystem(),
            RoomInstanceId: dict.TryGetValue("roomInstanceId", out var roomId) ? roomId.AsString() : string.Empty,
            ProtocolVersion: dict.TryGetValue("protocolVersion", out var protocolVersion) ? protocolVersion.AsInt32() : 1
        );
    }
}
