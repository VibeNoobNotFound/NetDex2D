using NetDex.Core.Enums;

namespace NetDex.Core.Models;

public readonly record struct PlayedCard(SeatPosition Seat, CardModel Card);
