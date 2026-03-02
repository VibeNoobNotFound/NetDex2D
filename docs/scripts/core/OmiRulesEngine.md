# OmiRulesEngine (core/OmiRulesEngine.cs)

- Source: `scripts/core/OmiRulesEngine.cs`
- Namespace: `NetDex.Core.Rules`
- Purpose: OmiRulesEngine in `core/OmiRulesEngine.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Core.Commands`
  - `NetDex.Core.Config`
  - `NetDex.Core.Enums`
  - `NetDex.Core.Models`
  - `NetDex.Core.Serialization`
  - `System`
  - `System.Collections.Generic`
  - `System.Linq`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `CreateInitialMatchState(SeatPosition hostSeat, int initialCredits = 10)` | `OmiMatchState` | Public entry point |
| `ApplyCommand(OmiMatchState state, MatchCommand command)` | `MatchCommandResult` | Public entry point |
| `GetVisibleStateForPeer(OmiMatchState state, SeatPosition? viewerSeat, ParticipantRole role)` | `VisibleMatchState` | Public entry point |
| `SerializeSnapshot(VisibleMatchState visibleState)` | `Godot.Collections.Dictionary` | Public entry point |
| `EnumerateLegalCommands(OmiMatchState state, SeatPosition actorSeat, int actorPeerId)` | `IReadOnlyList<MatchCommand>` | Public entry point |
| `CloneState(OmiMatchState state)` | `OmiMatchState` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `StartRound(OmiMatchState state, int seed)` | `private` | `MatchCommandResult` |
| `ShuffleAgain(OmiMatchState state, MatchCommand command)` | `private` | `MatchCommandResult` |
| `FinishShuffle(OmiMatchState state, MatchCommand command)` | `private` | `MatchCommandResult` |
| `CutDeck(OmiMatchState state, MatchCommand command)` | `private` | `MatchCommandResult` |
| `CompleteFirstDeal(OmiMatchState state)` | `private` | `MatchCommandResult` |
| `SelectTrump(OmiMatchState state, MatchCommand command)` | `private` | `MatchCommandResult` |
| `CompleteSecondDeal(OmiMatchState state)` | `private` | `MatchCommandResult` |
| `PlayCard(OmiMatchState state, MatchCommand command)` | `private` | `MatchCommandResult` |
| `ResolveCurrentTrick(OmiMatchState state)` | `private` | `MatchCommandResult` |
| `ForfeitTeam(OmiMatchState state, int teamIndex)` | `private` | `MatchCommandResult` |
| `FinalizeRound(OmiMatchState state, MatchCommandResult result)` | `private` | `void` |
| `IsCardPlayableByRule(OmiMatchState state, IReadOnlyList<CardModel> hand, CardModel candidate)` | `private` | `bool` |
| `BuildDeterministicActionSeed(OmiMatchState state, SeatPosition seat, int actionIndex)` | `private` | `int` |
| `DetermineTrickWinner(IReadOnlyList<PlayedCard> trickCards, CardSuit? trumpSuit)` | `private` | `SeatPosition` |
| `DealCardsRoundRobin(OmiMatchState state, SeatPosition startSeat, int cardsPerSeat)` | `private` | `void` |

## Function-by-Function

### `CreateInitialMatchState(SeatPosition hostSeat, int initialCredits = 10)`

