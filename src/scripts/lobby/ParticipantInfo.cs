using NetDex.AI;
using NetDex.Core.Enums;

namespace NetDex.Lobby;

public sealed class ParticipantInfo
{
    public int PeerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; }
    public string ReconnectToken { get; set; } = string.Empty;
    public bool IsConnected { get; set; } = true;
    public bool IsHost { get; set; }
    public SeatPosition? Seat { get; set; }
    public ParticipantKind Kind { get; set; } = ParticipantKind.Human;
    public AiDifficulty? BotDifficulty { get; set; }

    public bool IsBot => Kind == ParticipantKind.Bot;

    public Godot.Collections.Dictionary ToDictionary(bool includeReconnectToken)
    {
        var dict = new Godot.Collections.Dictionary
        {
            ["peerId"] = PeerId,
            ["name"] = Name,
            ["role"] = (int)Role,
            ["isConnected"] = IsConnected,
            ["isHost"] = IsHost,
            ["seat"] = Seat?.ToString() ?? string.Empty,
            ["kind"] = (int)Kind,
            ["botDifficulty"] = BotDifficulty.HasValue ? (int)BotDifficulty.Value : -1
        };

        if (includeReconnectToken)
        {
            dict["reconnectToken"] = ReconnectToken;
        }

        return dict;
    }

    public static ParticipantInfo FromDictionary(Godot.Collections.Dictionary dict)
    {
        return new ParticipantInfo
        {
            PeerId = dict.TryGetValue("peerId", out var peerId) ? peerId.AsInt32() : 0,
            Name = dict.TryGetValue("name", out var name) ? name.AsString() : string.Empty,
            Role = dict.TryGetValue("role", out var role) ? (ParticipantRole)role.AsInt32() : ParticipantRole.Spectator,
            IsConnected = dict.TryGetValue("isConnected", out var connected) && connected.AsBool(),
            IsHost = dict.TryGetValue("isHost", out var host) && host.AsBool(),
            Seat = dict.TryGetValue("seat", out var seatValue) ? NetDex.Core.Enums.SeatPositionExtensions.Parse(seatValue.AsString()) : null,
            ReconnectToken = dict.TryGetValue("reconnectToken", out var token) ? token.AsString() : string.Empty,
            Kind = dict.TryGetValue("kind", out var kind) ? (ParticipantKind)kind.AsInt32() : ParticipantKind.Human,
            BotDifficulty = dict.TryGetValue("botDifficulty", out var difficulty) && difficulty.AsInt32() >= 0
                ? (AiDifficulty)difficulty.AsInt32()
                : null
        };
    }
}
