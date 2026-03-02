# MatchStateStore (lobby/stores/MatchStateStore.cs)

- Source: `scripts/lobby/stores/MatchStateStore.cs`
- Namespace: `NetDex.Lobby.Stores`
- Purpose: MatchStateStore in `lobby/stores/MatchStateStore.cs`. Room membership, seat assignment, match lifecycle state, and room snapshots.

## Dependencies

- Key imports: none
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `Set(Godot.Collections.Dictionary snapshot)` | `void` | Public entry point |
| `Clear()` | `void` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `Set(Godot.Collections.Dictionary snapshot)`

- Signature: `public void Set(Godot.Collections.Dictionary snapshot)`
- Source range: `scripts/lobby/stores/MatchStateStore.cs:7`
- Inputs:
  - `Godot.Collections.Dictionary snapshot`
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

### `Clear()`

- Signature: `public void Clear()`
- Source range: `scripts/lobby/stores/MatchStateStore.cs:12`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Dictionary(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
