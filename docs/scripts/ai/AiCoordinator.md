# AiCoordinator (ai/AiCoordinator.cs)

- Source: `scripts/ai/AiCoordinator.cs`
- Namespace: `NetDex.AI`
- Purpose: AiCoordinator in `ai/AiCoordinator.cs`. AI decision-making, imperfect-information perception, and bot action scheduling.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Core.Commands`
  - `NetDex.Core.Enums`
  - `NetDex.Core.Models`
  - `NetDex.Core.Rules`
  - `NetDex.Lobby`
  - `System`
  - `System.Collections.Generic`
  - `System.Threading`
  - `System.Threading.Tasks`
- Autoload/manager dependencies detected:
  - `LobbyManager`
  - `MatchCoordinator`
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `_ExitTree()` | `void` | Public entry point |
| `_Process(double delta)` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `OnMatchStateAdvanced()` | `private` | `void` |
| `OnRoomStateChanged()` | `private` | `void` |
| `ScheduleIfNeeded()` | `private` | `void` |
| `BuildPerception(OmiMatchState state, int botPeerId, AiDifficulty difficulty, int stateVersion)` | `private` | `BotPerceptionState` |
| `StartSearch(BotPerceptionState perception, int stateVersion)` | `private` | `void` |
| `PollSearchCompletion()` | `private` | `void` |
| `TryExecutePendingDecision()` | `private` | `void` |
| `IsDecisionStillValid(PendingDecision decision)` | `private` | `bool` |
| `CancelActiveSearch()` | `private` | `void` |
| `BuildDecisionSeed(OmiMatchState state, int stateVersion, int peerId)` | `private` | `int` |
| `ResolveDelaySeconds(MatchCommand command)` | `private` | `double` |
| `GetShuffleCount(int roundNumber, SeatPosition seat)` | `private` | `int` |
| `IncrementShuffleCount(int roundNumber, SeatPosition seat)` | `private` | `void` |
| `ResetShuffleCount(int roundNumber, SeatPosition seat)` | `private` | `void` |
| `PruneShuffleCounters(int activeRound)` | `private` | `void` |
| `BuildShuffleKey(int roundNumber, SeatPosition seat)` | `private` | `string` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/ai/AiCoordinator.cs:35`
- Inputs:
  - `none`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `QueueFree(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `_ExitTree()`

- Signature: `public override void _ExitTree()`
- Source range: `scripts/ai/AiCoordinator.cs:49`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `CancelActiveSearch(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `_Process(double delta)`

- Signature: `public override void _Process(double delta)`
- Source range: `scripts/ai/AiCoordinator.cs:64`
- Inputs:
  - `double delta`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `PollSearchCompletion(...)`
  - Calls `TryExecutePendingDecision(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnMatchStateAdvanced()`

- Signature: `private void OnMatchStateAdvanced()`
- Source range: `scripts/ai/AiCoordinator.cs:70`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ScheduleIfNeeded(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnRoomStateChanged()`

- Signature: `private void OnRoomStateChanged()`
- Source range: `scripts/ai/AiCoordinator.cs:75`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ScheduleIfNeeded(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ScheduleIfNeeded()`

- Signature: `private void ScheduleIfNeeded()`
- Source range: `scripts/ai/AiCoordinator.cs:80`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `CancelActiveSearch(...)`
  - Calls `GetAuthoritativeState(...)`
  - Calls `PruneShuffleCounters(...)`
  - Calls `TryGetValue(...)`
  - Calls `BuildPerception(...)`
  - Calls `StartSearch(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `BuildPerception(OmiMatchState state, int botPeerId, AiDifficulty difficulty, int stateVersion)`

- Signature: `private BotPerceptionState BuildPerception(OmiMatchState state, int botPeerId, AiDifficulty difficulty, int stateVersion)`
- Source range: `scripts/ai/AiCoordinator.cs:145`
- Inputs:
  - `OmiMatchState state, int botPeerId, AiDifficulty difficulty, int stateVersion`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `CloneState(...)`
  - Calls `GetValues(...)`
  - Calls `Clear(...)`
  - Calls `BuildFromTricks(...)`
  - Calls `BuildDecisionSeed(...)`
  - Calls `GetShuffleCount(...)`
  - Calls `Max(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `StartSearch(BotPerceptionState perception, int stateVersion)`

- Signature: `private void StartSearch(BotPerceptionState perception, int stateVersion)`
- Source range: `scripts/ai/AiCoordinator.cs:176`
- Inputs:
  - `BotPerceptionState perception, int stateVersion`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `CancelActiveSearch(...)`
  - Calls `CancellationTokenSource(...)`
  - Calls `Run(...)`
  - Calls `ChooseCommand(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.
  - AI threading path present; avoid touching scene tree from background execution.

### `PollSearchCompletion()`

- Signature: `private void PollSearchCompletion()`
- Source range: `scripts/ai/AiCoordinator.cs:187`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `PrintErr(...)`
  - Calls `GetBaseException(...)`
  - Calls `GetAuthoritativeState(...)`
  - Calls `ResolveDelaySeconds(...)`
  - Calls `PendingDecision(...)`
  - Calls `GetUnixTimeFromSystem(...)`
- Failure paths:
  - Has exception handling/throw path.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `TryExecutePendingDecision()`

- Signature: `private void TryExecutePendingDecision()`
- Source range: `scripts/ai/AiCoordinator.cs:238`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetUnixTimeFromSystem(...)`
  - Calls `IsDecisionStillValid(...)`
  - Calls `ScheduleIfNeeded(...)`
  - Calls `ServerHandleAiCommand(...)`
  - Calls `IncrementShuffleCount(...)`
  - Calls `ResetShuffleCount(...)`
  - Calls `PrintErr(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `IsDecisionStillValid(PendingDecision decision)`

- Signature: `private bool IsDecisionStillValid(PendingDecision decision)`
- Source range: `scripts/ai/AiCoordinator.cs:277`
- Inputs:
  - `PendingDecision decision`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetAuthoritativeState(...)`
  - Calls `TryGetValue(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `CancelActiveSearch()`

- Signature: `private void CancelActiveSearch()`
- Source range: `scripts/ai/AiCoordinator.cs:319`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Cancel(...)`
  - Calls `Dispose(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `BuildDecisionSeed(OmiMatchState state, int stateVersion, int peerId)`

- Signature: `private static int BuildDecisionSeed(OmiMatchState state, int stateVersion, int peerId)`
- Source range: `scripts/ai/AiCoordinator.cs:340`
- Inputs:
  - `OmiMatchState state, int stateVersion, int peerId`
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

### `ResolveDelaySeconds(MatchCommand command)`

- Signature: `private static double ResolveDelaySeconds(MatchCommand command)`
- Source range: `scripts/ai/AiCoordinator.cs:355`
- Inputs:
  - `MatchCommand command`
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

### `GetShuffleCount(int roundNumber, SeatPosition seat)`

- Signature: `private int GetShuffleCount(int roundNumber, SeatPosition seat)`
- Source range: `scripts/ai/AiCoordinator.cs:369`
- Inputs:
  - `int roundNumber, SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `BuildShuffleKey(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `IncrementShuffleCount(int roundNumber, SeatPosition seat)`

- Signature: `private void IncrementShuffleCount(int roundNumber, SeatPosition seat)`
- Source range: `scripts/ai/AiCoordinator.cs:374`
- Inputs:
  - `int roundNumber, SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `BuildShuffleKey(...)`
  - Calls `TryGetValue(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ResetShuffleCount(int roundNumber, SeatPosition seat)`

- Signature: `private void ResetShuffleCount(int roundNumber, SeatPosition seat)`
- Source range: `scripts/ai/AiCoordinator.cs:381`
- Inputs:
  - `int roundNumber, SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Remove(...)`
  - Calls `BuildShuffleKey(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PruneShuffleCounters(int activeRound)`

- Signature: `private void PruneShuffleCounters(int activeRound)`
- Source range: `scripts/ai/AiCoordinator.cs:386`
- Inputs:
  - `int activeRound`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IndexOf(...)`
  - Calls `Add(...)`
  - Calls `TryParse(...)`
  - Calls `Remove(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildShuffleKey(int roundNumber, SeatPosition seat)`

- Signature: `private static string BuildShuffleKey(int roundNumber, SeatPosition seat)`
- Source range: `scripts/ai/AiCoordinator.cs:410`
- Inputs:
  - `int roundNumber, SeatPosition seat`
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
