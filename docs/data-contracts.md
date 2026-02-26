# Data Contracts and RPC Reference

This file summarizes the important backend data contracts.

## Enums

### `GameType`

- `Omi`

### `ParticipantRole`

- `Player`
- `Spectator`

### `ParticipantKind`

- `Human`
- `Bot`

### `AiDifficulty`

- `Easy`
- `Normal`
- `Strong`

### `SeatPosition`

- `Bottom`
- `Right`
- `Top`
- `Left`

Team mapping:

- Team 0: Bottom + Top
- Team 1: Right + Left

### `OmiPhase`

- `LobbySeating`
- `Shuffle`
- `Cut`
- `FirstDeal`
- `TrumpSelect`
- `SecondDeal`
- `TrickPlay`
- `RoundScore`
- `MatchScore` (currently unused in flow)
- `MatchEnd`
- `PausedReconnect`

## Match snapshot (server -> client)

Serializer: `src/scripts/core/serialization/MatchSnapshotSerializer.cs`

Important keys in snapshot dictionary:

- `phase`
- `roundNumber`
- `completedTricksCount`
- `trumpSuit`
- `shufflerSeat`
- `cutterSeat`
- `trumpSelectorSeat`
- `currentTurnSeat`
- `teamCredits`
- `teamTricks`
- `currentStake`
- `roundWinnerTeam`
- `matchWinnerTeam`
- `isPausedForReconnect`
- `reconnectPeerId`
- `reconnectDeadlineUnixSeconds`
- `handCounts`
- `visibleHands`
- `currentTrick`
- `viewerRole`
- `viewerSeat`

## Room snapshot (server -> client)

Built from `RoomState.ToSnapshotDictionary`.

Important keys:

- `roomName`
- `hostName`
- `hostPeerId`
- `roomInstanceId`
- `gameType`
- `matchLifecycle`
- `aiAutoFillEnabled`
- `selectedAiDifficulty`
- `participants`
- `seats`
- `playerCount`
- `spectatorCount`
- `connectedPlayerCount`

Participant entries now also include:

- `kind`
- `botDifficulty`

## Client -> server RPCs

Defined in `src/scripts/network/NetworkRpc.cs`:

- `RequestJoinRoom(playerName, requestedRole, reconnectToken)`
- `RequestSeatChange(targetSeat, targetPeerId)`
- `RequestStartMatch()`
- `RequestSetAiOptions(autoFill, difficulty)`
- `RequestCutDeck(cutIndex)`
- `RequestShuffleAgain(seed)`
- `RequestFinishShuffle()`
- `RequestSelectTrump(suit)`
- `RequestPlayCard(cardId)`
- `RequestLeaveRoom()`

## Server -> client RPCs

- `PushRoomSnapshot(snapshotJson)`
- `PushSeatMap(seatSnapshotJson)`
- `PushMatchSnapshot(snapshotJson)`
- `PushMatchStarted(payloadJson)`
- `PushPrivateHand(payloadJson)`
- `PushSpectatorHands(payloadJson)`
- `PushCardPlayed(payloadJson)`
- `PushTrickResolved(payloadJson)`
- `PushRoundResolved(payloadJson)`
- `PushCreditsUpdated(payloadJson)`
- `PushPausedForReconnect(disconnectedPeerId, reconnectDeadlineUnixSeconds)`
- `PushMatchEnded(payloadJson)`
- `PushServerMessage(message)`

## Match events produced by rules engine

Event names currently used:

- `round_started`
- `deck_shuffled`
- `shuffle_finished`
- `deck_cut`
- `trump_selected`
- `card_played`
- `trick_resolved`
- `round_resolved`
- `credits_updated`
- `match_ended`
