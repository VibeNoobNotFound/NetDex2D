using System;
using System.Collections.Generic;
using NetDex.Core.Enums;

namespace NetDex.Core.Models;

public sealed class OmiMatchState
{
    public GameType GameType { get; set; } = GameType.Omi;
    public OmiPhase Phase { get; set; } = OmiPhase.LobbySeating;
    public SeatPosition HostSeat { get; set; } = SeatPosition.Bottom;

    public int RoundNumber { get; set; }
    public int CompletedTricksCount { get; set; }
    public CardSuit? TrumpSuit { get; set; }

    public SeatPosition ShufflerSeat { get; set; } = SeatPosition.Bottom;
    public SeatPosition CutterSeat { get; set; } = SeatPosition.Left;
    public SeatPosition TrumpSelectorSeat { get; set; } = SeatPosition.Right;
    public SeatPosition CurrentTurnSeat { get; set; } = SeatPosition.Bottom;

    public int CurrentStake { get; set; } = 1;

    public int[] TeamCredits { get; set; } = new[] { 10, 10 };
    public int[] TeamTricks { get; set; } = new[] { 0, 0 };

    public Dictionary<SeatPosition, List<CardModel>> Hands { get; } = new();

    public List<CardModel> Deck { get; set; } = new();
    public int DeckCursor { get; set; }

    public List<PlayedCard> CurrentTrickCards { get; } = new();
    public List<List<PlayedCard>> CompletedTricks { get; } = new();

    public int? RoundWinnerTeam { get; set; }
    public int? MatchWinnerTeam { get; set; }

    public bool IsPausedForReconnect { get; set; }
    public int? ReconnectPeerId { get; set; }
    public double ReconnectDeadlineUnixSeconds { get; set; }

    public OmiMatchState()
    {
        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            Hands[seat] = new List<CardModel>(8);
        }
    }

    public void ResetRoundHandsAndBoard()
    {
        foreach (var hand in Hands.Values)
        {
            hand.Clear();
        }

        Deck.Clear();
        DeckCursor = 0;
        CurrentTrickCards.Clear();
        CompletedTricks.Clear();
        TeamTricks[0] = 0;
        TeamTricks[1] = 0;
        CompletedTricksCount = 0;
        RoundWinnerTeam = null;
        TrumpSuit = null;
    }
}
