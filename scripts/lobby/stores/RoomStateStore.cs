namespace NetDex.Lobby.Stores;

public sealed class RoomStateStore : IRoomStateStore
{
    public RoomState? Current { get; private set; }

    public void Set(RoomState room)
    {
        Current = room;
    }

    public void Clear()
    {
        Current = null;
    }
}
