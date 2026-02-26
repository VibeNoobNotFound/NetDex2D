using System;
using System.Collections.Generic;
using System.Linq;
using NetDex.Core.Enums;
using NetDex.Core.Models;

namespace NetDex.Core.Rules;

public static class DeckService
{
    public static List<CardModel> BuildOmiDeck()
    {
        var deck = new List<CardModel>(32);

        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            for (var rank = (int)CardRank.Seven; rank <= (int)CardRank.Ace; rank++)
            {
                deck.Add(new CardModel($"{suit}-{rank}", suit, (CardRank)rank));
            }
        }

        return deck;
    }

    public static List<CardModel> Shuffle(IReadOnlyList<CardModel> source, int seed)
    {
        var shuffled = source.ToList();
        var random = new Random(seed);

        for (var i = shuffled.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        return shuffled;
    }

    public static List<CardModel> Cut(IReadOnlyList<CardModel> source, int cutIndex)
    {
        if (source.Count == 0)
        {
            return new List<CardModel>();
        }

        var normalizedCut = Math.Clamp(cutIndex, 0, source.Count - 1);
        var top = source.Take(normalizedCut).ToList();
        var bottom = source.Skip(normalizedCut).ToList();
        bottom.AddRange(top);
        return bottom;
    }
}
