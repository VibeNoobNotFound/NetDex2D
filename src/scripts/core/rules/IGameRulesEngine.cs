using System.Collections.Generic;
using NetDex.Core.Commands;
using NetDex.Core.Enums;
using NetDex.Core.Models;

namespace NetDex.Core.Rules;

public interface IGameRulesEngine
{
    OmiMatchState CreateInitialMatchState(SeatPosition hostSeat, int initialCredits = 10);
    MatchCommandResult ApplyCommand(OmiMatchState state, MatchCommand command);
    IReadOnlyList<MatchCommand> EnumerateLegalCommands(OmiMatchState state, SeatPosition actorSeat, int actorPeerId);
    OmiMatchState CloneState(OmiMatchState state);
    VisibleMatchState GetVisibleStateForPeer(OmiMatchState state, SeatPosition? viewerSeat, ParticipantRole role);
    Godot.Collections.Dictionary SerializeSnapshot(VisibleMatchState visibleState);
}
