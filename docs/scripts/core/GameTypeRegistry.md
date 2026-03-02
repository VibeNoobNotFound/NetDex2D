# GameTypeRegistry (core/GameTypeRegistry.cs)

- Source: `scripts/core/GameTypeRegistry.cs`
- Namespace: `NetDex.Core.Rules`
- Purpose: GameTypeRegistry in `core/GameTypeRegistry.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `NetDex.Core.Enums`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `Resolve(GameType gameType)` | `IGameRulesEngine` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `Resolve(GameType gameType)`

- Signature: `public static IGameRulesEngine Resolve(GameType gameType)`
- Source range: `scripts/core/GameTypeRegistry.cs:9`
- Inputs:
  - `GameType gameType`
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
