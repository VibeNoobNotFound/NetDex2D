# GameManager (managers/GameManager.cs)

- Source: `scripts/managers/GameManager.cs`
- Namespace: `NetDex.Managers`
- Purpose: GameManager in `managers/GameManager.cs`. Global scene/audio managers used across menus and gameplay.

## Dependencies

- Key imports:
  - `Godot`
- Autoload/manager dependencies detected: none
- Scene node paths used: `2`

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `_Notification(int what)` | `void` | Public entry point |
| `LoadMainMenu()` | `void` | Public entry point |
| `LoadHostScreen()` | `void` | Public entry point |
| `LoadJoinScreen()` | `void` | Public entry point |
| `LoadLobby()` | `void` | Public entry point |
| `LoadGameScene()` | `void` | Public entry point |
| `LoadAboutScreen(string returnScreen = "MainMenu")` | `void` | Public entry point |
| `ReturnFromAbout()` | `void` | Public entry point |
| `LoadSettingsMenu(string returnScreen = "MainMenu")` | `void` | Public entry point |
| `ReturnFromSettings()` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `SetMenuState(string menuName)` | `private` | `void` |
| `IsKnownScreen(string menuName)` | `private` | `bool` |
| `EnsureMobileFullscreen()` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/managers/GameManager.cs:22`
- Inputs:
  - `none`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `QueueFree(...)`
  - Calls `EnsureMobileFullscreen(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `_Notification(int what)`

- Signature: `public override void _Notification(int what)`
- Source range: `scripts/managers/GameManager.cs:34`
- Inputs:
  - `int what`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `EnsureMobileFullscreen(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadMainMenu()`

- Signature: `public void LoadMainMenu()`
- Source range: `scripts/managers/GameManager.cs:42`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetMenuState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadHostScreen()`

- Signature: `public void LoadHostScreen()`
- Source range: `scripts/managers/GameManager.cs:43`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetMenuState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadJoinScreen()`

- Signature: `public void LoadJoinScreen()`
- Source range: `scripts/managers/GameManager.cs:44`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetMenuState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadLobby()`

- Signature: `public void LoadLobby()`
- Source range: `scripts/managers/GameManager.cs:45`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetMenuState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadGameScene()`

- Signature: `public void LoadGameScene()`
- Source range: `scripts/managers/GameManager.cs:46`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetMenuState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadAboutScreen(string returnScreen = "MainMenu")`

- Signature: `public void LoadAboutScreen(string returnScreen = "MainMenu")`
- Source range: `scripts/managers/GameManager.cs:47`
- Inputs:
  - `string returnScreen = "MainMenu"`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `SetMenuState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ReturnFromAbout()`

- Signature: `public void ReturnFromAbout()`
- Source range: `scripts/managers/GameManager.cs:53`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetMenuState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadSettingsMenu(string returnScreen = "MainMenu")`

- Signature: `public void LoadSettingsMenu(string returnScreen = "MainMenu")`
- Source range: `scripts/managers/GameManager.cs:58`
- Inputs:
  - `string returnScreen = "MainMenu"`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `SetMenuState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ReturnFromSettings()`

- Signature: `public void ReturnFromSettings()`
- Source range: `scripts/managers/GameManager.cs:64`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetMenuState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetMenuState(string menuName)`

- Signature: `private void SetMenuState(string menuName)`
- Source range: `scripts/managers/GameManager.cs:69`
- Inputs:
  - `string menuName`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetTree(...)`
  - Calls `IsKnownScreen(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `IsKnownScreen(string menuName)`

- Signature: `private static bool IsKnownScreen(string menuName)`
- Source range: `scripts/managers/GameManager.cs:101`
- Inputs:
  - `string menuName`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `EnsureMobileFullscreen()`

- Signature: `private static void EnsureMobileFullscreen()`
- Source range: `scripts/managers/GameManager.cs:114`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `HasFeature(...)`
  - Calls `WindowSetMode(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
