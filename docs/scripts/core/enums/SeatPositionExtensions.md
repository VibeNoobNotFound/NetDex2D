# SeatPositionExtensions (core/enums/SeatPositionExtensions.cs)

- Source: `scripts/core/enums/SeatPositionExtensions.cs`
- Namespace: `NetDex.Core.Enums`
- Purpose: SeatPositionExtensions in `core/enums/SeatPositionExtensions.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `System`
  - `System.Collections.Generic`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `Next(this SeatPosition seat)` | `SeatPosition` | Public entry point |
| `Previous(this SeatPosition seat)` | `SeatPosition` | Public entry point |
| `TeamIndex(this SeatPosition seat)` | `int` | Public entry point |
| `OrderedFrom(this SeatPosition start)` | `IReadOnlyList<SeatPosition>` | Public entry point |
| `Parse(string value)` | `SeatPosition?` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `Next(this SeatPosition seat)`

- Signature: `public static SeatPosition Next(this SeatPosition seat)`
- Source range: `scripts/core/enums/SeatPositionExtensions.cs:8`
- Inputs:
  - `this SeatPosition seat`
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

### `Previous(this SeatPosition seat)`

- Signature: `public static SeatPosition Previous(this SeatPosition seat)`
- Source range: `scripts/core/enums/SeatPositionExtensions.cs:13`
- Inputs:
  - `this SeatPosition seat`
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

### `TeamIndex(this SeatPosition seat)`

- Signature: `public static int TeamIndex(this SeatPosition seat)`
- Source range: `scripts/core/enums/SeatPositionExtensions.cs:18`
- Inputs:
  - `this SeatPosition seat`
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

### `OrderedFrom(this SeatPosition start)`

- Signature: `public static IReadOnlyList<SeatPosition> OrderedFrom(this SeatPosition start)`
- Source range: `scripts/core/enums/SeatPositionExtensions.cs:23`
- Inputs:
  - `this SeatPosition start`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Add(...)`
  - Calls `Next(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `Parse(string value)`

- Signature: `public static SeatPosition? Parse(string value)`
- Source range: `scripts/core/enums/SeatPositionExtensions.cs:36`
- Inputs:
  - `string value`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
