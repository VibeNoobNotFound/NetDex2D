using System.Collections.Generic;
using NetDex.Core.Enums;
using NetDex.Core.Models;

namespace NetDex.AI;

public sealed class BotPerceptionState
{
    public OmiMatchState PublicState { get; init; } = new();
    public SeatPosition BotSeat { get; init; } = SeatPosition.Bottom;
    public int BotPeerId { get; init; }
    public AiDifficulty Difficulty { get; init; } = AiDifficulty.Strong;
    public int DecisionSeed { get; init; }
    public int ShuffleCountForRound { get; init; }
    public int RemainingDeckCount { get; init; }
    public Dictionary<SeatPosition, int> PublicHandCounts { get; init; } = new();
    public VoidSuitTracker VoidTracker { get; init; } = new();
}
