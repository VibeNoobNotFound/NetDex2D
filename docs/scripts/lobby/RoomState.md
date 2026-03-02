# RoomState (lobby/RoomState.cs)

- Source: `scripts/lobby/RoomState.cs`
- Namespace: `NetDex.Lobby`
- Purpose: RoomState in `lobby/RoomState.cs`. Room membership, seat assignment, match lifecycle state, and room snapshots.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.AI`
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
| `TryGetSeatForPeer(int peerId, out SeatPosition seat)` | `bool` | Public entry point |
| `IsSeatTaken(SeatPosition seat)` | `bool` | Public entry point |
| `FirstEmptySeat()` | `SeatPosition?` | Public entry point |
| `SetSeat(SeatPosition seat, int? peerId)` | `void` | Public entry point |
| `ToSnapshotDictionary(bool includeReconnectToken = false)` | `Godot.Collections.Dictionary` | Public entry point |
| `FromSnapshotDictionary(Godot.Collections.Dictionary dict)` | `RoomState` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `TryGetSeatForPeer(int peerId, out SeatPosition seat)`

- Signature: `public bool TryGetSeatForPeer(int peerId, out SeatPosition seat)`
- Source range: `scripts/lobby/RoomState.cs:35`
- Inputs:
  - `int peerId, out SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `IsSeatTaken(SeatPosition seat)`

- Signature: `public bool IsSeatTaken(SeatPosition seat)`
- Source range: `scripts/lobby/RoomState.cs:50`
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

### `FirstEmptySeat()`

- Signature: `public SeatPosition? FirstEmptySeat()`
- Source range: `scripts/lobby/RoomState.cs:55`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetValues(...)`
  - Calls `IsSeatTaken(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetSeat(SeatPosition seat, int? peerId)`

- Signature: `public void SetSeat(SeatPosition seat, int? peerId)`
- Source range: `scripts/lobby/RoomState.cs:68`
- Inputs:
  - `SeatPosition seat, int? peerId`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `ToList(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ToSnapshotDictionary(bool includeReconnectToken = false)`

- Signature: `public Godot.Collections.Dictionary ToSnapshotDictionary(bool includeReconnectToken = false)`
- Source range: `scripts/lobby/RoomState.cs:95`
- Inputs:
  - `bool includeReconnectToken = false`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Array(...)`
  - Calls `OrderBy(...)`
  - Calls `Add(...)`
  - Calls `ToDictionary(...)`
  - Calls `Dictionary(...)`
  - Calls `ToString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `FromSnapshotDictionary(Godot.Collections.Dictionary dict)`

- Signature: `public static RoomState FromSnapshotDictionary(Godot.Collections.Dictionary dict)`
- Source range: `scripts/lobby/RoomState.cs:127`
- Inputs:
  - `Godot.Collections.Dictionary dict`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `AsString(...)`
  - Calls `AsInt32(...)`
  - Calls `NewGuid(...)`
  - Calls `ToString(...)`
  - Calls `AsBool(...)`
  - Calls `AsGodotArray(...)`
  - Calls `FromDictionary(...)`
  - Calls `AsGodotDictionary(...)`
  - Calls `GetValues(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
