# OmiBotPolicy (ai/OmiBotPolicy.cs)

- Source: `scripts/ai/OmiBotPolicy.cs`
- Namespace: `NetDex.AI`
- Purpose: OmiBotPolicy in `ai/OmiBotPolicy.cs`. AI decision-making, imperfect-information perception, and bot action scheduling.

## Dependencies

- Key imports:
  - `NetDex.Core.Commands`
  - `NetDex.Core.Enums`
  - `NetDex.Core.Models`
  - `NetDex.Core.Rules`
  - `System`
  - `System.Collections.Generic`
  - `System.Diagnostics`
  - `System.Linq`
  - `System.Threading`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `ChooseCommand(BotPerceptionState perception, IGameRulesEngine rulesEngine, CancellationToken cancellationToken)` | `MatchCommand` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `ResolveSettings(AiDifficulty difficulty)` | `private` | `SearchSettings` |
| `ChooseShuffleCommand(IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, SearchSettings settings, Random random)` | `private` | `MatchCommand` |
| `ChooseCutCommand(IReadOnlyList<MatchCommand> legal, Random random, AiDifficulty difficulty)` | `private` | `MatchCommand` |
| `ChooseTrumpCommand(IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, IGameRulesEngine rulesEngine, SearchSettings settings, Random random, CancellationToken cancellationToken)` | `private` | `MatchCommand` |
| `ChooseTrickPlayCommand(IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, IGameRulesEngine rulesEngine, SearchSettings settings, Random random, CancellationToken cancellationToken)` | `private` | `MatchCommand` |
| `SimulateCandidate(MatchCommand candidate, BotPerceptionState perception, IGameRulesEngine rulesEngine, Random random, CancellationToken cancellationToken)` | `private` | `double` |
| `RolloutToRoundEnd(OmiMatchState state, SeatPosition botSeat, IGameRulesEngine rulesEngine, Random random, CancellationToken cancellationToken)` | `private` | `void` |
| `ChooseRolloutCommand(OmiMatchState state, SeatPosition actorSeat, IReadOnlyList<MatchCommand> legal, Random random)` | `private` | `MatchCommand` |
| `ChooseBestTrumpForSeat(OmiMatchState state, IReadOnlyList<MatchCommand> legal, SeatPosition seat)` | `private` | `MatchCommand` |
| `SelectBestImmediatePlay(OmiMatchState state, IReadOnlyList<MatchCommand> playable, SeatPosition seat)` | `private` | `MatchCommand` |
| `ScoreImmediatePlay(OmiMatchState state, MatchCommand command, SeatPosition seat)` | `private` | `double` |
| `ScoreTrumpFromHand(IReadOnlyList<CardModel> hand, CardSuit suit)` | `private` | `double` |
| `ResolveActorSeat(OmiMatchState state)` | `private` | `SeatPosition` |
| `DetermineWinnerWithCandidate(IReadOnlyList<PlayedCard> currentTrick, CardModel candidate, SeatPosition candidateSeat, CardSuit? trumpSuit)` | `private` | `SeatPosition` |
| `EvaluateRoundUtility(OmiMatchState state, int botTeam)` | `private` | `double` |
| `SampleDeterminizedState(BotPerceptionState perception, Random random, bool strictVoidConstraints)` | `private` | `OmiMatchState` |
| `TryDistributeUnknownCards(IReadOnlyList<CardModel> unknownPool, IReadOnlyList<SeatPosition> seats, IReadOnlyDictionary<SeatPosition, int> requiredBySeat, int remainingDeckCount, VoidSuitTracker voidTracker, bool strictVoidConstraints, Random random, out Dictionary<SeatPosition, List<CardModel>> seatHands, out List<CardModel> deckRemainder)` | `private` | `bool` |

## Function-by-Function

### `ChooseCommand(BotPerceptionState perception, IGameRulesEngine rulesEngine, CancellationToken cancellationToken)`

