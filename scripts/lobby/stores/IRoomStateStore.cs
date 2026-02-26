namespace NetDex.Lobby.Stores;

public interface IRoomStateStore
{
    RoomState? Current { get; }
    void Set(RoomState room);
    void Clear();
}
