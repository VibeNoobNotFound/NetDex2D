using System;
using System.Collections.Generic;
using NetDex.Core.Enums;
using NetDex.Core.Models;

namespace NetDex.AI;

public sealed class VoidSuitTracker
{
    private readonly Dictionary<SeatPosition, HashSet<CardSuit>> _voidSuits = new();

    public VoidSuitTracker()
    {
        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            _voidSuits[seat] = new HashSet<CardSuit>();
        }
    }

    public bool CanSeatHoldSuit(SeatPosition seat, CardSuit suit)
    {
        return !_voidSuits.TryGetValue(seat, out var voidSet) || !voidSet.Contains(suit);
    }

    public IReadOnlyCollection<CardSuit> GetVoids(SeatPosition seat)
    {
        return _voidSuits.TryGetValue(seat, out var voidSet)
            ? voidSet
            : Array.Empty<CardSuit>();
    }

    public void MarkVoid(SeatPosition seat, CardSuit suit)
    {
        if (_voidSuits.TryGetValue(seat, out var voidSet))
        {
            voidSet.Add(suit);
        }
    }

    public static VoidSuitTracker BuildFromTricks(IReadOnlyList<List<PlayedCard>> completedTricks, IReadOnlyList<PlayedCard> currentTrick)
    {
        var tracker = new VoidSuitTracker();
        foreach (var trick in completedTricks)
        {
            TrackTrickVoidInfo(tracker, trick);
        }

        if (currentTrick.Count > 0)
        {
            TrackTrickVoidInfo(tracker, currentTrick);
        }

        return tracker;
    }

    private static void TrackTrickVoidInfo(VoidSuitTracker tracker, IReadOnlyList<PlayedCard> trick)
    {
        if (trick.Count < 2)
        {
            return;
        }

        var leadSuit = trick[0].Card.Suit;
        for (var i = 1; i < trick.Count; i++)
        {
            if (trick[i].Card.Suit != leadSuit)
            {
                tracker.MarkVoid(trick[i].Seat, leadSuit);
            }
        }
    }
}
