# AI Lifecycle

## Host-side orchestration
- `AiCoordinator` observes room + match state changes.
- When current actor is bot and phase is actionable, it builds a strict-fair perception model.
- Search runs off main thread with cancellation/version guard.
- Chosen command is submitted through the same authoritative command path as human commands.

## Fairness boundary
- Bot sees own hand + public trick history + inferred void suits.
- Bot does not read hidden opponent hands directly.

## Difficulty
- `Easy`, `Normal`, `Strong` map to different simulation/time budgets and rollout quality in `OmiBotPolicy`.
