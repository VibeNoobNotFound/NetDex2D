# AI Bot System (Host Side)

This file explains how AI players work in NetDex.

## What AI bots are

- Bots are fake players controlled by the host server.
- Bots can fill empty seats when a match starts.
- Bots follow the same rules as human players.
- Bots do not receive hidden cards from opponents.

## Where the AI code lives

- `src/scripts/ai/AiCoordinator.cs`
- `src/scripts/ai/OmiBotPolicy.cs`
- `src/scripts/ai/BotPerceptionState.cs`
- `src/scripts/ai/VoidSuitTracker.cs`
- `src/scripts/ai/AiDifficulty.cs`
- `src/scripts/ai/ParticipantKind.cs`

## Lobby controls

Host can set these options in lobby:

- `Auto-fill empty seats with AI` (on/off)
- `AI Difficulty` (`Easy`, `Normal`, `Strong`)

When host starts match:

- If auto-fill is on, server creates bots for empty seats.
- If auto-fill is off, host still needs all 4 seats filled by humans.

## Bot fairness model

Bots are strict-fair:

- Bot can see:
  - its own hand
  - cards already played (current trick + completed tricks)
  - public state (turn, phase, trump, score, hand counts)
- Bot cannot see:
  - hidden cards in opponent hands
  - future deck order

To reason about unknown cards, bot builds sampled possible worlds (determinization) from legal public info.

## Bot decision flow

1. `AiCoordinator` checks whose turn it is.
2. If that seat belongs to a bot, it creates a fair perception snapshot.
3. It runs bot search on a background thread.
4. It waits a short delay (human-like pacing).
5. It submits one command to `MatchCoordinator`.
6. `MatchCoordinator` still validates command using `OmiRulesEngine`.

If the state changed while bot was thinking, the old decision is dropped.

## What actions bots can take

Bots handle all Omi phases:

- Shuffle:
  - choose `ShuffleAgain` or `FinishShuffle`
- Cut:
  - choose cut index
- Trump select:
  - choose trump suit using hand + rollouts
- Trick play:
  - choose legal card using simulation + heuristics

## Difficulty levels

- `Easy`: small budget, simpler choices
- `Normal`: medium budget
- `Strong`: larger search budget and stronger rollout policy

Default difficulty is `Strong`.

## Data model changes

- `ParticipantInfo` now includes:
  - `Kind` (`Human` or `Bot`)
  - `BotDifficulty` (nullable)
- `RoomState` now includes:
  - `AiAutoFillEnabled`
  - `SelectedAiDifficulty`

These fields are included in room snapshots so all clients can render bot info.

## Limits in current version

- Bot auto-fill happens at match start.
- No mid-match human takeover of a bot seat.
- No omniscient (cheating) bot mode.
