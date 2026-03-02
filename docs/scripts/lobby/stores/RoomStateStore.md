# RoomStateStore (lobby/stores/RoomStateStore.cs)

- Source: `scripts/lobby/stores/RoomStateStore.cs`
- Namespace: `NetDex.Lobby.Stores`
- Purpose: RoomStateStore in `lobby/stores/RoomStateStore.cs`. Room membership, seat assignment, match lifecycle state, and room snapshots.

## Dependencies

- Key imports: none
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `Set(RoomState room)` | `void` | Public entry point |
| `Clear()` | `void` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `Set(RoomState room)`

- Signature: `public void Set(RoomState room)`
- Source range: `scripts/lobby/stores/RoomStateStore.cs:7`
- Inputs:
  - `RoomState room`
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
- Source range: `scripts/lobby/stores/RoomStateStore.cs:12`
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
