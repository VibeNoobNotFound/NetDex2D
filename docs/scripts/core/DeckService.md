# DeckService (core/DeckService.cs)

- Source: `scripts/core/DeckService.cs`
- Namespace: `NetDex.Core.Rules`
- Purpose: DeckService in `core/DeckService.cs`. Deterministic game domain, Omi rules engine, commands, enums, and serialization.

## Dependencies

- Key imports:
  - `NetDex.Core.Enums`
  - `NetDex.Core.Models`
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
| `BuildOmiDeck()` | `List<CardModel>` | Public entry point |
| `Shuffle(IReadOnlyList<CardModel> source, int seed)` | `List<CardModel>` | Public entry point |
| `Cut(IReadOnlyList<CardModel> source, int cutIndex)` | `List<CardModel>` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `BuildOmiDeck()`

- Signature: `public static List<CardModel> BuildOmiDeck()`
- Source range: `scripts/core/DeckService.cs:11`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetValues(...)`
  - Calls `Add(...)`
  - Calls `CardModel(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `Shuffle(IReadOnlyList<CardModel> source, int seed)`

- Signature: `public static List<CardModel> Shuffle(IReadOnlyList<CardModel> source, int seed)`
- Source range: `scripts/core/DeckService.cs:26`
- Inputs:
  - `IReadOnlyList<CardModel> source, int seed`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ToList(...)`
  - Calls `Random(...)`
  - Calls `Next(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `Cut(IReadOnlyList<CardModel> source, int cutIndex)`

- Signature: `public static List<CardModel> Cut(IReadOnlyList<CardModel> source, int cutIndex)`
- Source range: `scripts/core/DeckService.cs:40`
- Inputs:
  - `IReadOnlyList<CardModel> source, int cutIndex`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Clamp(...)`
  - Calls `Take(...)`
  - Calls `ToList(...)`
  - Calls `Skip(...)`
  - Calls `AddRange(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
