using System;
using System.Collections.Generic;
using NetDex.Core.Enums;

namespace NetDex.Core.Models;

public sealed class VisibleMatchState
{
    public OmiMatchState SourceState { get; init; } = new();
    public ParticipantRole ViewerRole { get; init; }
    public SeatPosition? ViewerSeat { get; init; }

    public Dictionary<SeatPosition, List<CardModel>> VisibleHands { get; } = new();

    public VisibleMatchState()
    {
        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            VisibleHands[seat] = new List<CardModel>();
        }
    }
}
