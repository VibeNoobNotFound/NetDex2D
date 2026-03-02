# PauseMenu (ui/PauseMenu.cs)

- Source: `scripts/ui/PauseMenu.cs`
- Namespace: `NetDex.UI.Game`
- Purpose: PauseMenu in `ui/PauseMenu.cs`. Menu/lobby/settings/about/pause user interface scripts and interactions.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Managers`
  - `NetDex.Networking`
- Autoload/manager dependencies detected:
  - `GameManager`
  - `NetworkManager`
- Scene node paths used: `3`

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `OnResumePressed()` | `private` | `void` |
| `OnSettingsPressed()` | `private` | `void` |
| `OnLeavePressed()` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/ui/PauseMenu.cs:9`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Hide(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnResumePressed()`

- Signature: `private void OnResumePressed()`
- Source range: `scripts/ui/PauseMenu.cs:18`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Hide(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnSettingsPressed()`

- Signature: `private void OnSettingsPressed()`
- Source range: `scripts/ui/PauseMenu.cs:23`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Hide(...)`
  - Calls `LoadSettingsMenu(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnLeavePressed()`

- Signature: `private static void OnLeavePressed()`
- Source range: `scripts/ui/PauseMenu.cs:29`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `DisconnectSession(...)`
  - Calls `LoadMainMenu(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
