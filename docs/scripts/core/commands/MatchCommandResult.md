# MatchCommandResult (core/commands/MatchCommandResult.cs)

- Source: `scripts/core/commands/MatchCommandResult.cs`
- Namespace: `NetDex.Core.Commands`
- Purpose: MatchCommandResult in `core/commands/MatchCommandResult.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `System.Collections.Generic`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `Ok(params MatchEvent[] events)` | `MatchCommandResult` | Public entry point |
| `Fail(string error)` | `MatchCommandResult` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `Ok(params MatchEvent[] events)`

- Signature: `public static MatchCommandResult Ok(params MatchEvent[] events)`
- Source range: `scripts/core/commands/MatchCommandResult.cs:11`
- Inputs:
  - `params MatchEvent[] events`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `AddRange(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `Fail(string error)`

- Signature: `public static MatchCommandResult Fail(string error)`
- Source range: `scripts/core/commands/MatchCommandResult.cs:18`
- Inputs:
  - `string error`
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
