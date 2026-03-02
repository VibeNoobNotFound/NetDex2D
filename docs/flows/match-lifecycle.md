# Match Lifecycle

## Authoritative loop
1. Client submits gameplay command through `NetworkRpc` (`RequestCutDeck`, `RequestPlayCard`, etc.).
2. `MatchCoordinator` checks authority/lifecycle constraints.
3. `OmiRulesEngine.ApplyCommand` validates and mutates `OmiMatchState`.
4. `MatchCoordinator` broadcasts snapshots/events (`PushMatchSnapshot`, `PushCardPlayed`, `PushTrickResolved`, etc.).
5. `GameScreen` renders from snapshots only (no client-side authority).

## Timed holds / pacing
- Deal/trick resolve hold phases use deadlines in state and server-side `_Process` timers.
- Clients observe same timing via authoritative phase snapshots.

## Round/match ending
- Round scoring updates credits/stake according to Omi logic.
- Match ends on credit exhaustion or sweep condition.
