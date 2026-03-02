# AI Bot System (Host-Authoritative, Strict-Fair)

This document explains how bots are executed in the current multiplayer backend.

## Runtime ownership

- Bots run only on the host/server.
- `AiCoordinator` decides when a bot must act.
- `MatchCoordinator` still validates every AI command via `OmiRulesEngine`.
- Clients never simulate authoritative bot state.

## Files

- `src/scripts/ai/AiCoordinator.cs`
- `src/scripts/ai/OmiBotPolicy.cs`
- `src/scripts/ai/BotPerceptionState.cs`
- `src/scripts/ai/VoidSuitTracker.cs`
- `src/scripts/ai/AiDifficulty.cs`
- `src/scripts/ai/ParticipantKind.cs`

## Fairness model

Bots only use:

- own hand,
- public current trick cards,
- completed trick history,
- public counters (phase, trump, credits, tricks, hand counts).

Bots do not read hidden opponent hands. Unknown cards are determinized from legal public constraints.

## Scheduling and safety

`AiCoordinator`:

- watches `MatchStateAdvanced` + room updates,
- starts background search task for actionable phases,
- cancels stale searches on state-version change,
- applies short delay before dispatching final command.

AI is disabled for non-action phases (`FirstDeal`, `SecondDeal`, `TrickResolveHold`, reconnect pause, etc.).

## Kapothi team-action behavior

Kapothi phases are team-owned, so coordinator resolves a bot actor differently:

- `KapothiProposal`: acting team = `KapothiEligibleTeam`.
- `KapothiResponse`: acting team = `KapothiTargetTeam`.

Protection rule:

- if the acting team has any connected human seat, AI does not auto-act for Kapothi.
- AI only acts when that acting team is bot-controlled.

## Policy behavior by phase

`OmiBotPolicy.ChooseCommand(...)` handles:

- `Shuffle` (reshuffle vs finish)
- `Cut`
- `TrumpSelect` (rollout-evaluated)
- `TrickPlay` (simulation + immediate heuristics)
- `KapothiProposal` (propose vs skip by expected utility)
- `KapothiResponse` (accept vs reject by scoring-favorable probability threshold + utility)

## Difficulty differences

Difficulty adjusts simulation budgets and risk thresholds:

- `Easy`: lower budgets, more permissive Kapothi acceptance.
- `Normal`: medium budgets and thresholds.
- `Strong`: largest budget and stricter Kapothi acceptance requirement.

## Rollout updates for new rules

Rollout now understands:

- timed transition commands (`CompleteFirstDeal`, `CompleteSecondDeal`, `ResolveCurrentTrick`),
- Kapothi phases/commands during simulated progression.

Utility no longer uses old `CurrentStake` logic. It now factors:

- trick advantage,
- scoring winner / credit loser outcome,
- credit delta,
- pending draw-bonus risk,
- Kapothi accepted contract risk/reward context (including caller-failure even with trick lead).

## Lobby integration

Host controls:

- `AiAutoFillEnabled`
- `SelectedAiDifficulty`

At match start, empty seats may be auto-filled by bots (depending on host setting). Mid-match human/bot replacement is still disabled.
