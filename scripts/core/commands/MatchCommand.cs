using NetDex.Core.Enums;

namespace NetDex.Core.Commands;

public sealed class MatchCommand
{
    public MatchCommandType Type { get; init; }
    public SeatPosition? ActorSeat { get; init; }
    public int ActorPeerId { get; init; }
    public int Seed { get; init; }
    public int CutIndex { get; init; }
    public CardSuit TrumpSuit { get; init; }
    public string CardId { get; init; } = string.Empty;
    public int TeamIndex { get; init; } = -1;

    public static MatchCommand StartRound(int seed) => new()
    {
        Type = MatchCommandType.StartRound,
        Seed = seed
    };

    public static MatchCommand CutDeck(SeatPosition actorSeat, int actorPeerId, int cutIndex) => new()
    {
        Type = MatchCommandType.CutDeck,
        ActorSeat = actorSeat,
        ActorPeerId = actorPeerId,
        CutIndex = cutIndex
    };

    public static MatchCommand SelectTrump(SeatPosition actorSeat, int actorPeerId, CardSuit suit) => new()
    {
        Type = MatchCommandType.SelectTrump,
        ActorSeat = actorSeat,
        ActorPeerId = actorPeerId,
        TrumpSuit = suit
    };

    public static MatchCommand PlayCard(SeatPosition actorSeat, int actorPeerId, string cardId) => new()
    {
        Type = MatchCommandType.PlayCard,
        ActorSeat = actorSeat,
        ActorPeerId = actorPeerId,
        CardId = cardId
    };

    public static MatchCommand StartNextRound(int seed) => new()
    {
        Type = MatchCommandType.StartNextRound,
        Seed = seed
    };

    public static MatchCommand ForfeitTeam(int teamIndex) => new()
    {
        Type = MatchCommandType.ForfeitTeam,
        TeamIndex = teamIndex
    };
}
