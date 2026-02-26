using Godot;

public interface IGameRulesEngine
{
    OmiMatchState CreateInitialMatchState(SeatPosition hostSeat, int initialCredits = 10);
    MatchCommandResult ApplyCommand(OmiMatchState state, MatchCommand command);
    VisibleMatchState GetVisibleStateForPeer(OmiMatchState state, SeatPosition? viewerSeat, ParticipantRole role);
    Godot.Collections.Dictionary SerializeSnapshot(VisibleMatchState visibleState);
}

public static class GameTypeRegistry
{
    private static readonly OmiRulesEngine OmiEngine = new();

    public static IGameRulesEngine Resolve(GameType gameType)
    {
        return gameType switch
        {
            GameType.Omi => OmiEngine,
            _ => OmiEngine
        };
    }
}
