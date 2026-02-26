using System.Collections.Generic;

namespace NetDex.Core.Commands;

public sealed class MatchCommandResult
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
    public List<MatchEvent> Events { get; } = new();

    public static MatchCommandResult Ok(params MatchEvent[] events)
    {
        var result = new MatchCommandResult { Success = true };
        result.Events.AddRange(events);
        return result;
    }

    public static MatchCommandResult Fail(string error)
    {
        return new MatchCommandResult
        {
            Success = false,
            Error = error
        };
    }
}
