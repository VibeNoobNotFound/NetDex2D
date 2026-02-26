namespace NetDex.Lobby;

public enum RoomMatchLifecycle
{
    Lobby = 0,
    InMatch = 1,
    PausedReconnect = 2,
    MatchEnded = 3
}
