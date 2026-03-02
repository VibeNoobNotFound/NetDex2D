# MacOsUpdateInstaller (updates/installers/MacOsUpdateInstaller.cs)

- Source: `scripts/updates/installers/MacOsUpdateInstaller.cs`
- Namespace: `NetDex.Updates.Installers`
- Purpose: MacOsUpdateInstaller in `updates/installers/MacOsUpdateInstaller.cs`. GitHub release updater state machine, checksum verification, and installers.

## Dependencies

- Key imports:
  - `Godot`
  - `System`
  - `System.IO`
  - `System.Text`
  - `System.Threading.Tasks`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `ExecuteAsync(Node host, UpdateReleaseInfo release, UpdateAssetInfo? platformAsset)` | `Task<UpdateOperationResult>` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `EnsureUpdateDirectory()` | `private` | `UpdateOperationResult?` |
| `DownloadFileAsync(Node host, string url, string targetGodotPath)` | `private` | `Task<UpdateOperationResult>` |
| `MoveTempDownloadToFinalPath(out string message)` | `private` | `bool` |
| `WriteUpdateScript(out string message)` | `private` | `bool` |
| `ResolveCurrentAppBundlePath()` | `private` | `string` |
| `BuildScript(string appBundlePath, string zipPath)` | `private` | `string` |
| `ShellEscape(string input)` | `private` | `string` |

## Function-by-Function

### `ExecuteAsync(Node host, UpdateReleaseInfo release, UpdateAssetInfo? platformAsset)`

- Signature: `public async Task<UpdateOperationResult> ExecuteAsync(Node host, UpdateReleaseInfo release, UpdateAssetInfo? platformAsset)`
- Source range: `scripts/updates/installers/MacOsUpdateInstaller.cs:19`
- Inputs:
  - `Node host, UpdateReleaseInfo release, UpdateAssetInfo? platformAsset`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `UpdateOperationResult(...)`
  - Calls `TryParseSha256Digest(...)`
  - Calls `EnsureUpdateDirectory(...)`
  - Calls `DownloadFileAsync(...)`
  - Calls `VerifyFileSha256(...)`
  - Calls `MoveTempDownloadToFinalPath(...)`
  - Calls `WriteUpdateScript(...)`
  - Calls `GlobalizePath(...)`
  - Calls `CreateProcess(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `EnsureUpdateDirectory()`

- Signature: `private static UpdateOperationResult? EnsureUpdateDirectory()`
- Source range: `scripts/updates/installers/MacOsUpdateInstaller.cs:70`
- Inputs:
  - `none`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GlobalizePath(...)`
  - Calls `CreateDirectory(...)`
  - Calls `UpdateOperationResult(...)`
- Failure paths:
  - Has exception handling/throw path.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `DownloadFileAsync(Node host, string url, string targetGodotPath)`

- Signature: `private static async Task<UpdateOperationResult> DownloadFileAsync(Node host, string url, string targetGodotPath)`
- Source range: `scripts/updates/installers/MacOsUpdateInstaller.cs:84`
- Inputs:
  - `Node host, string url, string targetGodotPath`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `HttpRequest(...)`
  - Calls `AddChild(...)`
  - Calls `GlobalizePath(...)`
  - Calls `Request(...)`
  - Calls `QueueFree(...)`
  - Calls `UpdateOperationResult(...)`
  - Calls `ToSignal(...)`
  - Calls `AsInt32(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `MoveTempDownloadToFinalPath(out string message)`

- Signature: `private static bool MoveTempDownloadToFinalPath(out string message)`
- Source range: `scripts/updates/installers/MacOsUpdateInstaller.cs:122`
- Inputs:
  - `out string message`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GlobalizePath(...)`
  - Calls `Exists(...)`
  - Calls `Delete(...)`
  - Calls `Move(...)`
- Failure paths:
  - Has exception handling/throw path.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `WriteUpdateScript(out string message)`

- Signature: `private static bool WriteUpdateScript(out string message)`
- Source range: `scripts/updates/installers/MacOsUpdateInstaller.cs:145`
- Inputs:
  - `out string message`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `ResolveCurrentAppBundlePath(...)`
  - Calls `GlobalizePath(...)`
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `BuildScript(...)`
  - Calls `WriteAllText(...)`
- Failure paths:
  - Has exception handling/throw path.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ResolveCurrentAppBundlePath()`

- Signature: `private static string ResolveCurrentAppBundlePath()`
- Source range: `scripts/updates/installers/MacOsUpdateInstaller.cs:172`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `GetExecutablePath(...)`
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `IndexOf(...)`
  - Calls `EndsWith(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildScript(string appBundlePath, string zipPath)`

- Signature: `private static string BuildScript(string appBundlePath, string zipPath)`
- Source range: `scripts/updates/installers/MacOsUpdateInstaller.cs:194`
- Inputs:
  - `string appBundlePath, string zipPath`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ShellEscape(...)`
  - Calls `cleanup(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ShellEscape(string input)`

- Signature: `private static string ShellEscape(string input)`
- Source range: `scripts/updates/installers/MacOsUpdateInstaller.cs:223`
- Inputs:
  - `string input`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Replace(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
