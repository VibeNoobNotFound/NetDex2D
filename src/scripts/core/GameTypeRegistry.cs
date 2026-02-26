using NetDex.Core.Enums;

namespace NetDex.Core.Rules;

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
