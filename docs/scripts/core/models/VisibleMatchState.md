# VisibleMatchState (core/models/VisibleMatchState.cs)

- Source: `scripts/core/models/VisibleMatchState.cs`
- Namespace: `NetDex.Core.Models`
- Purpose: VisibleMatchState in `core/models/VisibleMatchState.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `NetDex.Core.Enums`
  - `System`
  - `System.Collections.Generic`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `VisibleMatchState()` | `(constructor)` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `VisibleMatchState()`

- Signature: `public (constructor) VisibleMatchState()`
- Source range: `scripts/core/models/VisibleMatchState.cs:15`
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
