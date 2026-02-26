using NetDex.Core.Enums;
using NetDex.Core.Models;

namespace NetDex.Core.Serialization;

public static class CardModelConversions
{
    public static CardModel FromDictionary(Godot.Collections.Dictionary dict)
    {
        var id = dict.TryGetValue("id", out var idValue) ? idValue.AsString() : string.Empty;
        var suit = dict.TryGetValue("suit", out var suitValue)
            ? (CardSuit)(int)suitValue.AsInt32()
            : CardSuit.Spades;
        var rank = dict.TryGetValue("rank", out var rankValue)
            ? (CardRank)(int)rankValue.AsInt32()
            : CardRank.Seven;
        return new CardModel(id, suit, rank);
    }

    public static Godot.Collections.Dictionary ToDictionary(this CardModel card)
    {
        return new Godot.Collections.Dictionary
        {
            ["id"] = card.Id,
            ["suit"] = (int)card.Suit,
            ["rank"] = (int)card.Rank
        };
    }
}
