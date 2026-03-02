# CardModelConversions (core/serialization/CardModelConversions.cs)

- Source: `scripts/core/serialization/CardModelConversions.cs`
- Namespace: `NetDex.Core.Serialization`
- Purpose: CardModelConversions in `core/serialization/CardModelConversions.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `NetDex.Core.Enums`
  - `NetDex.Core.Models`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `FromDictionary(Godot.Collections.Dictionary dict)` | `CardModel` | Public entry point |
| `ToDictionary(this CardModel card)` | `Godot.Collections.Dictionary` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `FromDictionary(Godot.Collections.Dictionary dict)`

- Signature: `public static CardModel FromDictionary(Godot.Collections.Dictionary dict)`
- Source range: `scripts/core/serialization/CardModelConversions.cs:8`
- Inputs:
  - `Godot.Collections.Dictionary dict`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `AsString(...)`
  - Calls `AsInt32(...)`
  - Calls `CardModel(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ToDictionary(this CardModel card)`

- Signature: `public static Godot.Collections.Dictionary ToDictionary(this CardModel card)`
- Source range: `scripts/core/serialization/CardModelConversions.cs:20`
- Inputs:
  - `this CardModel card`
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
