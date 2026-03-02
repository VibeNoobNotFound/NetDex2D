# ChecksumVerifier (updates/ChecksumVerifier.cs)

- Source: `scripts/updates/ChecksumVerifier.cs`
- Namespace: `NetDex.Updates`
- Purpose: ChecksumVerifier in `updates/ChecksumVerifier.cs`. GitHub release updater state machine, checksum verification, and installers.

## Dependencies

- Key imports:
  - `Godot`
  - `System`
  - `System.IO`
  - `System.Security.Cryptography`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `TryParseSha256Digest(string digest, out string expectedHex)` | `bool` | Public entry point |
| `ComputeSha256Hex(string godotPath)` | `string` | Public entry point |
| `VerifyFileSha256(string godotPath, string expectedHex, out string actualHex)` | `bool` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `TryParseSha256Digest(string digest, out string expectedHex)`

- Signature: `public static bool TryParseSha256Digest(string digest, out string expectedHex)`
- Source range: `scripts/updates/ChecksumVerifier.cs:10`
- Inputs:
  - `string digest, out string expectedHex`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `StartsWith(...)`
  - Calls `Trim(...)`
  - Calls `IsHexDigit(...)`
  - Calls `ToLowerInvariant(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ComputeSha256Hex(string godotPath)`

- Signature: `public static string ComputeSha256Hex(string godotPath)`
- Source range: `scripts/updates/ChecksumVerifier.cs:42`
- Inputs:
  - `string godotPath`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GlobalizePath(...)`
  - Calls `Exists(...)`
  - Calls `FileNotFoundException(...)`
  - Calls `OpenRead(...)`
  - Calls `Create(...)`
  - Calls `ComputeHash(...)`
  - Calls `ToHexString(...)`
  - Calls `ToLowerInvariant(...)`
- Failure paths:
  - Has exception handling/throw path.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `VerifyFileSha256(string godotPath, string expectedHex, out string actualHex)`

- Signature: `public static bool VerifyFileSha256(string godotPath, string expectedHex, out string actualHex)`
- Source range: `scripts/updates/ChecksumVerifier.cs:56`
- Inputs:
  - `string godotPath, string expectedHex, out string actualHex`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ComputeSha256Hex(...)`
  - Calls `Equals(...)`
  - Calls `Trim(...)`
  - Calls `ToLowerInvariant(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
