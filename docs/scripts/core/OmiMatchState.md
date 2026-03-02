# OmiMatchState (core/OmiMatchState.cs)

- Source: `scripts/core/OmiMatchState.cs`
- Namespace: `NetDex.Core.Models`
- Purpose: OmiMatchState in `core/OmiMatchState.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `NetDex.Core.Enums`
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
| `OmiMatchState()` | `(constructor)` | Public entry point |
| `ResetRoundHandsAndBoard()` | `void` | Public entry point |
| `DeepClone()` | `OmiMatchState` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `OmiMatchState()`

- Signature: `public (constructor) OmiMatchState()`
- Source range: `scripts/core/OmiMatchState.cs:44`
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

### `ResetRoundHandsAndBoard()`

- Signature: `public void ResetRoundHandsAndBoard()`
- Source range: `scripts/core/OmiMatchState.cs:52`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Clear(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `DeepClone()`

- Signature: `public OmiMatchState DeepClone()`
- Source range: `scripts/core/OmiMatchState.cs:71`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Clone(...)`
  - Calls `ToList(...)`
  - Calls `GetValues(...)`
  - Calls `Clear(...)`
  - Calls `AddRange(...)`
  - Calls `Add(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