- Signature: `public OmiMatchState CreateInitialMatchState(SeatPosition hostSeat, int initialCredits = 10)`
- Source range: `scripts/core/OmiRulesEngine.cs:15`
- Inputs:
  - `SeatPosition hostSeat, int initialCredits = 10`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ApplyCommand(OmiMatchState state, MatchCommand command)`

- Signature: `public MatchCommandResult ApplyCommand(OmiMatchState state, MatchCommand command)`
- Source range: `scripts/core/OmiRulesEngine.cs:28`
- Inputs:
  - `OmiMatchState state, MatchCommand command`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `StartRound(...)`
  - Calls `ShuffleAgain(...)`
  - Calls `FinishShuffle(...)`
  - Calls `CutDeck(...)`
  - Calls `CompleteFirstDeal(...)`
  - Calls `SelectTrump(...)`
  - Calls `CompleteSecondDeal(...)`
  - Calls `PlayCard(...)`
  - Calls `ResolveCurrentTrick(...)`
  - Calls `ForfeitTeam(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetVisibleStateForPeer(OmiMatchState state, SeatPosition? viewerSeat, ParticipantRole role)`

- Signature: `public VisibleMatchState GetVisibleStateForPeer(OmiMatchState state, SeatPosition? viewerSeat, ParticipantRole role)`
- Source range: `scripts/core/OmiRulesEngine.cs:47`
- Inputs:
  - `OmiMatchState state, SeatPosition? viewerSeat, ParticipantRole role`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetValues(...)`
  - Calls `AddRange(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SerializeSnapshot(VisibleMatchState visibleState)`

- Signature: `public Godot.Collections.Dictionary SerializeSnapshot(VisibleMatchState visibleState)`
- Source range: `scripts/core/OmiRulesEngine.cs:67`
- Inputs:
  - `VisibleMatchState visibleState`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Serialize(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `EnumerateLegalCommands(OmiMatchState state, SeatPosition actorSeat, int actorPeerId)`

- Signature: `public IReadOnlyList<MatchCommand> EnumerateLegalCommands(OmiMatchState state, SeatPosition actorSeat, int actorPeerId)`
- Source range: `scripts/core/OmiRulesEngine.cs:72`
- Inputs:
  - `OmiMatchState state, SeatPosition actorSeat, int actorPeerId`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `Add(...)`
  - Calls `FinishShuffle(...)`
  - Calls `ShuffleAgain(...)`
  - Calls `BuildDeterministicActionSeed(...)`
  - Calls `Max(...)`
  - Calls `CutDeck(...)`
  - Calls `GetValues(...)`
  - Calls `SelectTrump(...)`
  - Calls `TryGetValue(...)`
  - Calls `IsCardPlayableByRule(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `CloneState(OmiMatchState state)`

- Signature: `public OmiMatchState CloneState(OmiMatchState state)`
- Source range: `scripts/core/OmiRulesEngine.cs:138`
- Inputs:
  - `OmiMatchState state`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `DeepClone(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `StartRound(OmiMatchState state, int seed)`

- Signature: `private static MatchCommandResult StartRound(OmiMatchState state, int seed)`
- Source range: `scripts/core/OmiRulesEngine.cs:143`
- Inputs:
  - `OmiMatchState state, int seed`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `ResetRoundHandsAndBoard(...)`
  - Calls `Next(...)`
  - Calls `Previous(...)`
  - Calls `Shuffle(...)`
  - Calls `BuildOmiDeck(...)`
  - Calls `Ok(...)`
  - Calls `ToString(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ShuffleAgain(OmiMatchState state, MatchCommand command)`

- Signature: `private static MatchCommandResult ShuffleAgain(OmiMatchState state, MatchCommand command)`
- Source range: `scripts/core/OmiRulesEngine.cs:177`
- Inputs:
  - `OmiMatchState state, MatchCommand command`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `Shuffle(...)`
  - Calls `BuildOmiDeck(...)`
  - Calls `Ok(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `FinishShuffle(OmiMatchState state, MatchCommand command)`

- Signature: `private static MatchCommandResult FinishShuffle(OmiMatchState state, MatchCommand command)`
- Source range: `scripts/core/OmiRulesEngine.cs:203`
- Inputs:
  - `OmiMatchState state, MatchCommand command`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `Ok(...)`
  - Calls `ToString(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `CutDeck(OmiMatchState state, MatchCommand command)`

- Signature: `private static MatchCommandResult CutDeck(OmiMatchState state, MatchCommand command)`
- Source range: `scripts/core/OmiRulesEngine.cs:230`
- Inputs:
  - `OmiMatchState state, MatchCommand command`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `Cut(...)`
  - Calls `DealCardsRoundRobin(...)`
  - Calls `GetUnixTimeFromSystem(...)`
  - Calls `Ok(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `CompleteFirstDeal(OmiMatchState state)`

- Signature: `private static MatchCommandResult CompleteFirstDeal(OmiMatchState state)`
- Source range: `scripts/core/OmiRulesEngine.cs:261`
- Inputs:
  - `OmiMatchState state`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `Ok(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SelectTrump(OmiMatchState state, MatchCommand command)`

- Signature: `private static MatchCommandResult SelectTrump(OmiMatchState state, MatchCommand command)`
- Source range: `scripts/core/OmiRulesEngine.cs:274`
- Inputs:
  - `OmiMatchState state, MatchCommand command`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `DealCardsRoundRobin(...)`
  - Calls `GetUnixTimeFromSystem(...)`
  - Calls `Ok(...)`
  - Calls `ToString(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `CompleteSecondDeal(OmiMatchState state)`

- Signature: `private static MatchCommandResult CompleteSecondDeal(OmiMatchState state)`
- Source range: `scripts/core/OmiRulesEngine.cs:304`
- Inputs:
  - `OmiMatchState state`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `Ok(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PlayCard(OmiMatchState state, MatchCommand command)`

- Signature: `private static MatchCommandResult PlayCard(OmiMatchState state, MatchCommand command)`
- Source range: `scripts/core/OmiRulesEngine.cs:317`
- Inputs:
  - `OmiMatchState state, MatchCommand command`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `FindIndex(...)`
  - Calls `Any(...)`
  - Calls `RemoveAt(...)`
  - Calls `Add(...)`
  - Calls `PlayedCard(...)`
  - Calls `ToString(...)`
  - Calls `ToDictionary(...)`
  - Calls `Next(...)`
  - Calls `GetUnixTimeFromSystem(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ResolveCurrentTrick(OmiMatchState state)`

- Signature: `private static MatchCommandResult ResolveCurrentTrick(OmiMatchState state)`
- Source range: `scripts/core/OmiRulesEngine.cs:380`
- Inputs:
  - `OmiMatchState state`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `DetermineTrickWinner(...)`
  - Calls `TeamIndex(...)`
  - Calls `Add(...)`
  - Calls `ToList(...)`
  - Calls `Clear(...)`
  - Calls `ToString(...)`
  - Calls `FinalizeRound(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ForfeitTeam(OmiMatchState state, int teamIndex)`

- Signature: `private static MatchCommandResult ForfeitTeam(OmiMatchState state, int teamIndex)`
- Source range: `scripts/core/OmiRulesEngine.cs:426`
- Inputs:
  - `OmiMatchState state, int teamIndex`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Fail(...)`
  - Calls `Ok(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `FinalizeRound(OmiMatchState state, MatchCommandResult result)`

- Signature: `private static void FinalizeRound(OmiMatchState state, MatchCommandResult result)`
- Source range: `scripts/core/OmiRulesEngine.cs:451`
- Inputs:
  - `OmiMatchState state, MatchCommandResult result`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Add(...)`
  - Calls `Max(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `IsCardPlayableByRule(OmiMatchState state, IReadOnlyList<CardModel> hand, CardModel candidate)`

- Signature: `private static bool IsCardPlayableByRule(OmiMatchState state, IReadOnlyList<CardModel> hand, CardModel candidate)`
- Source range: `scripts/core/OmiRulesEngine.cs:560`
- Inputs:
  - `OmiMatchState state, IReadOnlyList<CardModel> hand, CardModel candidate`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Any(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildDeterministicActionSeed(OmiMatchState state, SeatPosition seat, int actionIndex)`

- Signature: `private static int BuildDeterministicActionSeed(OmiMatchState state, SeatPosition seat, int actionIndex)`
- Source range: `scripts/core/OmiRulesEngine.cs:572`
- Inputs:
  - `OmiMatchState state, SeatPosition seat, int actionIndex`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `DetermineTrickWinner(IReadOnlyList<PlayedCard> trickCards, CardSuit? trumpSuit)`

- Signature: `private static SeatPosition DetermineTrickWinner(IReadOnlyList<PlayedCard> trickCards, CardSuit? trumpSuit)`
- Source range: `scripts/core/OmiRulesEngine.cs:587`
- Inputs:
  - `IReadOnlyList<PlayedCard> trickCards, CardSuit? trumpSuit`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `InvalidOperationException(...)`
  - Calls `AsEnumerable(...)`
  - Calls `Any(...)`
  - Calls `Where(...)`
  - Calls `OrderByDescending(...)`
  - Calls `First(...)`
- Failure paths:
  - Has exception handling/throw path.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `DealCardsRoundRobin(OmiMatchState state, SeatPosition startSeat, int cardsPerSeat)`

- Signature: `private static void DealCardsRoundRobin(OmiMatchState state, SeatPosition startSeat, int cardsPerSeat)`
- Source range: `scripts/core/OmiRulesEngine.cs:612`
- Inputs:
  - `OmiMatchState state, SeatPosition startSeat, int cardsPerSeat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `OrderedFrom(...)`
  - Calls `InvalidOperationException(...)`
  - Calls `Add(...)`
- Failure paths:
  - Has exception handling/throw path.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
