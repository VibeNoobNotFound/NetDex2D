# GitHubReleaseProvider (updates/GitHubReleaseProvider.cs)

- Source: `scripts/updates/GitHubReleaseProvider.cs`
- Namespace: `NetDex.Updates`
- Purpose: GitHubReleaseProvider in `updates/GitHubReleaseProvider.cs`. GitHub release updater state machine, checksum verification, and installers.

## Dependencies

- Key imports:
  - `Godot`
  - `System.Collections.Generic`
  - `System.Text`
  - `System.Threading.Tasks`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `FetchLatestReleaseAsync(Node host, string owner, string repo)` | `Task<ReleaseQueryResult>` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `FetchLatestReleaseAsync(Node host, string owner, string repo)`

- Signature: `public async Task<ReleaseQueryResult> FetchLatestReleaseAsync(Node host, string owner, string repo)`
- Source range: `scripts/updates/GitHubReleaseProvider.cs:10`
- Inputs:
  - `Node host, string owner, string repo`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `HttpRequest(...)`
  - Calls `AddChild(...)`
  - Calls `Request(...)`
  - Calls `QueueFree(...)`
  - Calls `ReleaseQueryResult(...)`
  - Calls `ToSignal(...)`
  - Calls `AsInt32(...)`
  - Calls `AsByteArray(...)`
  - Calls `GetString(...)`
  - Calls `ParseString(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.
