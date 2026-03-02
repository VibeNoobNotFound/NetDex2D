# Card (game/Card.cs)

- Source: `scripts/game/Card.cs`
- Namespace: `NetDex.UI.Game`
- Purpose: Card in `game/Card.cs`. Runtime match orchestration and in-game UI rendering/animation logic.

## Dependencies

- Key imports:
  - `Godot`
- Autoload/manager dependencies detected: none
- Scene node paths used: `1`

## Signals

| Signal | Parameters | Emitted In File |
|---|---|---|
| `CardClickedEventHandler` | `Card card` | No |

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `Setup(SuitType suit, RankType rank, bool isFaceUp = false, string cardId = "")` | `void` | Public entry point |
| `SetFaceUp(bool faceUp)` | `void` | Public entry point |
| `SetInteractable(bool interactable)` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `UpdateVisuals()` | `private` | `void` |
| `GetCardRegion(SuitType suit, RankType rank)` | `private` | `Rect2` |
| `OnGuiInput(InputEvent @event)` | `private` | `void` |
| `OnMouseEntered()` | `private` | `void` |
| `OnMouseExited()` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/game/Card.cs:27`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `UpdateVisuals(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `Setup(SuitType suit, RankType rank, bool isFaceUp = false, string cardId = "")`

- Signature: `public void Setup(SuitType suit, RankType rank, bool isFaceUp = false, string cardId = "")`
- Source range: `scripts/game/Card.cs:40`
- Inputs:
  - `SuitType suit, RankType rank, bool isFaceUp = false, string cardId = ""`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `IsInsideTree(...)`
  - Calls `UpdateVisuals(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetFaceUp(bool faceUp)`

- Signature: `public void SetFaceUp(bool faceUp)`
- Source range: `scripts/game/Card.cs:53`
- Inputs:
  - `bool faceUp`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `UpdateVisuals(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetInteractable(bool interactable)`

- Signature: `public void SetInteractable(bool interactable)`
- Source range: `scripts/game/Card.cs:59`
- Inputs:
  - `bool interactable`
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

### `UpdateVisuals()`

- Signature: `private void UpdateVisuals()`
- Source range: `scripts/game/Card.cs:64`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetCardRegion(...)`
  - Calls `Rect2(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetCardRegion(SuitType suit, RankType rank)`

- Signature: `private static Rect2 GetCardRegion(SuitType suit, RankType rank)`
- Source range: `scripts/game/Card.cs:93`
- Inputs:
  - `SuitType suit, RankType rank`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ToString(...)`
  - Calls `Rect2(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnGuiInput(InputEvent @event)`

- Signature: `private void OnGuiInput(InputEvent @event)`
- Source range: `scripts/game/Card.cs:155`
- Inputs:
  - `InputEvent @event`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnMouseEntered()`

- Signature: `private void OnMouseEntered()`
- Source range: `scripts/game/Card.cs:168`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Vector2(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnMouseExited()`

- Signature: `private void OnMouseExited()`
- Source range: `scripts/game/Card.cs:178`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Vector2(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
