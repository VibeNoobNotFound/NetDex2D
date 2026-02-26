using System;
using Godot;
using NetDex.Core.Enums;
using NetDex.Core.Models;

namespace NetDex.Core.Serialization;

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
