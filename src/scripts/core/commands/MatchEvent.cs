using Godot;

namespace NetDex.Core.Commands;

public sealed class MatchEvent
{
    public string Type { get; init; } = string.Empty;
    public Godot.Collections.Dictionary Payload { get; init; } = new();
}
