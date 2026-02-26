using NetDex.Core.Enums;

namespace NetDex.Core.Models;

public readonly record struct CardModel(string Id, CardSuit Suit, CardRank Rank)
{
    public int Strength => (int)Rank;
}
