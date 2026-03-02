# AboutScreen (ui/AboutScreen.cs)

- Source: `scripts/ui/AboutScreen.cs`
- Namespace: `NetDex.UI.Main`
- Purpose: AboutScreen in `ui/AboutScreen.cs`. Menu/lobby/settings/about/pause user interface scripts and interactions.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Managers`
  - `NetDex.Updates`
- Autoload/manager dependencies detected:
  - `GameManager`
  - `UpdateManager`
- Scene node paths used: `16`

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `_ExitTree()` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `InitializeUi()` | `private` | `void` |
| `PopulateInfo()` | `private` | `void` |
| `RefreshUpdateUi()` | `private` | `void` |
| `OnCheckUpdatesPressed()` | `private` | `void` |
| `OnUpdateActionPressed()` | `private` | `void` |
| `OnSkipUpdatePressed()` | `private` | `void` |
| `OnOpenRepoPressed()` | `private` | `void` |
| `OnOpenReleasesPressed()` | `private` | `void` |
| `OnBackPressed()` | `private` | `void` |
| `OnVisibilityChanged()` | `private` | `void` |
| `OnUpdateStatusChanged(string state, string message)` | `private` | `void` |
| `OnUpdateAvailable(string version, string platformActionLabel)` | `private` | `void` |
| `OnUpdateIssueRaised(int issueCode, string message)` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/ui/AboutScreen.cs:23`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `InitializeUi(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `_ExitTree()`

- Signature: `public override void _ExitTree()`
- Source range: `scripts/ui/AboutScreen.cs:88`
- Inputs:
  - `none`
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

### `InitializeUi()`

- Signature: `private void InitializeUi()`
- Source range: `scripts/ui/AboutScreen.cs:29`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `PushError(...)`
  - Calls `Label(...)`
  - Calls `Button(...)`
  - Calls `PopulateInfo(...)`
  - Calls `RefreshUpdateUi(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PopulateInfo()`

- Signature: `private void PopulateInfo()`
- Source range: `scripts/ui/AboutScreen.cs:102`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `GetSetting(...)`
  - Calls `AsString(...)`
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `ToString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RefreshUpdateUi()`

- Signature: `private void RefreshUpdateUi()`
- Source range: `scripts/ui/AboutScreen.cs:133`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `GetLatestVersionLabel(...)`
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `GetActionLabel(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnCheckUpdatesPressed()`

- Signature: `private static void OnCheckUpdatesPressed()`
- Source range: `scripts/ui/AboutScreen.cs:156`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `CheckForUpdates(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnUpdateActionPressed()`

- Signature: `private static void OnUpdateActionPressed()`
- Source range: `scripts/ui/AboutScreen.cs:161`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `PerformUpdateAction(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnSkipUpdatePressed()`

- Signature: `private static void OnSkipUpdatePressed()`
- Source range: `scripts/ui/AboutScreen.cs:166`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SkipCurrentUpdate(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnOpenRepoPressed()`

- Signature: `private static void OnOpenRepoPressed()`
- Source range: `scripts/ui/AboutScreen.cs:171`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ShellOpen(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnOpenReleasesPressed()`

- Signature: `private static void OnOpenReleasesPressed()`
- Source range: `scripts/ui/AboutScreen.cs:182`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ShellOpen(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnBackPressed()`

- Signature: `private void OnBackPressed()`
- Source range: `scripts/ui/AboutScreen.cs:193`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ReturnFromAbout(...)`
  - Calls `GetTree(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnVisibilityChanged()`

- Signature: `private void OnVisibilityChanged()`
- Source range: `scripts/ui/AboutScreen.cs:204`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `PopulateInfo(...)`
  - Calls `RefreshUpdateUi(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnUpdateStatusChanged(string state, string message)`

- Signature: `private void OnUpdateStatusChanged(string state, string message)`
- Source range: `scripts/ui/AboutScreen.cs:215`
- Inputs:
  - `string state, string message`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `RefreshUpdateUi(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnUpdateAvailable(string version, string platformActionLabel)`

- Signature: `private void OnUpdateAvailable(string version, string platformActionLabel)`
- Source range: `scripts/ui/AboutScreen.cs:220`
- Inputs:
  - `string version, string platformActionLabel`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `RefreshUpdateUi(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnUpdateIssueRaised(int issueCode, string message)`

- Signature: `private void OnUpdateIssueRaised(int issueCode, string message)`
- Source range: `scripts/ui/AboutScreen.cs:226`
- Inputs:
  - `int issueCode, string message`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `RefreshUpdateUi(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
