namespace NetDex.Lobby.Stores;

public interface IMatchStateStore
{
    Godot.Collections.Dictionary Snapshot { get; }
    void Set(Godot.Collections.Dictionary snapshot);
    void Clear();
}
