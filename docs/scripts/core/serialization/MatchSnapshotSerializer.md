# MatchSnapshotSerializer (core/serialization/MatchSnapshotSerializer.cs)

- Source: `scripts/core/serialization/MatchSnapshotSerializer.cs`
- Namespace: `NetDex.Core.Serialization`
- Purpose: MatchSnapshotSerializer in `core/serialization/MatchSnapshotSerializer.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Core.Enums`
  - `NetDex.Core.Models`
  - `System`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `Serialize(VisibleMatchState view)` | `Godot.Collections.Dictionary` | Public entry point |
| `SerializeJson(VisibleMatchState view)` | `string` | Public entry point |
| `ParseJson(string json)` | `Godot.Collections.Dictionary` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `Serialize(VisibleMatchState view)`

- Signature: `public static Godot.Collections.Dictionary Serialize(VisibleMatchState view)`
- Source range: `scripts/core/serialization/MatchSnapshotSerializer.cs:10`
- Inputs:
  - `VisibleMatchState view`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `Dictionary(...)`
  - Calls `GetValues(...)`
  - Calls `ToString(...)`
  - Calls `TryGetValue(...)`
  - Calls `Array(...)`
  - Calls `Add(...)`
  - Calls `ToDictionary(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SerializeJson(VisibleMatchState view)`

- Signature: `public static string SerializeJson(VisibleMatchState view)`
- Source range: `scripts/core/serialization/MatchSnapshotSerializer.cs:65`
- Inputs:
  - `VisibleMatchState view`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Stringify(...)`
  - Calls `Serialize(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ParseJson(string json)`

- Signature: `public static Godot.Collections.Dictionary ParseJson(string json)`
- Source range: `scripts/core/serialization/MatchSnapshotSerializer.cs:70`
- Inputs:
  - `string json`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ParseString(...)`
  - Calls `AsGodotDictionary(...)`
  - Calls `Dictionary(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
