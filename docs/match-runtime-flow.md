# Match Runtime Flow (With Kapothi + Timed Holds)

This page explains what actually happens at runtime from host start to round scoring.

## 1) Match start

- Host triggers `RequestStartMatch`.
- Server validates lobby state and seats.
- `MatchCoordinator.ServerStartMatch()` creates authoritative state and applies `StartRound`.
- Room lifecycle switches to `InMatch`.
- Server broadcasts room snapshot + match snapshot.

## 2) Round setup and timed deal phases

- `Shuffle`:
- shuffler can `ShuffleAgain` repeatedly, then `FinishShuffle`.
- `Cut`:
- cutter sends `CutDeck`.
- `FirstDeal`:
- server deals 4 cards each and enters timed reveal hold.
- deadline auto-advances using `CompleteFirstDeal`.
- `TrumpSelect`:
- trump selector sends `SelectTrump`.
- `SecondDeal`:
- server deals remaining 4 cards each and enters timed reveal hold.
- deadline auto-advances using `CompleteSecondDeal`.

All clients render these as authoritative snapshots and animations.

## 3) Trick play + server lock

- `TrickPlay` accepts only legal card from current turn seat.
- After 4th card, server moves to `TrickResolveHold` (readability delay).
- On deadline, server applies `ResolveCurrentTrick`.
- Winner seat leads next trick.

## 4) Kapothi window insertion

After trick resolution, if first `4-0` checkpoint is reached:

- phase becomes `KapothiProposal`,
- eligible winning team can propose or skip.

If proposed:

- phase becomes `KapothiResponse`,
- losing team can accept or reject.

Timeout behavior:

- proposal timeout => auto-skip,
- response timeout => auto-reject.

If no Kapothi window is opened (or after it closes), flow returns to `TrickPlay`.

## 5) Round end scoring

After 8 tricks:

- Draw:
- first draw arms `+1` pending bonus.
- second consecutive draw cancels pending bonus.
- Decisive round:
- base loss is `2` if trump team lost, else `1`.
- add pending draw bonus (`0/1`).
- add Kapothi accepted bonus (`+2`) if accepted this round.
- loser credits decrease by total loss.

No immediate match end from `8-0` tricks. `8-0` is just decisive scoring under the same formula.

## 6) Match end and next round

- If any team credits reach `0`: phase -> `MatchEnd`, `match_ended` event broadcast.
- Otherwise phase enters `RoundScore`; coordinator immediately starts next round via `StartNextRound`.

## 7) Reconnect pause precedence

If seated player disconnects mid-match:

- server pauses state (`PausedReconnect`),
- timed phase progression is suspended,
- reconnect before deadline resumes original phase,
- timeout forfeits disconnected team.

## 8) Client rendering contract

`GameScreen` is snapshot-driven only:

- action panel enables only when local role/seat/phase permits.
- Kapothi actions are shown only to the acting team.
- spectators/other players are read-only.

This keeps gameplay consistent on host, clients, and spectators.