- Signature: `public MatchCommand ChooseCommand(BotPerceptionState perception, IGameRulesEngine rulesEngine, CancellationToken cancellationToken)`
- Source range: `scripts/ai/OmiBotPolicy.cs:22`
- Inputs:
  - `BotPerceptionState perception, IGameRulesEngine rulesEngine, CancellationToken cancellationToken`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `EnumerateLegalCommands(...)`
  - Calls `InvalidOperationException(...)`
  - Calls `ResolveSettings(...)`
  - Calls `Random(...)`
  - Calls `ChooseShuffleCommand(...)`
  - Calls `ChooseCutCommand(...)`
  - Calls `ChooseTrumpCommand(...)`
  - Calls `ChooseTrickPlayCommand(...)`
- Failure paths:
  - Has exception handling/throw path.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ResolveSettings(AiDifficulty difficulty)`

- Signature: `private static SearchSettings ResolveSettings(AiDifficulty difficulty)`
- Source range: `scripts/ai/OmiBotPolicy.cs:44`
- Inputs:
  - `AiDifficulty difficulty`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SearchSettings(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ChooseShuffleCommand(IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, SearchSettings settings, Random random)`

- Signature: `private static MatchCommand ChooseShuffleCommand(IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, SearchSettings settings, Random random)`
- Source range: `scripts/ai/OmiBotPolicy.cs:69`
- Inputs:
  - `IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, SearchSettings settings, Random random`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `FirstOrDefault(...)`
  - Calls `Where(...)`
  - Calls `ToList(...)`
  - Calls `NextDouble(...)`
  - Calls `Next(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ChooseCutCommand(IReadOnlyList<MatchCommand> legal, Random random, AiDifficulty difficulty)`

