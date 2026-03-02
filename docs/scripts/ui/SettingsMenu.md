# SettingsMenu (ui/SettingsMenu.cs)

- Source: `scripts/ui/SettingsMenu.cs`
- Namespace: `NetDex.UI.Main`
- Purpose: SettingsMenu in `ui/SettingsMenu.cs`. Menu/lobby/settings/about/pause user interface scripts and interactions.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Managers`
- Autoload/manager dependencies detected:
  - `AudioManager`
  - `GameManager`
- Scene node paths used: `13`

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `OnFullscreenToggled(bool toggled)` | `private` | `void` |
| `OnQualitySliderChanged(double value)` | `private` | `void` |
| `OnMusicVolumeChanged(double value)` | `private` | `void` |
| `OnSfxVolumeChanged(double value)` | `private` | `void` |
| `SaveSettings()` | `private` | `void` |
| `LoadSettings()` | `private` | `void` |
| `OnBackPressed()` | `private` | `void` |
| `OnAboutPressed()` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/ui/SettingsMenu.cs:17`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetName(...)`
  - Calls `LoadSettings(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnFullscreenToggled(bool toggled)`

- Signature: `private void OnFullscreenToggled(bool toggled)`
- Source range: `scripts/ui/SettingsMenu.cs:53`
- Inputs:
  - `bool toggled`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `WindowSetMode(...)`
  - Calls `SaveSettings(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnQualitySliderChanged(double value)`

- Signature: `private void OnQualitySliderChanged(double value)`
- Source range: `scripts/ui/SettingsMenu.cs:71`
- Inputs:
  - `double value`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetWindow(...)`
  - Calls `SaveSettings(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnMusicVolumeChanged(double value)`

- Signature: `private void OnMusicVolumeChanged(double value)`
- Source range: `scripts/ui/SettingsMenu.cs:78`
- Inputs:
  - `double value`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetMusicVolumePercent(...)`
  - Calls `SaveSettings(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnSfxVolumeChanged(double value)`

- Signature: `private void OnSfxVolumeChanged(double value)`
- Source range: `scripts/ui/SettingsMenu.cs:85`
- Inputs:
  - `double value`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetSfxVolumePercent(...)`
  - Calls `SaveSettings(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SaveSettings()`

- Signature: `private void SaveSettings()`
- Source range: `scripts/ui/SettingsMenu.cs:92`
- Inputs:
  - `none`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ConfigFile(...)`
  - Calls `SetValue(...)`
  - Calls `Save(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadSettings()`

- Signature: `private void LoadSettings()`
- Source range: `scripts/ui/SettingsMenu.cs:102`
- Inputs:
  - `none`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ConfigFile(...)`
  - Calls `Load(...)`
  - Calls `GetValue(...)`
  - Calls `SetPressedNoSignal(...)`
  - Calls `WindowSetMode(...)`
  - Calls `SetValueNoSignal(...)`
  - Calls `GetWindow(...)`
  - Calls `SetMusicVolumePercent(...)`
  - Calls `SetSfxVolumePercent(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnBackPressed()`

- Signature: `private static void OnBackPressed()`
- Source range: `scripts/ui/SettingsMenu.cs:158`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ReturnFromSettings(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnAboutPressed()`

- Signature: `private static void OnAboutPressed()`
- Source range: `scripts/ui/SettingsMenu.cs:163`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `LoadAboutScreen(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
