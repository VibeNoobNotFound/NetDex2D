# Omi Rules Engine (Authoritative Rule Truth)

Main file: `src/scripts/core/OmiRulesEngine.cs`

This is the backend source of truth for legal moves, phase transitions, trick winners, credits, and match end.

## Core model

- Deterministic command/state engine.
- Input: `MatchCommand`.
- Validation + mutation happen only in the engine.
- Output: `MatchCommandResult` with events for UI/network broadcast.

No UI timing, animation, or client trust logic is implemented here.

## Card and trick baseline

- Deck: 32 cards (`7..A`, 4 suits).
- Must-follow-suit is enforced in `PlayCard`.
- Trick winner:
- highest trump if any trump was played.
- otherwise highest lead-suit card.

## Active phase flow

1. `LobbySeating`
2. `Shuffle`
3. `Cut`
4. `FirstDeal`
5. `TrumpSelect`
6. `SecondDeal`
7. `TrickPlay`
8. `TrickResolveHold`
9. `KapothiProposal` (optional window)
10. `KapothiResponse` (optional window)
11. `RoundScore`
12. `MatchEnd`

`MatchCoordinator` advances timed phases (`FirstDeal`, `SecondDeal`, `TrickResolveHold`, Kapothi windows) when deadlines expire.

## Kapothi house-rule implementation

Project-specific variant:

- Kapothi window opens once per round, only at the first `4-0` checkpoint after trick resolution.
- Eligibility requires:
- `CompletedTricksCount == 4`
- One team has 4 tricks and the other has 0.
- Proposal phase:
- eligible (winning) team can `KapothiPropose` or `KapothiSkip`.
- Response phase:
- target (losing) team can `KapothiAccept` or `KapothiReject`.
- First valid response finalizes the window.
- Timeout fallback (handled by coordinator):
- proposal timeout => auto-skip
- response timeout => auto-reject

If accepted:

- `KapothiAcceptedThisRound = true`.
- contract caller persists in `KapothiCallingTeamThisRound`.
- success = caller takes all 8 tricks.
- failure = opponent takes at least 1 trick (caller loses Kapothi contract even if caller still has more tricks).

## Scoring model

No sweep auto-match-end by tricks. Match ends by credits reaching `0` (or forfeit path).

On non-Kapothi rounds:

- `baseLoss = 2` if trick-loser is trump team.
- `baseLoss = 1` otherwise.
- `drawBonus = PendingDrawBonusCredits` (`0` or `1`).
- `kapothiBonus = 0`.
- `totalLoss = baseLoss + drawBonus`.

On accepted Kapothi rounds:

- credit loser is determined by contract outcome (not trick majority).
- `baseLoss = 2` if credit loser is trump team.
- `baseLoss = 1` otherwise.
- `drawBonus = PendingDrawBonusCredits` (`0` or `1`).
- `kapothiBonus = 2`.
- `totalLoss = baseLoss + drawBonus + kapothiBonus`.

Draw bookkeeping:

- first consecutive draw arms bonus: `PendingDrawBonusCredits = 1`, `ConsecutiveDraws = 1`.
- second consecutive draw cancels bonus: `PendingDrawBonusCredits = 0`, `ConsecutiveDraws = 0`.
- accepted Kapothi ending `4-4` still runs draw bookkeeping while also applying Kapothi scoring.

On non-draw resolution:

- pending draw bonus and consecutive draw counter reset to zero.

## Round-tracked state added

`OmiMatchState` includes:

- `PendingDrawBonusCredits`
- `ConsecutiveDraws`
- `TrumpTeamIndexThisRound`
- `KapothiEligibleTeam`
- `KapothiTargetTeam`
- `KapothiCallingTeamThisRound`
- `KapothiOfferedThisRound`
- `KapothiAcceptedThisRound`
- `KapothiWindowConsumed`

These fields are cloned and serialized in snapshots.

## Events emitted for clients

Core events now include Kapothi and richer round payload:

- `kapothi_window_opened`
- `kapothi_proposed`
- `kapothi_skipped`
- `kapothi_accepted`
- `kapothi_rejected`
- `round_resolved` (with `baseLoss`, `drawBonusApplied`, `kapothiBonusApplied`, `totalLoss`, `kapothiAccepted`, `kapothiCallingTeam`, `kapothiSucceeded`, `creditLoserTeam`, etc.)

## Reconnect/forfeit

Reconnect timeout forfeit remains unchanged:

- server submits `ForfeitTeam(teamIndex)`,
- forfeited team credits forced to `0`,
- phase becomes `MatchEnd`.