- Signature: `private static MatchCommand ChooseCutCommand(IReadOnlyList<MatchCommand> legal, Random random, AiDifficulty difficulty)`
- Source range: `scripts/ai/OmiBotPolicy.cs:93`
- Inputs:
  - `IReadOnlyList<MatchCommand> legal, Random random, AiDifficulty difficulty`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Where(...)`
  - Calls `OrderBy(...)`
  - Calls `ToList(...)`
  - Calls `Next(...)`
  - Calls `Abs(...)`
  - Calls `ThenBy(...)`
  - Calls `First(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ChooseTrumpCommand(IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, IGameRulesEngine rulesEngine, SearchSettings settings, Random random, CancellationToken cancellationToken)`

- Signature: `private MatchCommand ChooseTrumpCommand(IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, IGameRulesEngine rulesEngine, SearchSettings settings, Random random, CancellationToken cancellationToken)`
- Source range: `scripts/ai/OmiBotPolicy.cs:121`
- Inputs:
  - `IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, IGameRulesEngine rulesEngine, SearchSettings settings, Random random, CancellationToken cancellationToken`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Where(...)`
  - Calls `ScoreTrumpFromHand(...)`
  - Calls `SampleDeterminizedState(...)`
  - Calls `ApplyCommand(...)`
  - Calls `RolloutToRoundEnd(...)`
  - Calls `EvaluateRoundUtility(...)`
  - Calls `TeamIndex(...)`
  - Calls `OrderByDescending(...)`
  - Calls `ThenBy(...)`
  - Calls `First(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ChooseTrickPlayCommand(IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, IGameRulesEngine rulesEngine, SearchSettings settings, Random random, CancellationToken cancellationToken)`

- Signature: `private MatchCommand ChooseTrickPlayCommand(IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, IGameRulesEngine rulesEngine, SearchSettings settings, Random random, CancellationToken cancellationToken)`
- Source range: `scripts/ai/OmiBotPolicy.cs:180`
- Inputs:
  - `IReadOnlyList<MatchCommand> legal, BotPerceptionState perception, IGameRulesEngine rulesEngine, SearchSettings settings, Random random, CancellationToken cancellationToken`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Where(...)`
  - Calls `ToList(...)`
  - Calls `SelectBestImmediatePlay(...)`
  - Calls `StartNew(...)`
  - Calls `SimulateCandidate(...)`
  - Calls `ScoreImmediatePlay(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SimulateCandidate(MatchCommand candidate, BotPerceptionState perception, IGameRulesEngine rulesEngine, Random random, CancellationToken cancellationToken)`

- Signature: `private double SimulateCandidate(MatchCommand candidate, BotPerceptionState perception, IGameRulesEngine rulesEngine, Random random, CancellationToken cancellationToken)`
- Source range: `scripts/ai/OmiBotPolicy.cs:252`
- Inputs:
  - `MatchCommand candidate, BotPerceptionState perception, IGameRulesEngine rulesEngine, Random random, CancellationToken cancellationToken`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `SampleDeterminizedState(...)`
  - Calls `ApplyCommand(...)`
  - Calls `RolloutToRoundEnd(...)`
  - Calls `EvaluateRoundUtility(...)`
  - Calls `TeamIndex(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RolloutToRoundEnd(OmiMatchState state, SeatPosition botSeat, IGameRulesEngine rulesEngine, Random random, CancellationToken cancellationToken)`

- Signature: `private void RolloutToRoundEnd(OmiMatchState state, SeatPosition botSeat, IGameRulesEngine rulesEngine, Random random, CancellationToken cancellationToken)`
- Source range: `scripts/ai/OmiBotPolicy.cs:270`
- Inputs:
  - `OmiMatchState state, SeatPosition botSeat, IGameRulesEngine rulesEngine, Random random, CancellationToken cancellationToken`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ApplyCommand(...)`
  - Calls `CompleteFirstDeal(...)`
  - Calls `CompleteSecondDeal(...)`
  - Calls `ResolveCurrentTrick(...)`
  - Calls `ResolveActorSeat(...)`
  - Calls `EnumerateLegalCommands(...)`
  - Calls `ChooseRolloutCommand(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ChooseRolloutCommand(OmiMatchState state, SeatPosition actorSeat, IReadOnlyList<MatchCommand> legal, Random random)`

- Signature: `private MatchCommand ChooseRolloutCommand(OmiMatchState state, SeatPosition actorSeat, IReadOnlyList<MatchCommand> legal, Random random)`
- Source range: `scripts/ai/OmiBotPolicy.cs:340`
- Inputs:
  - `OmiMatchState state, SeatPosition actorSeat, IReadOnlyList<MatchCommand> legal, Random random`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `FirstOrDefault(...)`
  - Calls `ChooseBestTrumpForSeat(...)`
  - Calls `Where(...)`
  - Calls `ToList(...)`
  - Calls `NextDouble(...)`
  - Calls `Next(...)`
  - Calls `SelectBestImmediatePlay(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ChooseBestTrumpForSeat(OmiMatchState state, IReadOnlyList<MatchCommand> legal, SeatPosition seat)`

- Signature: `private static MatchCommand ChooseBestTrumpForSeat(OmiMatchState state, IReadOnlyList<MatchCommand> legal, SeatPosition seat)`
- Source range: `scripts/ai/OmiBotPolicy.cs:377`
- Inputs:
  - `OmiMatchState state, IReadOnlyList<MatchCommand> legal, SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Where(...)`
  - Calls `ToList(...)`
  - Calls `OrderByDescending(...)`
  - Calls `ScoreTrumpFromHand(...)`
  - Calls `ThenBy(...)`
  - Calls `First(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SelectBestImmediatePlay(OmiMatchState state, IReadOnlyList<MatchCommand> playable, SeatPosition seat)`

- Signature: `private static MatchCommand SelectBestImmediatePlay(OmiMatchState state, IReadOnlyList<MatchCommand> playable, SeatPosition seat)`
- Source range: `scripts/ai/OmiBotPolicy.cs:392`
- Inputs:
  - `OmiMatchState state, IReadOnlyList<MatchCommand> playable, SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `OrderByDescending(...)`
  - Calls `ScoreImmediatePlay(...)`
  - Calls `ThenBy(...)`
  - Calls `First(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ScoreImmediatePlay(OmiMatchState state, MatchCommand command, SeatPosition seat)`

- Signature: `private static double ScoreImmediatePlay(OmiMatchState state, MatchCommand command, SeatPosition seat)`
- Source range: `scripts/ai/OmiBotPolicy.cs:400`
- Inputs:
  - `OmiMatchState state, MatchCommand command, SeatPosition seat`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `FirstOrDefault(...)`
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `DetermineWinnerWithCandidate(...)`
  - Calls `Next(...)`
  - Calls `Max(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ScoreTrumpFromHand(IReadOnlyList<CardModel> hand, CardSuit suit)`

- Signature: `private static double ScoreTrumpFromHand(IReadOnlyList<CardModel> hand, CardSuit suit)`
- Source range: `scripts/ai/OmiBotPolicy.cs:439`
- Inputs:
  - `IReadOnlyList<CardModel> hand, CardSuit suit`
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

### `ResolveActorSeat(OmiMatchState state)`

- Signature: `private static SeatPosition ResolveActorSeat(OmiMatchState state)`
- Source range: `scripts/ai/OmiBotPolicy.cs:465`
- Inputs:
  - `OmiMatchState state`
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

### `DetermineWinnerWithCandidate(IReadOnlyList<PlayedCard> currentTrick, CardModel candidate, SeatPosition candidateSeat, CardSuit? trumpSuit)`

- Signature: `private static SeatPosition DetermineWinnerWithCandidate(IReadOnlyList<PlayedCard> currentTrick, CardModel candidate, SeatPosition candidateSeat, CardSuit? trumpSuit)`
- Source range: `scripts/ai/OmiBotPolicy.cs:476`
- Inputs:
  - `IReadOnlyList<PlayedCard> currentTrick, CardModel candidate, SeatPosition candidateSeat, CardSuit? trumpSuit`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `AddRange(...)`
  - Calls `Add(...)`
  - Calls `PlayedCard(...)`
  - Calls `Any(...)`
  - Calls `Where(...)`
  - Calls `OrderByDescending(...)`
  - Calls `First(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `EvaluateRoundUtility(OmiMatchState state, int botTeam)`

- Signature: `private static double EvaluateRoundUtility(OmiMatchState state, int botTeam)`
- Source range: `scripts/ai/OmiBotPolicy.cs:500`
- Inputs:
  - `OmiMatchState state, int botTeam`
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

### `SampleDeterminizedState(BotPerceptionState perception, Random random, bool strictVoidConstraints)`

- Signature: `private static OmiMatchState SampleDeterminizedState(BotPerceptionState perception, Random random, bool strictVoidConstraints)`
- Source range: `scripts/ai/OmiBotPolicy.cs:526`
- Inputs:
  - `BotPerceptionState perception, Random random, bool strictVoidConstraints`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `DeepClone(...)`
  - Calls `Add(...)`
  - Calls `BuildOmiDeck(...)`
  - Calls `Where(...)`
  - Calls `Contains(...)`
  - Calls `ToList(...)`
  - Calls `ToDictionary(...)`
  - Calls `TryGetValue(...)`
  - Calls `Sum(...)`
  - Calls `Max(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `TryDistributeUnknownCards(IReadOnlyList<CardModel> unknownPool, IReadOnlyList<SeatPosition> seats, IReadOnlyDictionary<SeatPosition, int> requiredBySeat, int remainingDeckCount, VoidSuitTracker voidTracker, bool strictVoidConstraints, Random random, out Dictionary<SeatPosition, List<CardModel>> seatHands, out List<CardModel> deckRemainder)`

- Signature: `private static bool TryDistributeUnknownCards(IReadOnlyList<CardModel> unknownPool, IReadOnlyList<SeatPosition> seats, IReadOnlyDictionary<SeatPosition, int> requiredBySeat, int remainingDeckCount, VoidSuitTracker voidTracker, bool strictVoidConstraints, Random random, out Dictionary<SeatPosition, List<CardModel>> seatHands, out List<CardModel> deckRemainder)`
- Source range: `scripts/ai/OmiBotPolicy.cs:607`
- Inputs:
  - `IReadOnlyList<CardModel> unknownPool, IReadOnlyList<SeatPosition> seats, IReadOnlyDictionary<SeatPosition, int> requiredBySeat, int remainingDeckCount, VoidSuitTracker voidTracker, bool strictVoidConstraints, Random random, out Dictionary<SeatPosition, List<CardModel>> seatHands, out List<CardModel> deckRemainder`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `ToList(...)`
  - Calls `ShuffleInPlace(...)`
  - Calls `TryGetValue(...)`
  - Calls `FindIndex(...)`
  - Calls `CanSeatHoldSuit(...)`
  - Calls `Add(...)`
  - Calls `RemoveAt(...)`
  - Calls `Take(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
