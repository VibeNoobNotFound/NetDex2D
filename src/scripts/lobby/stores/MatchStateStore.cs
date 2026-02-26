namespace NetDex.Lobby.Stores;

public sealed class MatchStateStore : IMatchStateStore
{
    public Godot.Collections.Dictionary Snapshot { get; private set; } = new();

    public void Set(Godot.Collections.Dictionary snapshot)
    {
        Snapshot = snapshot;
    }

    public void Clear()
    {
        Snapshot = new Godot.Collections.Dictionary();
    }
}
