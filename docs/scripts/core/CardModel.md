# struct (core/CardModel.cs)

- Source: `scripts/core/CardModel.cs`
- Namespace: `NetDex.Core.Models`
- Purpose: struct in `core/CardModel.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

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
| `CardModel(string Id, CardSuit Suit, CardRank Rank)` | `record struct` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `CardModel(string Id, CardSuit Suit, CardRank Rank)`

- Signature: `public readonly record struct CardModel(string Id, CardSuit Suit, CardRank Rank)`
- Source range: `scripts/core/CardModel.cs:5`
- Inputs:
  - `string Id, CardSuit Suit, CardRank Rank`
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
