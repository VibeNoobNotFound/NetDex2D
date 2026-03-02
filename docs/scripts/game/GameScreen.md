# GameScreen (game/GameScreen.cs)

- Source: `scripts/game/GameScreen.cs`
- Namespace: `NetDex.UI.Game`
- Purpose: GameScreen in `game/GameScreen.cs`. Runtime match orchestration and in-game UI rendering/animation logic.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Core.Config`
  - `NetDex.Core.Enums`
  - `NetDex.Core.Models`
  - `NetDex.Core.Serialization`
  - `NetDex.Lobby`
  - `NetDex.Managers`
  - `NetDex.Networking`
  - `System`
  - `System.Collections.Generic`
  - `System.Linq`
  - `System.Threading.Tasks`
- Autoload/manager dependencies detected:
  - `GameManager`
  - `LobbyManager`
  - `NetworkRpc`
- Scene node paths used: `25`

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `_ExitTree()` | `void` | Public entry point |
| `_Input(InputEvent @event)` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `OnRoomStateChanged()` | `private` | `void` |
| `OnMatchSnapshotChanged()` | `private` | `void` |
| `RefreshLocalIdentity()` | `private` | `void` |
| `RenderSnapshot(Godot.Collections.Dictionary snapshot)` | `private` | `void` |
| `BuildStatusText(OmiPhase phase, int roundNumber, SeatPosition currentTurnSeat, int trumpSuit, int[] teamCredits, int[] teamTricks)` | `private` | `string` |
| `TryStartPhaseAnimation(Godot.Collections.Dictionary snapshot, OmiPhase phase, int trumpSuit)` | `private` | `void` |
| `ExtractPair(Godot.Collections.Dictionary snapshot, string key, int fallbackLeft, int fallbackRight)` | `private` | `int[]` |
| `ParseSeat(Godot.Collections.Dictionary snapshot, string key)` | `private` | `SeatPosition?` |
| `RunDealAnimationSequence(SeatPosition shufflerSeat, SeatPosition dealStartSeat, bool includeTrumpAnnouncement, int trumpSuit)` | `private` | `Task` |
| `ShowTrumpAnnouncementAsync(string selectorName, CardSuit trumpSuit)` | `private` | `Task` |
| `AnimateDealAsync(SeatPosition shufflerSeat, SeatPosition dealStartSeat, int cardsPerSeat)` | `private` | `Task` |
| `SpawnDealCard(SeatPosition shufflerSeat, SeatPosition targetSeat, int seatCardIndex)` | `private` | `void` |
| `GetHandCenterGlobal(SeatPosition seat, Vector2 cardSize)` | `private` | `Vector2` |
| `GetDealSeatOffset(SeatPosition seat, int seatCardIndex)` | `private` | `Vector2` |
| `RenderHands(Godot.Collections.Dictionary snapshot, OmiPhase phase, SeatPosition currentTurnSeat)` | `private` | `void` |
| `RenderTrickState(Godot.Collections.Dictionary snapshot, int roundNumber, SeatPosition currentTurnSeat, int[] teamTricks)` | `private` | `void` |
| `RenderActionPanel(Godot.Collections.Dictionary snapshot, OmiPhase phase)` | `private` | `void` |
| `OnActionButtonPressed()` | `private` | `void` |
| `OnSecondaryActionButtonPressed()` | `private` | `void` |
| `OnCardClicked(Card card)` | `private` | `void` |
| `OnBackLobbyPressed()` | `private` | `void` |
| `OnMobilePausePressed()` | `private` | `void` |
| `TogglePauseMenu()` | `private` | `void` |
| `EnsurePauseMenu()` | `private` | `Control` |
| `CreateCard(CardModel model, bool faceUp, bool interactable, bool playRevealSound = false)` | `private` | `Card` |
| `ClearHands()` | `private` | `void` |
| `ResetBoardVisualState()` | `private` | `void` |
| `ClearTrickAndPileVisuals()` | `private` | `void` |
| `ClearContainer(Node container)` | `private` | `void` |
| `ClearAnimationLayerCards()` | `private` | `void` |
| `GetHandContainer(SeatPosition seat)` | `private` | `Control` |
| `GetDeskCardPosition(SeatPosition seat, Vector2 cardSize)` | `private` | `Vector2` |
| `GetDeskSpawnPosition(SeatPosition seat, Vector2 cardSize)` | `private` | `Vector2` |
| `ToVisualSlot(SeatPosition actualSeat)` | `private` | `VisualSlot` |
| `RenderSeatNames()` | `private` | `void` |
| `ToSeatForVisualSlot(VisualSlot visualSlot)` | `private` | `SeatPosition` |
| `BuildSeatTitle(SeatPosition seat)` | `private` | `string` |
| `ResolveWinnerTeam(int[] teamTricks, SeatPosition currentTurnSeat)` | `private` | `int` |
| `AnimateTrickCollection(int winnerTeam)` | `private` | `void` |
| `UpdatePileCounts(int[] teamTricks)` | `private` | `void` |
| `SyncPileMarkers(int friendlyCount, int enemyCount)` | `private` | `void` |
| `SyncSinglePile(List<Card> pileCards, Control pileContainer, int targetCount)` | `private` | `void` |
| `EnsureCardSize(Card card)` | `private` | `Vector2` |
| `PlaySound(AudioStreamOggVorbis[] sounds)` | `private` | `void` |
| `PlaySound(AudioStream sound)` | `private` | `void` |
| `ToViewSuit(CardSuit suit)` | `private` | `Card.SuitType` |
| `ToViewRank(CardRank rank)` | `private` | `Card.RankType` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/game/GameScreen.cs:78`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `HasFeature(...)`
  - Calls `Clear(...)`
  - Calls `AddItem(...)`
  - Calls `AudioStreamPlayer(...)`
  - Calls `GetSfxBusName(...)`
  - Calls `AddChild(...)`
  - Calls `RefreshLocalIdentity(...)`
  - Calls `RenderSnapshot(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `_ExitTree()`

- Signature: `public override void _ExitTree()`
- Source range: `scripts/game/GameScreen.cs:145`
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

### `_Input(InputEvent @event)`

- Signature: `public override void _Input(InputEvent @event)`
- Source range: `scripts/game/GameScreen.cs:154`
- Inputs:
  - `InputEvent @event`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `IsActionPressed(...)`
  - Calls `TogglePauseMenu(...)`
  - Calls `GetViewport(...)`
  - Calls `SetInputAsHandled(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnRoomStateChanged()`

- Signature: `private void OnRoomStateChanged()`
- Source range: `scripts/game/GameScreen.cs:163`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `RefreshLocalIdentity(...)`
  - Calls `RenderSnapshot(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnMatchSnapshotChanged()`

- Signature: `private void OnMatchSnapshotChanged()`
- Source range: `scripts/game/GameScreen.cs:169`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `RenderSnapshot(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RefreshLocalIdentity()`

- Signature: `private void RefreshLocalIdentity()`
- Source range: `scripts/game/GameScreen.cs:174`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetLocalSeat(...)`
  - Calls `GetLocalRole(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RenderSnapshot(Godot.Collections.Dictionary snapshot)`

- Signature: `private void RenderSnapshot(Godot.Collections.Dictionary snapshot)`
- Source range: `scripts/game/GameScreen.cs:180`
- Inputs:
  - `Godot.Collections.Dictionary snapshot`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `RenderSeatNames(...)`
  - Calls `ResetBoardVisualState(...)`
  - Calls `Dictionary(...)`
  - Calls `TryGetValue(...)`
  - Calls `AsInt32(...)`
  - Calls `Parse(...)`
  - Calls `AsString(...)`
  - Calls `ExtractPair(...)`
  - Calls `BuildStatusText(...)`
  - Calls `TryStartPhaseAnimation(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildStatusText(OmiPhase phase, int roundNumber, SeatPosition currentTurnSeat, int trumpSuit, int[] teamCredits, int[] teamTricks)`

- Signature: `private string BuildStatusText(OmiPhase phase, int roundNumber, SeatPosition currentTurnSeat, int trumpSuit, int[] teamCredits, int[] teamTricks)`
- Source range: `scripts/game/GameScreen.cs:230`
- Inputs:
  - `OmiPhase phase, int roundNumber, SeatPosition currentTurnSeat, int trumpSuit, int[] teamCredits, int[] teamTricks`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ToString(...)`
  - Calls `BuildSeatTitle(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `TryStartPhaseAnimation(Godot.Collections.Dictionary snapshot, OmiPhase phase, int trumpSuit)`

- Signature: `private void TryStartPhaseAnimation(Godot.Collections.Dictionary snapshot, OmiPhase phase, int trumpSuit)`
- Source range: `scripts/game/GameScreen.cs:246`
- Inputs:
  - `Godot.Collections.Dictionary snapshot, OmiPhase phase, int trumpSuit`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ParseSeat(...)`
  - Calls `RunDealAnimationSequence(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ExtractPair(Godot.Collections.Dictionary snapshot, string key, int fallbackLeft, int fallbackRight)`

- Signature: `private static int[] ExtractPair(Godot.Collections.Dictionary snapshot, string key, int fallbackLeft, int fallbackRight)`
- Source range: `scripts/game/GameScreen.cs:272`
- Inputs:
  - `Godot.Collections.Dictionary snapshot, string key, int fallbackLeft, int fallbackRight`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `AsGodotArray(...)`
  - Calls `AsInt32(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ParseSeat(Godot.Collections.Dictionary snapshot, string key)`

- Signature: `private static SeatPosition? ParseSeat(Godot.Collections.Dictionary snapshot, string key)`
- Source range: `scripts/game/GameScreen.cs:285`
- Inputs:
  - `Godot.Collections.Dictionary snapshot, string key`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `Parse(...)`
  - Calls `AsString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RunDealAnimationSequence(SeatPosition shufflerSeat, SeatPosition dealStartSeat, bool includeTrumpAnnouncement, int trumpSuit)`

- Signature: `private async Task RunDealAnimationSequence(SeatPosition shufflerSeat, SeatPosition dealStartSeat, bool includeTrumpAnnouncement, int trumpSuit)`
- Source range: `scripts/game/GameScreen.cs:292`
- Inputs:
  - `SeatPosition shufflerSeat, SeatPosition dealStartSeat, bool includeTrumpAnnouncement, int trumpSuit`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `BuildSeatTitle(...)`
  - Calls `ShowTrumpAnnouncementAsync(...)`
  - Calls `AnimateDealAsync(...)`
  - Calls `Dictionary(...)`
  - Calls `RenderSnapshot(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `ShowTrumpAnnouncementAsync(string selectorName, CardSuit trumpSuit)`

- Signature: `private async Task ShowTrumpAnnouncementAsync(string selectorName, CardSuit trumpSuit)`
- Source range: `scripts/game/GameScreen.cs:319`
- Inputs:
  - `string selectorName, CardSuit trumpSuit`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Color(...)`
  - Calls `Vector2(...)`
  - Calls `CreateTween(...)`
  - Calls `TweenProperty(...)`
  - Calls `SetTrans(...)`
  - Calls `SetEase(...)`
  - Calls `Parallel(...)`
  - Calls `ToSignal(...)`
  - Calls `GetTree(...)`
  - Calls `CreateTimer(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `AnimateDealAsync(SeatPosition shufflerSeat, SeatPosition dealStartSeat, int cardsPerSeat)`

- Signature: `private async Task AnimateDealAsync(SeatPosition shufflerSeat, SeatPosition dealStartSeat, int cardsPerSeat)`
- Source range: `scripts/game/GameScreen.cs:348`
- Inputs:
  - `SeatPosition shufflerSeat, SeatPosition dealStartSeat, int cardsPerSeat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `OrderedFrom(...)`
  - Calls `GetValues(...)`
  - Calls `SpawnDealCard(...)`
  - Calls `PlaySound(...)`
  - Calls `ToSignal(...)`
  - Calls `GetTree(...)`
  - Calls `CreateTimer(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Async-sensitive: ensure calls are coordinated with Godot main-thread scene operations.

### `SpawnDealCard(SeatPosition shufflerSeat, SeatPosition targetSeat, int seatCardIndex)`

- Signature: `private void SpawnDealCard(SeatPosition shufflerSeat, SeatPosition targetSeat, int seatCardIndex)`
- Source range: `scripts/game/GameScreen.cs:371`
- Inputs:
  - `SeatPosition shufflerSeat, SeatPosition targetSeat, int seatCardIndex`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `CreateCard(...)`
  - Calls `CardModel(...)`
  - Calls `GetTicksMsec(...)`
  - Calls `AddChild(...)`
  - Calls `EnsureCardSize(...)`
  - Calls `GetHandCenterGlobal(...)`
  - Calls `GetDealSeatOffset(...)`
  - Calls `CreateTween(...)`
  - Calls `TweenProperty(...)`
  - Calls `SetTrans(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetHandCenterGlobal(SeatPosition seat, Vector2 cardSize)`

- Signature: `private Vector2 GetHandCenterGlobal(SeatPosition seat, Vector2 cardSize)`
- Source range: `scripts/game/GameScreen.cs:388`
- Inputs:
  - `SeatPosition seat, Vector2 cardSize`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetHandContainer(...)`
  - Calls `GetGlobalRect(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetDealSeatOffset(SeatPosition seat, int seatCardIndex)`

- Signature: `private Vector2 GetDealSeatOffset(SeatPosition seat, int seatCardIndex)`
- Source range: `scripts/game/GameScreen.cs:394`
- Inputs:
  - `SeatPosition seat, int seatCardIndex`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ToVisualSlot(...)`
  - Calls `Vector2(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RenderHands(Godot.Collections.Dictionary snapshot, OmiPhase phase, SeatPosition currentTurnSeat)`

- Signature: `private void RenderHands(Godot.Collections.Dictionary snapshot, OmiPhase phase, SeatPosition currentTurnSeat)`
- Source range: `scripts/game/GameScreen.cs:407`
- Inputs:
  - `Godot.Collections.Dictionary snapshot, OmiPhase phase, SeatPosition currentTurnSeat`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `AsGodotDictionary(...)`
  - Calls `Dictionary(...)`
  - Calls `GetValues(...)`
  - Calls `GetHandContainer(...)`
  - Calls `Array(...)`
  - Calls `ToString(...)`
  - Calls `AsGodotArray(...)`
  - Calls `AsInt32(...)`
  - Calls `FromDictionary(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RenderTrickState(Godot.Collections.Dictionary snapshot, int roundNumber, SeatPosition currentTurnSeat, int[] teamTricks)`

- Signature: `private void RenderTrickState(Godot.Collections.Dictionary snapshot, int roundNumber, SeatPosition currentTurnSeat, int[] teamTricks)`
- Source range: `scripts/game/GameScreen.cs:457`
- Inputs:
  - `Godot.Collections.Dictionary snapshot, int roundNumber, SeatPosition currentTurnSeat, int[] teamTricks`
- Output / side effects:
  - Mutates local object state/fields.
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `ClearTrickAndPileVisuals(...)`
  - Calls `TryGetValue(...)`
  - Calls `UpdatePileCounts(...)`
  - Calls `AsInt32(...)`
  - Calls `ResolveWinnerTeam(...)`
  - Calls `AnimateTrickCollection(...)`
  - Calls `AsGodotArray(...)`
  - Calls `AsGodotDictionary(...)`
  - Calls `Parse(...)`
  - Calls `AsString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RenderActionPanel(Godot.Collections.Dictionary snapshot, OmiPhase phase)`

- Signature: `private void RenderActionPanel(Godot.Collections.Dictionary snapshot, OmiPhase phase)`
- Source range: `scripts/game/GameScreen.cs:545`
- Inputs:
  - `Godot.Collections.Dictionary snapshot, OmiPhase phase`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `Parse(...)`
  - Calls `AsString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnActionButtonPressed()`

- Signature: `private void OnActionButtonPressed()`
- Source range: `scripts/game/GameScreen.cs:607`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `AsInt32(...)`
  - Calls `SendShuffleAgainRequest(...)`
  - Calls `Randi(...)`
  - Calls `SendCutDeckRequest(...)`
  - Calls `RandRange(...)`
  - Calls `SendSelectTrumpRequest(...)`
  - Calls `GetSelectedId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnSecondaryActionButtonPressed()`

- Signature: `private void OnSecondaryActionButtonPressed()`
- Source range: `scripts/game/GameScreen.cs:637`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `AsInt32(...)`
  - Calls `SendFinishShuffleRequest(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnCardClicked(Card card)`

- Signature: `private void OnCardClicked(Card card)`
- Source range: `scripts/game/GameScreen.cs:655`
- Inputs:
  - `Card card`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `StartsWith(...)`
  - Calls `SendPlayCardRequest(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnBackLobbyPressed()`

- Signature: `private static void OnBackLobbyPressed()`
- Source range: `scripts/game/GameScreen.cs:670`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `LoadLobby(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnMobilePausePressed()`

- Signature: `private void OnMobilePausePressed()`
- Source range: `scripts/game/GameScreen.cs:675`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `TogglePauseMenu(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `TogglePauseMenu()`

- Signature: `private void TogglePauseMenu()`
- Source range: `scripts/game/GameScreen.cs:680`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `EnsurePauseMenu(...)`
  - Calls `Show(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `EnsurePauseMenu()`

- Signature: `private Control EnsurePauseMenu()`
- Source range: `scripts/game/GameScreen.cs:693`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `AddChild(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `CreateCard(CardModel model, bool faceUp, bool interactable, bool playRevealSound = false)`

- Signature: `private Card CreateCard(CardModel model, bool faceUp, bool interactable, bool playRevealSound = false)`
- Source range: `scripts/game/GameScreen.cs:702`
- Inputs:
  - `CardModel model, bool faceUp, bool interactable, bool playRevealSound = false`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Setup(...)`
  - Calls `ToViewSuit(...)`
  - Calls `ToViewRank(...)`
  - Calls `SetInteractable(...)`
  - Calls `PlaySound(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ClearHands()`

- Signature: `private void ClearHands()`
- Source range: `scripts/game/GameScreen.cs:721`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ClearContainer(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ResetBoardVisualState()`

- Signature: `private void ResetBoardVisualState()`
- Source range: `scripts/game/GameScreen.cs:729`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ClearTrickAndPileVisuals(...)`
  - Calls `Dictionary(...)`
  - Calls `ClearAnimationLayerCards(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ClearTrickAndPileVisuals()`

- Signature: `private void ClearTrickAndPileVisuals()`
- Source range: `scripts/game/GameScreen.cs:746`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
  - Changes node lifecycle/application flow.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `QueueFree(...)`
  - Calls `Clear(...)`
  - Calls `ClearContainer(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ClearContainer(Node container)`

- Signature: `private static void ClearContainer(Node container)`
- Source range: `scripts/game/GameScreen.cs:773`
- Inputs:
  - `Node container`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetChildren(...)`
  - Calls `QueueFree(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ClearAnimationLayerCards()`

- Signature: `private void ClearAnimationLayerCards()`
- Source range: `scripts/game/GameScreen.cs:781`
- Inputs:
  - `none`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetChildren(...)`
  - Calls `QueueFree(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetHandContainer(SeatPosition seat)`

- Signature: `private Control GetHandContainer(SeatPosition seat)`
- Source range: `scripts/game/GameScreen.cs:794`
- Inputs:
  - `SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ToVisualSlot(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetDeskCardPosition(SeatPosition seat, Vector2 cardSize)`

- Signature: `private Vector2 GetDeskCardPosition(SeatPosition seat, Vector2 cardSize)`
- Source range: `scripts/game/GameScreen.cs:806`
- Inputs:
  - `SeatPosition seat, Vector2 cardSize`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ToVisualSlot(...)`
  - Calls `Vector2(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetDeskSpawnPosition(SeatPosition seat, Vector2 cardSize)`

- Signature: `private Vector2 GetDeskSpawnPosition(SeatPosition seat, Vector2 cardSize)`
- Source range: `scripts/game/GameScreen.cs:819`
- Inputs:
  - `SeatPosition seat, Vector2 cardSize`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetHandContainer(...)`
  - Calls `GetGlobalRect(...)`
  - Calls `GetGlobalTransformWithCanvas(...)`
  - Calls `AffineInverse(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ToVisualSlot(SeatPosition actualSeat)`

- Signature: `private VisualSlot ToVisualSlot(SeatPosition actualSeat)`
- Source range: `scripts/game/GameScreen.cs:827`
- Inputs:
  - `SeatPosition actualSeat`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RenderSeatNames()`

- Signature: `private void RenderSeatNames()`
- Source range: `scripts/game/GameScreen.cs:837`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `GetValues(...)`
  - Calls `ToString(...)`
  - Calls `TryGetValue(...)`
  - Calls `ToSeatForVisualSlot(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ToSeatForVisualSlot(VisualSlot visualSlot)`

- Signature: `private SeatPosition ToSeatForVisualSlot(VisualSlot visualSlot)`
- Source range: `scripts/game/GameScreen.cs:873`
- Inputs:
  - `VisualSlot visualSlot`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildSeatTitle(SeatPosition seat)`

- Signature: `private string BuildSeatTitle(SeatPosition seat)`
- Source range: `scripts/game/GameScreen.cs:882`
- Inputs:
  - `SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `ToString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ResolveWinnerTeam(int[] teamTricks, SeatPosition currentTurnSeat)`

- Signature: `private int ResolveWinnerTeam(int[] teamTricks, SeatPosition currentTurnSeat)`
- Source range: `scripts/game/GameScreen.cs:899`
- Inputs:
  - `int[] teamTricks, SeatPosition currentTurnSeat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `TeamIndex(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `AnimateTrickCollection(int winnerTeam)`

- Signature: `private void AnimateTrickCollection(int winnerTeam)`
- Source range: `scripts/game/GameScreen.cs:914`
- Inputs:
  - `int winnerTeam`
- Output / side effects:
  - Mutates local object state/fields.
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ToList(...)`
  - Calls `Clear(...)`
  - Calls `TeamIndex(...)`
  - Calls `Vector2(...)`
  - Calls `CreateTween(...)`
  - Calls `SetParallel(...)`
  - Calls `TweenProperty(...)`
  - Calls `SetTrans(...)`
  - Calls `SetEase(...)`
  - Calls `PlaySound(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `UpdatePileCounts(int[] teamTricks)`

- Signature: `private void UpdatePileCounts(int[] teamTricks)`
- Source range: `scripts/game/GameScreen.cs:956`
- Inputs:
  - `int[] teamTricks`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `TeamIndex(...)`
  - Calls `SyncPileMarkers(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SyncPileMarkers(int friendlyCount, int enemyCount)`

- Signature: `private void SyncPileMarkers(int friendlyCount, int enemyCount)`
- Source range: `scripts/game/GameScreen.cs:973`
- Inputs:
  - `int friendlyCount, int enemyCount`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SyncSinglePile(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SyncSinglePile(List<Card> pileCards, Control pileContainer, int targetCount)`

- Signature: `private void SyncSinglePile(List<Card> pileCards, Control pileContainer, int targetCount)`
- Source range: `scripts/game/GameScreen.cs:979`
- Inputs:
  - `List<Card> pileCards, Control pileContainer, int targetCount`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `RemoveAt(...)`
  - Calls `QueueFree(...)`
  - Calls `CreateCard(...)`
  - Calls `CardModel(...)`
  - Calls `AddChild(...)`
  - Calls `Vector2(...)`
  - Calls `Add(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `EnsureCardSize(Card card)`

- Signature: `private static Vector2 EnsureCardSize(Card card)`
- Source range: `scripts/game/GameScreen.cs:1003`
- Inputs:
  - `Card card`
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

### `PlaySound(AudioStreamOggVorbis[] sounds)`

- Signature: `private void PlaySound(AudioStreamOggVorbis[] sounds)`
- Source range: `scripts/game/GameScreen.cs:1014`
- Inputs:
  - `AudioStreamOggVorbis[] sounds`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `RandRange(...)`
  - Calls `Play(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PlaySound(AudioStream sound)`

- Signature: `private void PlaySound(AudioStream sound)`
- Source range: `scripts/game/GameScreen.cs:1025`
- Inputs:
  - `AudioStream sound`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetSfxBusName(...)`
  - Calls `AddChild(...)`
  - Calls `QueueFree(...)`
  - Calls `Play(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ToViewSuit(CardSuit suit)`

- Signature: `private static Card.SuitType ToViewSuit(CardSuit suit)`
- Source range: `scripts/game/GameScreen.cs:1042`
- Inputs:
  - `CardSuit suit`
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

### `ToViewRank(CardRank rank)`

- Signature: `private static Card.RankType ToViewRank(CardRank rank)`
- Source range: `scripts/game/GameScreen.cs:1054`
- Inputs:
  - `CardRank rank`
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
