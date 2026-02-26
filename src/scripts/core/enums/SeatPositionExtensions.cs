using System;
using System.Collections.Generic;

namespace NetDex.Core.Enums;

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
