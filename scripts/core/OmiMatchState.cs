using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public enum OmiPhase
{
    LobbySeating = 0,
    Shuffle = 1,
    Cut = 2,
    FirstDeal = 3,
    TrumpSelect = 4,
    SecondDeal = 5,
    TrickPlay = 6,
    RoundScore = 7,
    MatchScore = 8,
    MatchEnd = 9,
    PausedReconnect = 10
}

public enum MatchCommandType
{
    StartRound = 0,
    CutDeck = 1,
    SelectTrump = 2,
    PlayCard = 3,
    StartNextRound = 4,
    ForfeitTeam = 5
}

public sealed class MatchCommand
{
    public MatchCommandType Type { get; init; }
    public SeatPosition? ActorSeat { get; init; }
    public int ActorPeerId { get; init; }
    public int Seed { get; init; }
    public int CutIndex { get; init; }
    public CardSuit TrumpSuit { get; init; }
    public string CardId { get; init; } = string.Empty;
    public int TeamIndex { get; init; } = -1;

    public static MatchCommand StartRound(int seed) => new()
    {
        Type = MatchCommandType.StartRound,
        Seed = seed
    };

    public static MatchCommand CutDeck(SeatPosition actorSeat, int actorPeerId, int cutIndex) => new()
    {
        Type = MatchCommandType.CutDeck,
        ActorSeat = actorSeat,
        ActorPeerId = actorPeerId,
        CutIndex = cutIndex
    };

    public static MatchCommand SelectTrump(SeatPosition actorSeat, int actorPeerId, CardSuit suit) => new()
    {
        Type = MatchCommandType.SelectTrump,
        ActorSeat = actorSeat,
        ActorPeerId = actorPeerId,
        TrumpSuit = suit
    };

    public static MatchCommand PlayCard(SeatPosition actorSeat, int actorPeerId, string cardId) => new()
    {
        Type = MatchCommandType.PlayCard,
        ActorSeat = actorSeat,
        ActorPeerId = actorPeerId,
        CardId = cardId
    };

    public static MatchCommand StartNextRound(int seed) => new()
    {
        Type = MatchCommandType.StartNextRound,
        Seed = seed
    };

    public static MatchCommand ForfeitTeam(int teamIndex) => new()
    {
        Type = MatchCommandType.ForfeitTeam,
        TeamIndex = teamIndex
    };
}

public sealed class MatchEvent
{
    public string Type { get; init; } = string.Empty;
    public Godot.Collections.Dictionary Payload { get; init; } = new();
}

public sealed class MatchCommandResult
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
    public List<MatchEvent> Events { get; } = new();

    public static MatchCommandResult Ok(params MatchEvent[] events)
    {
        var result = new MatchCommandResult { Success = true };
        result.Events.AddRange(events);
        return result;
    }

    public static MatchCommandResult Fail(string error)
    {
        return new MatchCommandResult
        {
            Success = false,
            Error = error
        };
    }
}

public sealed class OmiMatchState
{
    public GameType GameType { get; set; } = GameType.Omi;
    public OmiPhase Phase { get; set; } = OmiPhase.LobbySeating;
    public SeatPosition HostSeat { get; set; } = SeatPosition.Bottom;

    public int RoundNumber { get; set; } = 0;
    public int CompletedTricksCount { get; set; } = 0;
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
    public int DeckCursor { get; set; } = 0;

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

public static class MatchSnapshotSerializer
{
    public static Godot.Collections.Dictionary Serialize(VisibleMatchState view)
    {
        var state = view.SourceState;
        var handCounts = new Godot.Collections.Dictionary();
        var visibleHands = new Godot.Collections.Dictionary();

        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            handCounts[seat.ToString()] = state.Hands.TryGetValue(seat, out var hand) ? hand.Count : 0;
            var cards = new Godot.Collections.Array();
            foreach (var card in view.VisibleHands[seat])
            {
                cards.Add(card.ToDictionary());
            }

            visibleHands[seat.ToString()] = cards;
        }

        var currentTrick = new Godot.Collections.Array();
        foreach (var played in state.CurrentTrickCards)
        {
            currentTrick.Add(new Godot.Collections.Dictionary
            {
                ["seat"] = played.Seat.ToString(),
                ["card"] = played.Card.ToDictionary()
            });
        }

        return new Godot.Collections.Dictionary
        {
            ["phase"] = (int)state.Phase,
            ["roundNumber"] = state.RoundNumber,
            ["completedTricksCount"] = state.CompletedTricksCount,
            ["trumpSuit"] = state.TrumpSuit.HasValue ? (int)state.TrumpSuit.Value : -1,
            ["shufflerSeat"] = state.ShufflerSeat.ToString(),
            ["cutterSeat"] = state.CutterSeat.ToString(),
            ["trumpSelectorSeat"] = state.TrumpSelectorSeat.ToString(),
            ["currentTurnSeat"] = state.CurrentTurnSeat.ToString(),
            ["teamCredits"] = new Godot.Collections.Array { state.TeamCredits[0], state.TeamCredits[1] },
            ["teamTricks"] = new Godot.Collections.Array { state.TeamTricks[0], state.TeamTricks[1] },
            ["currentStake"] = state.CurrentStake,
            ["roundWinnerTeam"] = state.RoundWinnerTeam ?? -1,
            ["matchWinnerTeam"] = state.MatchWinnerTeam ?? -1,
            ["isPausedForReconnect"] = state.IsPausedForReconnect,
            ["reconnectPeerId"] = state.ReconnectPeerId ?? -1,
            ["reconnectDeadlineUnixSeconds"] = state.ReconnectDeadlineUnixSeconds,
            ["handCounts"] = handCounts,
            ["visibleHands"] = visibleHands,
            ["currentTrick"] = currentTrick,
            ["viewerRole"] = (int)view.ViewerRole,
            ["viewerSeat"] = view.ViewerSeat?.ToString() ?? string.Empty
        };
    }

    public static string SerializeJson(VisibleMatchState view)
    {
        return Json.Stringify(Serialize(view));
    }

    public static Godot.Collections.Dictionary ParseJson(string json)
    {
        var parsed = Json.ParseString(json);
        if (parsed.VariantType == Variant.Type.Dictionary)
        {
            return parsed.AsGodotDictionary();
        }

        return new Godot.Collections.Dictionary();
    }
}
