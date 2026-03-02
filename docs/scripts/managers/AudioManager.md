# AudioManager (managers/AudioManager.cs)

- Source: `scripts/managers/AudioManager.cs`
- Namespace: `NetDex.Managers`
- Purpose: AudioManager in `managers/AudioManager.cs`. Global scene/audio managers used across menus and gameplay.

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
| `_Ready()` | `void` | Public entry point |
| `SetMusicVolumePercent(double value)` | `void` | Public entry point |
| `SetSfxVolumePercent(double value)` | `void` | Public entry point |
| `GetSfxBusName()` | `string` | Public entry point |
| `GetMusicBusName()` | `string` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `PercentToDb(double percent)` | `private` | `float` |
| `EnsureBuses()` | `private` | `void` |
| `EnsureBus(string busName)` | `private` | `void` |
| `SetupMusicPlayer()` | `private` | `void` |
| `StartBackgroundMusic()` | `private` | `void` |
| `LoadAudioSettings()` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/managers/AudioManager.cs:20`
- Inputs:
  - `none`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `QueueFree(...)`
  - Calls `EnsureBuses(...)`
  - Calls `SetupMusicPlayer(...)`
  - Calls `LoadAudioSettings(...)`
  - Calls `StartBackgroundMusic(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetMusicVolumePercent(double value)`

- Signature: `public void SetMusicVolumePercent(double value)`
- Source range: `scripts/managers/AudioManager.cs:35`
- Inputs:
  - `double value`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Clamp(...)`
  - Calls `GetBusIndex(...)`
  - Calls `SetBusVolumeDb(...)`
  - Calls `PercentToDb(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetSfxVolumePercent(double value)`

- Signature: `public void SetSfxVolumePercent(double value)`
- Source range: `scripts/managers/AudioManager.cs:45`
- Inputs:
  - `double value`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Clamp(...)`
  - Calls `GetBusIndex(...)`
  - Calls `SetBusVolumeDb(...)`
  - Calls `PercentToDb(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetSfxBusName()`

- Signature: `public static string GetSfxBusName()`
- Source range: `scripts/managers/AudioManager.cs:55`
- Inputs:
  - `none`
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

### `GetMusicBusName()`

- Signature: `public static string GetMusicBusName()`
- Source range: `scripts/managers/AudioManager.cs:56`
- Inputs:
  - `none`
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

### `PercentToDb(double percent)`

- Signature: `private static float PercentToDb(double percent)`
- Source range: `scripts/managers/AudioManager.cs:58`
- Inputs:
  - `double percent`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `LinearToDb(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `EnsureBuses()`

- Signature: `private static void EnsureBuses()`
- Source range: `scripts/managers/AudioManager.cs:68`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EnsureBus(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `EnsureBus(string busName)`

- Signature: `private static void EnsureBus(string busName)`
- Source range: `scripts/managers/AudioManager.cs:74`
- Inputs:
  - `string busName`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetBusIndex(...)`
  - Calls `AddBus(...)`
  - Calls `SetBusName(...)`
  - Calls `SetBusSend(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetupMusicPlayer()`

- Signature: `private void SetupMusicPlayer()`
- Source range: `scripts/managers/AudioManager.cs:87`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `AddChild(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `StartBackgroundMusic()`

- Signature: `private void StartBackgroundMusic()`
- Source range: `scripts/managers/AudioManager.cs:104`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Play(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadAudioSettings()`

- Signature: `private void LoadAudioSettings()`
- Source range: `scripts/managers/AudioManager.cs:114`
- Inputs:
  - `none`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ConfigFile(...)`
  - Calls `Load(...)`
  - Calls `SetMusicVolumePercent(...)`
  - Calls `SetSfxVolumePercent(...)`
  - Calls `GetValue(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
