# VoidSuitTracker (ai/VoidSuitTracker.cs)

- Source: `scripts/ai/VoidSuitTracker.cs`
- Namespace: `NetDex.AI`
- Purpose: VoidSuitTracker in `ai/VoidSuitTracker.cs`. AI decision-making, imperfect-information perception, and bot action scheduling.

## Dependencies

- Key imports:
  - `NetDex.Core.Enums`
  - `NetDex.Core.Models`
  - `System`
  - `System.Collections.Generic`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `VoidSuitTracker()` | `(constructor)` | Public entry point |
| `CanSeatHoldSuit(SeatPosition seat, CardSuit suit)` | `bool` | Public entry point |
| `GetVoids(SeatPosition seat)` | `IReadOnlyCollection<CardSuit>` | Public entry point |
| `MarkVoid(SeatPosition seat, CardSuit suit)` | `void` | Public entry point |
| `BuildFromTricks(IReadOnlyList<List<PlayedCard>> completedTricks, IReadOnlyList<PlayedCard> currentTrick)` | `VoidSuitTracker` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `TrackTrickVoidInfo(VoidSuitTracker tracker, IReadOnlyList<PlayedCard> trick)` | `private` | `void` |

## Function-by-Function

### `VoidSuitTracker()`

- Signature: `public (constructor) VoidSuitTracker()`
- Source range: `scripts/ai/VoidSuitTracker.cs:12`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetValues(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `CanSeatHoldSuit(SeatPosition seat, CardSuit suit)`

- Signature: `public bool CanSeatHoldSuit(SeatPosition seat, CardSuit suit)`
- Source range: `scripts/ai/VoidSuitTracker.cs:20`
- Inputs:
  - `SeatPosition seat, CardSuit suit`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `Contains(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetVoids(SeatPosition seat)`

- Signature: `public IReadOnlyCollection<CardSuit> GetVoids(SeatPosition seat)`
- Source range: `scripts/ai/VoidSuitTracker.cs:25`
- Inputs:
  - `SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `MarkVoid(SeatPosition seat, CardSuit suit)`

- Signature: `public void MarkVoid(SeatPosition seat, CardSuit suit)`
- Source range: `scripts/ai/VoidSuitTracker.cs:32`
- Inputs:
  - `SeatPosition seat, CardSuit suit`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `Add(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildFromTricks(IReadOnlyList<List<PlayedCard>> completedTricks, IReadOnlyList<PlayedCard> currentTrick)`

- Signature: `public static VoidSuitTracker BuildFromTricks(IReadOnlyList<List<PlayedCard>> completedTricks, IReadOnlyList<PlayedCard> currentTrick)`
- Source range: `scripts/ai/VoidSuitTracker.cs:40`
- Inputs:
  - `IReadOnlyList<List<PlayedCard>> completedTricks, IReadOnlyList<PlayedCard> currentTrick`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `VoidSuitTracker(...)`
  - Calls `TrackTrickVoidInfo(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `TrackTrickVoidInfo(VoidSuitTracker tracker, IReadOnlyList<PlayedCard> trick)`

- Signature: `private static void TrackTrickVoidInfo(VoidSuitTracker tracker, IReadOnlyList<PlayedCard> trick)`
- Source range: `scripts/ai/VoidSuitTracker.cs:56`
- Inputs:
  - `VoidSuitTracker tracker, IReadOnlyList<PlayedCard> trick`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `MarkVoid(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
