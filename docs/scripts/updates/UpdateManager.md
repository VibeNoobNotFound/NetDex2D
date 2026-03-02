# UpdateManager (updates/UpdateManager.cs)

- Source: `scripts/updates/UpdateManager.cs`
- Namespace: `NetDex.Updates`
- Purpose: UpdateManager in `updates/UpdateManager.cs`. GitHub release updater state machine, checksum verification, and installers.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Updates.Installers`
  - `System`
  - `System.Collections.Generic`
  - `System.Threading.Tasks`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

| Signal | Parameters | Emitted In File |
|---|---|---|
| `UpdateStatusChangedEventHandler` | `string state, string message` | No |
| `UpdateAvailableEventHandler` | `string version, string platformActionLabel` | No |
| `UpdateIssueRaisedEventHandler` | `int issueCode, string message` | No |

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `GetActionLabel()` | `string` | Public entry point |
| `GetLatestVersionLabel()` | `string` | Public entry point |
| `CheckForUpdates(bool manual = false)` | `void` | Public entry point |
| `CheckForUpdatesAsync(bool manual = false)` | `Task` | Public entry point |
| `PerformUpdateAction()` | `void` | Public entry point |
| `PerformUpdateActionAsync()` | `Task` | Public entry point |
| `SkipCurrentUpdate()` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `AutoCheckIfNeededAsync()` | `private` | `Task` |
| `SetState(UpdateState state, string message)` | `private` | `void` |
| `SetError(UpdateIssueCode code, string message)` | `private` | `void` |
| `ResolvePlatformAsset(UpdateReleaseInfo release, UpdatePlatform platform)` | `private` | `UpdateAssetInfo?` |
| `FindAssetByName(UpdateReleaseInfo release, string expectedName)` | `private` | `UpdateAssetInfo?` |
| `LoadUpdaterConfig()` | `private` | `void` |
| `SaveUpdaterConfig()` | `private` | `void` |
| `DetectRuntimePlatform()` | `private` | `UpdatePlatform` |
| `TryParseOwnerRepoFromUrl(string repositoryUrl, out string owner, out string repo)` | `private` | `bool` |
| `GetProjectSettingString(string key, string fallback)` | `private` | `string` |
| `IsDesktopInstaller(UpdatePlatform platform)` | `private` | `bool` |
| `RequiresReleaseAsset(UpdatePlatform platform)` | `private` | `bool` |
| `RequiresDigestValidation(UpdatePlatform platform)` | `private` | `bool` |
| `GetDownloadFileName(UpdatePlatform platform)` | `private` | `string` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/updates/UpdateManager.cs:60`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `QueueFree(...)`
  - Calls `DetectRuntimePlatform(...)`
  - Calls `NormalizeTagToVersion(...)`
  - Calls `GetSetting(...)`
  - Calls `AsString(...)`
  - Calls `GetProjectSettingString(...)`
  - Calls `TryParseOwnerRepoFromUrl(...)`
  - Calls `MacOsUpdateInstaller(...)`
  - Calls `WindowsUpdateInstaller(...)`
  - Calls `LinuxUpdateInstaller(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetActionLabel()`

