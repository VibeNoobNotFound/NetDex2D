namespace NetDex.Core.Commands;

public enum MatchCommandType
{
    StartRound = 0,
    ShuffleAgain = 1,
    FinishShuffle = 2,
    CutDeck = 3,
    SelectTrump = 4,
    PlayCard = 5,
    StartNextRound = 6,
    ForfeitTeam = 7,
    CompleteFirstDeal = 8,
    CompleteSecondDeal = 9,
    ResolveCurrentTrick = 10
}
