# ParticipantInfo (lobby/ParticipantInfo.cs)

- Source: `scripts/lobby/ParticipantInfo.cs`
- Namespace: `NetDex.Lobby`
- Purpose: ParticipantInfo in `lobby/ParticipantInfo.cs`. Room membership, seat assignment, match lifecycle state, and room snapshots.

## Dependencies

- Key imports:
  - `NetDex.AI`
  - `NetDex.Core.Enums`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `ToDictionary(bool includeReconnectToken)` | `Godot.Collections.Dictionary` | Public entry point |
| `FromDictionary(Godot.Collections.Dictionary dict)` | `ParticipantInfo` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `ToDictionary(bool includeReconnectToken)`

- Signature: `public Godot.Collections.Dictionary ToDictionary(bool includeReconnectToken)`
- Source range: `scripts/lobby/ParticipantInfo.cs:20`
- Inputs:
  - `bool includeReconnectToken`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ToString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `FromDictionary(Godot.Collections.Dictionary dict)`

- Signature: `public static ParticipantInfo FromDictionary(Godot.Collections.Dictionary dict)`
- Source range: `scripts/lobby/ParticipantInfo.cs:42`
- Inputs:
  - `Godot.Collections.Dictionary dict`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `AsInt32(...)`
  - Calls `AsString(...)`
  - Calls `AsBool(...)`
  - Calls `Parse(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
