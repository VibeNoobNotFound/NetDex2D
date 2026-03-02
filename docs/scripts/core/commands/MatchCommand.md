# MatchCommand (core/commands/MatchCommand.cs)

- Source: `scripts/core/commands/MatchCommand.cs`
- Namespace: `NetDex.Core.Commands`
- Purpose: MatchCommand in `core/commands/MatchCommand.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `NetDex.Core.Enums`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `StartRound(int seed)` | `MatchCommand` | Public entry point |
| `ShuffleAgain(SeatPosition actorSeat, int actorPeerId, int seed)` | `MatchCommand` | Public entry point |
| `FinishShuffle(SeatPosition actorSeat, int actorPeerId)` | `MatchCommand` | Public entry point |
| `CutDeck(SeatPosition actorSeat, int actorPeerId, int cutIndex)` | `MatchCommand` | Public entry point |
| `SelectTrump(SeatPosition actorSeat, int actorPeerId, CardSuit suit)` | `MatchCommand` | Public entry point |
| `PlayCard(SeatPosition actorSeat, int actorPeerId, string cardId)` | `MatchCommand` | Public entry point |
| `StartNextRound(int seed)` | `MatchCommand` | Public entry point |
| `ForfeitTeam(int teamIndex)` | `MatchCommand` | Public entry point |
| `CompleteFirstDeal()` | `MatchCommand` | Public entry point |
| `CompleteSecondDeal()` | `MatchCommand` | Public entry point |
| `ResolveCurrentTrick()` | `MatchCommand` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `StartRound(int seed)`

- Signature: `public static MatchCommand StartRound(int seed)`
- Source range: `scripts/core/commands/MatchCommand.cs:16`
- Inputs:
  - `int seed`
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

### `ShuffleAgain(SeatPosition actorSeat, int actorPeerId, int seed)`

- Signature: `public static MatchCommand ShuffleAgain(SeatPosition actorSeat, int actorPeerId, int seed)`
- Source range: `scripts/core/commands/MatchCommand.cs:22`
- Inputs:
  - `SeatPosition actorSeat, int actorPeerId, int seed`
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

### `FinishShuffle(SeatPosition actorSeat, int actorPeerId)`

- Signature: `public static MatchCommand FinishShuffle(SeatPosition actorSeat, int actorPeerId)`
- Source range: `scripts/core/commands/MatchCommand.cs:30`
- Inputs:
  - `SeatPosition actorSeat, int actorPeerId`
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

### `CutDeck(SeatPosition actorSeat, int actorPeerId, int cutIndex)`

- Signature: `public static MatchCommand CutDeck(SeatPosition actorSeat, int actorPeerId, int cutIndex)`
- Source range: `scripts/core/commands/MatchCommand.cs:37`
- Inputs:
  - `SeatPosition actorSeat, int actorPeerId, int cutIndex`
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

### `SelectTrump(SeatPosition actorSeat, int actorPeerId, CardSuit suit)`

- Signature: `public static MatchCommand SelectTrump(SeatPosition actorSeat, int actorPeerId, CardSuit suit)`
- Source range: `scripts/core/commands/MatchCommand.cs:45`
- Inputs:
  - `SeatPosition actorSeat, int actorPeerId, CardSuit suit`
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

### `PlayCard(SeatPosition actorSeat, int actorPeerId, string cardId)`

- Signature: `public static MatchCommand PlayCard(SeatPosition actorSeat, int actorPeerId, string cardId)`
- Source range: `scripts/core/commands/MatchCommand.cs:53`
- Inputs:
  - `SeatPosition actorSeat, int actorPeerId, string cardId`
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

### `StartNextRound(int seed)`

- Signature: `public static MatchCommand StartNextRound(int seed)`
- Source range: `scripts/core/commands/MatchCommand.cs:61`
- Inputs:
  - `int seed`
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

### `ForfeitTeam(int teamIndex)`

- Signature: `public static MatchCommand ForfeitTeam(int teamIndex)`
- Source range: `scripts/core/commands/MatchCommand.cs:67`
- Inputs:
  - `int teamIndex`
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

### `CompleteFirstDeal()`

- Signature: `public static MatchCommand CompleteFirstDeal()`
- Source range: `scripts/core/commands/MatchCommand.cs:73`
- Inputs:
  - `none`
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

### `CompleteSecondDeal()`

- Signature: `public static MatchCommand CompleteSecondDeal()`
- Source range: `scripts/core/commands/MatchCommand.cs:78`
- Inputs:
  - `none`
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

### `ResolveCurrentTrick()`

- Signature: `public static MatchCommand ResolveCurrentTrick()`
- Source range: `scripts/core/commands/MatchCommand.cs:83`
- Inputs:
  - `none`
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
