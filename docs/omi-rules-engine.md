# Omi Rules Engine (Backend Truth)

Main file: `src/scripts/core/OmiRulesEngine.cs`

This is the most important backend file for game correctness.

## Core idea

The rules engine is a deterministic state machine:

- It receives a command (`MatchCommand`).
- It validates the command against current phase/rules.
- If valid, it updates `OmiMatchState`.
- It returns events describing what happened.

No UI logic is inside this engine.

## Card model

Deck:

- 32 cards total
- suits: Hearts, Diamonds, Clubs, Spades
- ranks: 7 to Ace

Strength currently equals rank numeric value (`7..14`).

Files:

- `src/scripts/core/CardModel.cs`
- `src/scripts/core/DeckService.cs`

## Match phases

Enum: `OmiPhase`

Actual phase order used:

1. `LobbySeating`
2. `Shuffle`
3. `Cut`
4. `FirstDeal`
5. `TrumpSelect`
6. `SecondDeal`
7. `TrickPlay`
8. `RoundScore`
9. `MatchEnd`

(`MatchScore` exists in enum but current flow goes through `RoundScore` and auto-next-round logic.)

## Round setup

At start of round:

- shuffler rotates each round (first round host seat)
- cutter is left of shuffler (`Previous()`)
- trump selector is right of shuffler (`Next()`)
- deck is shuffled with random seed
- phase becomes `Shuffle`

## Shuffle stage behavior

Current behavior:

- only shuffler can reshuffle (`ShuffleAgain`)
- shuffler can reshuffle unlimited times
- only shuffler can finish shuffle (`FinishShuffle`)
- finishing shuffle moves phase to `Cut` and gives turn to cutter

## Deal and trump behavior

- Cutter cuts the deck.
- First deal: 4 cards each, round-robin starting from trump selector seat.
- Trump selector chooses trump suit.
- Second deal: remaining 4 cards each, same order.
- Trick play starts, first turn = trump selector.

## Trick play validation

For each card play:

- command must be in `TrickPlay` phase
- actor seat must match current turn
- card must exist in player's hand
- if player has lead suit, player must follow lead suit

Winner logic:

- if any trump played, highest trump wins
- else highest lead suit wins

Winner seat leads next trick.

## Round and match scoring

After 8 tricks:

- If 8-0 sweep:
  - immediate match end
  - sweep winner becomes match winner
- If draw (4-4):
  - no credit deduction
  - next stake becomes `2`
- If decisive winner:
  - loser team loses `currentStake` credits
  - stake resets to `1`

If loser credits reach `0`, match ends.

## Reconnect forfeit path

If reconnect timeout expires, coordinator sends `ForfeitTeam(teamIndex)` command:

- forfeited team credits become `0`
- other team immediately wins match

## Why this design is good

- deterministic
- easy to unit test
- host can reject illegal client actions
- future games can plug in via `IGameRulesEngine`
