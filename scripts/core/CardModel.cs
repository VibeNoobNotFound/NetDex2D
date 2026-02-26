using System;
using System.Collections.Generic;
using System.Linq;

public enum GameType
{
    Omi = 0
}

public enum ParticipantRole
{
    Player = 0,
    Spectator = 1
}

public enum SeatPosition
{
    Bottom = 0,
    Right = 1,
    Top = 2,
    Left = 3
}

public enum CardSuit
{
    Hearts = 0,
    Diamonds = 1,
    Clubs = 2,
    Spades = 3
}

public enum CardRank
{
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14
}

public readonly record struct CardModel(string Id, CardSuit Suit, CardRank Rank)
{
    public int Strength => (int)Rank;
}

public readonly record struct PlayedCard(SeatPosition Seat, CardModel Card);

public static class SeatPositionExtensions
{
    public static SeatPosition Next(this SeatPosition seat)
    {
        return (SeatPosition)(((int)seat + 1) % 4);
    }

    public static SeatPosition Previous(this SeatPosition seat)
    {
        return (SeatPosition)(((int)seat + 3) % 4);
    }

    public static int TeamIndex(this SeatPosition seat)
    {
        return seat is SeatPosition.Bottom or SeatPosition.Top ? 0 : 1;
    }

    public static IReadOnlyList<SeatPosition> OrderedFrom(this SeatPosition start)
    {
        var order = new List<SeatPosition>(4);
        var cursor = start;
        for (var i = 0; i < 4; i++)
        {
            order.Add(cursor);
            cursor = cursor.Next();
        }

        return order;
    }

    public static SeatPosition? Parse(string value)
    {
        if (Enum.TryParse<SeatPosition>(value, true, out var seat))
        {
            return seat;
        }

        return null;
    }
}

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

    public static string ToCompactString(this CardModel card)
    {
        return $"{card.Suit}-{(int)card.Rank}";
    }
}