- Signature: `public string GetActionLabel()`
- Source range: `scripts/updates/UpdateManager.cs:92`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetLatestVersionLabel()`

- Signature: `public string GetLatestVersionLabel()`
- Source range: `scripts/updates/UpdateManager.cs:99`
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

### `CheckForUpdates(bool manual = false)`

- Signature: `public void CheckForUpdates(bool manual = false)`
- Source range: `scripts/updates/UpdateManager.cs:104`
- Inputs:
  - `bool manual = false`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `CheckForUpdatesAsync(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `CheckForUpdatesAsync(bool manual = false)`

- Signature: `public async Task CheckForUpdatesAsync(bool manual = false)`
- Source range: `scripts/updates/UpdateManager.cs:109`
- Inputs:
  - `bool manual = false`
- Output / side effects:
  - Emits signals to notify other systems.
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `SetState(...)`
  - Calls `FetchLatestReleaseAsync(...)`
  - Calls `GetUnixTimeFromSystem(...)`
  - Calls `SetError(...)`
  - Calls `SaveUpdaterConfig(...)`
  - Calls `IsNewer(...)`
  - Calls `date(...)`
  - Calls `ResolvePlatformAsset(...)`
  - Calls `RequiresReleaseAsset(...)`
  - Calls `RequiresDigestValidation(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `PerformUpdateAction()`

- Signature: `public void PerformUpdateAction()`
- Source range: `scripts/updates/UpdateManager.cs:178`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `PerformUpdateActionAsync(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PerformUpdateActionAsync()`

- Signature: `public async Task PerformUpdateActionAsync()`
- Source range: `scripts/updates/UpdateManager.cs:183`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `SetState(...)`
  - Calls `TryGetValue(...)`
  - Calls `SetError(...)`
  - Calls `IsDesktopInstaller(...)`
  - Calls `GetDownloadFileName(...)`
  - Calls `SaveUpdaterConfig(...)`
  - Calls `ResolvePlatformAsset(...)`
  - Calls `ExecuteAsync(...)`
  - Calls `GetTree(...)`
  - Calls `Quit(...)`
- Failure paths:
  - Returns explicit failure state/error code on invalid conditions.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `SkipCurrentUpdate()`

- Signature: `public void SkipCurrentUpdate()`
- Source range: `scripts/updates/UpdateManager.cs:244`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `SaveUpdaterConfig(...)`
  - Calls `SetState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `AutoCheckIfNeededAsync()`

- Signature: `private async Task AutoCheckIfNeededAsync()`
- Source range: `scripts/updates/UpdateManager.cs:256`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetUnixTimeFromSystem(...)`
  - Calls `CheckForUpdatesAsync(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `SetState(UpdateState state, string message)`

- Signature: `private void SetState(UpdateState state, string message)`
- Source range: `scripts/updates/UpdateManager.cs:267`
- Inputs:
  - `UpdateState state, string message`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
  - Calls `ToString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetError(UpdateIssueCode code, string message)`

- Signature: `private void SetError(UpdateIssueCode code, string message)`
- Source range: `scripts/updates/UpdateManager.cs:274`
- Inputs:
  - `UpdateIssueCode code, string message`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
  - Calls `ToString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ResolvePlatformAsset(UpdateReleaseInfo release, UpdatePlatform platform)`

- Signature: `private static UpdateAssetInfo? ResolvePlatformAsset(UpdateReleaseInfo release, UpdatePlatform platform)`
- Source range: `scripts/updates/UpdateManager.cs:282`
- Inputs:
  - `UpdateReleaseInfo release, UpdatePlatform platform`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `FindAssetByName(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `FindAssetByName(UpdateReleaseInfo release, string expectedName)`

- Signature: `private static UpdateAssetInfo? FindAssetByName(UpdateReleaseInfo release, string expectedName)`
- Source range: `scripts/updates/UpdateManager.cs:307`
- Inputs:
  - `UpdateReleaseInfo release, string expectedName`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Equals(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `LoadUpdaterConfig()`

- Signature: `private void LoadUpdaterConfig()`
- Source range: `scripts/updates/UpdateManager.cs:320`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ConfigFile(...)`
  - Calls `Load(...)`
  - Calls `GetValue(...)`
  - Calls `AsDouble(...)`
  - Calls `AsString(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SaveUpdaterConfig()`

- Signature: `private void SaveUpdaterConfig()`
- Source range: `scripts/updates/UpdateManager.cs:334`
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

### `DetectRuntimePlatform()`

- Signature: `private static UpdatePlatform DetectRuntimePlatform()`
- Source range: `scripts/updates/UpdateManager.cs:344`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `HasFeature(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `TryParseOwnerRepoFromUrl(string repositoryUrl, out string owner, out string repo)`

- Signature: `private static bool TryParseOwnerRepoFromUrl(string repositoryUrl, out string owner, out string repo)`
- Source range: `scripts/updates/UpdateManager.cs:374`
- Inputs:
  - `string repositoryUrl, out string owner, out string repo`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `TryCreate(...)`
  - Calls `Trim(...)`
  - Calls `Split(...)`
  - Calls `Replace(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetProjectSettingString(string key, string fallback)`

- Signature: `private static string GetProjectSettingString(string key, string fallback)`
- Source range: `scripts/updates/UpdateManager.cs:395`
- Inputs:
  - `string key, string fallback`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `HasSetting(...)`
  - Calls `GetSetting(...)`
  - Calls `AsString(...)`
  - Calls `IsNullOrWhiteSpace(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `IsDesktopInstaller(UpdatePlatform platform)`

- Signature: `private static bool IsDesktopInstaller(UpdatePlatform platform)`
- Source range: `scripts/updates/UpdateManager.cs:406`
- Inputs:
  - `UpdatePlatform platform`
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

### `RequiresReleaseAsset(UpdatePlatform platform)`

- Signature: `private static bool RequiresReleaseAsset(UpdatePlatform platform)`
- Source range: `scripts/updates/UpdateManager.cs:411`
- Inputs:
  - `UpdatePlatform platform`
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

### `RequiresDigestValidation(UpdatePlatform platform)`

- Signature: `private static bool RequiresDigestValidation(UpdatePlatform platform)`
- Source range: `scripts/updates/UpdateManager.cs:416`
- Inputs:
  - `UpdatePlatform platform`
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

### `GetDownloadFileName(UpdatePlatform platform)`

- Signature: `private static string GetDownloadFileName(UpdatePlatform platform)`
- Source range: `scripts/updates/UpdateManager.cs:421`
- Inputs:
  - `UpdatePlatform platform`
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
