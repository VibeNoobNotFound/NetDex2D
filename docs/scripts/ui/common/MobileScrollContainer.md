# MobileScrollContainer (ui/common/MobileScrollContainer.cs)

- Source: `scripts/ui/common/MobileScrollContainer.cs`
- Namespace: `NetDex.UI.Common`
- Purpose: MobileScrollContainer in `ui/common/MobileScrollContainer.cs`. Menu/lobby/settings/about/pause user interface scripts and interactions.

## Dependencies

- Key imports:
  - `Godot`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Input(InputEvent @event)` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `HandleScreenTouch(InputEventScreenTouch touch)` | `private` | `void` |
| `HandleScreenDrag(InputEventScreenDrag drag)` | `private` | `void` |
| `HandleMouseButton(InputEventMouseButton button)` | `private` | `void` |
| `HandleMouseMotion(InputEventMouseMotion motion)` | `private` | `void` |
| `BeginPointer(int pointerIndex, Vector2 position)` | `private` | `void` |
| `EndPointer()` | `private` | `void` |
| `UpdateDrag(Vector2 currentPosition)` | `private` | `void` |

## Function-by-Function

### `_Input(InputEvent @event)`

- Signature: `public override void _Input(InputEvent @event)`
- Source range: `scripts/ui/common/MobileScrollContainer.cs:19`
- Inputs:
  - `InputEvent @event`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `IsVisibleInTree(...)`
  - Calls `HandleScreenTouch(...)`
  - Calls `HandleScreenDrag(...)`
  - Calls `HandleMouseButton(...)`
  - Calls `HandleMouseMotion(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `HandleScreenTouch(InputEventScreenTouch touch)`

- Signature: `private void HandleScreenTouch(InputEventScreenTouch touch)`
- Source range: `scripts/ui/common/MobileScrollContainer.cs:43`
- Inputs:
  - `InputEventScreenTouch touch`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetGlobalRect(...)`
  - Calls `HasPoint(...)`
  - Calls `BeginPointer(...)`
  - Calls `EndPointer(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `HandleScreenDrag(InputEventScreenDrag drag)`

- Signature: `private void HandleScreenDrag(InputEventScreenDrag drag)`
- Source range: `scripts/ui/common/MobileScrollContainer.cs:62`
- Inputs:
  - `InputEventScreenDrag drag`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `UpdateDrag(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `HandleMouseButton(InputEventMouseButton button)`

- Signature: `private void HandleMouseButton(InputEventMouseButton button)`
- Source range: `scripts/ui/common/MobileScrollContainer.cs:72`
- Inputs:
  - `InputEventMouseButton button`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetGlobalRect(...)`
  - Calls `HasPoint(...)`
  - Calls `BeginPointer(...)`
  - Calls `GetViewport(...)`
  - Calls `SetInputAsHandled(...)`
  - Calls `EndPointer(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `HandleMouseMotion(InputEventMouseMotion motion)`

- Signature: `private void HandleMouseMotion(InputEventMouseMotion motion)`
- Source range: `scripts/ui/common/MobileScrollContainer.cs:93`
- Inputs:
  - `InputEventMouseMotion motion`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `UpdateDrag(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BeginPointer(int pointerIndex, Vector2 position)`

- Signature: `private void BeginPointer(int pointerIndex, Vector2 position)`
- Source range: `scripts/ui/common/MobileScrollContainer.cs:103`
- Inputs:
  - `int pointerIndex, Vector2 position`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `EndPointer()`

- Signature: `private void EndPointer()`
- Source range: `scripts/ui/common/MobileScrollContainer.cs:112`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `UpdateDrag(Vector2 currentPosition)`

- Signature: `private void UpdateDrag(Vector2 currentPosition)`
- Source range: `scripts/ui/common/MobileScrollContainer.cs:119`
- Inputs:
  - `Vector2 currentPosition`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Abs(...)`
  - Calls `GetVScrollBar(...)`
  - Calls `RoundToInt(...)`
  - Calls `Clamp(...)`
  - Calls `GetViewport(...)`
  - Calls `SetInputAsHandled(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
