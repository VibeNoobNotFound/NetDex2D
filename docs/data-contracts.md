# Data Contracts and RPC Map (Current)

This is the compact contract reference for multiplayer state and commands.

## Enums used by runtime

### `OmiPhase`

- `LobbySeating`
- `Shuffle`
- `Cut`
- `FirstDeal`
- `TrumpSelect`
- `SecondDeal`
- `TrickPlay`
- `RoundScore`
- `MatchScore` (legacy/unused in current flow)
- `MatchEnd`
- `PausedReconnect`
- `TrickResolveHold`
- `KapothiProposal`
- `KapothiResponse`

### `MatchCommandType`

- `StartRound`
- `ShuffleAgain`
- `FinishShuffle`
- `CutDeck`
- `SelectTrump`
- `PlayCard`
- `StartNextRound`
- `ForfeitTeam`
- `CompleteFirstDeal`
- `CompleteSecondDeal`
- `ResolveCurrentTrick`
- `KapothiPropose`
- `KapothiSkip`
- `KapothiAccept`
- `KapothiReject`

## Match snapshot keys

Serializer: `src/scripts/core/serialization/MatchSnapshotSerializer.cs`

Important keys:

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
- `currentStake` (compatibility field; scoring no longer depends on this)
- `pendingDrawBonusCredits`
- `consecutiveDraws`
- `trumpTeamIndexThisRound`
- `kapothiEligibleTeam`
- `kapothiTargetTeam`
- `kapothiOfferedThisRound`
- `kapothiAcceptedThisRound`
- `kapothiWindowConsumed`
- `roundWinnerTeam`
- `matchWinnerTeam`
- `phaseDeadlineUnixSeconds`
- `isPausedForReconnect`
- `reconnectPeerId`
- `reconnectDeadlineUnixSeconds`
- `handCounts`
- `visibleHands`
- `currentTrick`
- `viewerRole`
- `viewerSeat`

## Room snapshot keys

From `RoomState.ToSnapshotDictionary`:

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

Participant entries include:

- `peerId`
- `name`
- `role`
- `seat`
- `isConnected`
- `reconnectToken`
- `kind`
- `botDifficulty`

## Client -> server RPCs

`src/scripts/network/NetworkRpc.cs`

- `RequestJoinRoom(playerName, requestedRole, reconnectToken)`
- `RequestSeatChange(targetSeat, targetPeerId)`
- `RequestStartMatch()`
- `RequestSetAiOptions(autoFill, difficulty)`
- `RequestCutDeck(cutIndex)`
- `RequestShuffleAgain(seed)`
- `RequestFinishShuffle()`
- `RequestSelectTrump(suit)`
- `RequestPlayCard(cardId)`
- `RequestKapothiPropose()`
- `RequestKapothiSkip()`
- `RequestKapothiAccept()`
- `RequestKapothiReject()`
- `RequestLeaveRoom()`

## Server -> client RPCs

- `PushRoomSnapshot(snapshotJson)`
- `PushSeatMap(snapshotJson)`
- `PushMatchSnapshot(snapshotJson)`
- `PushMatchStarted(payloadJson)`
- `PushCardPlayed(payloadJson)`
- `PushTrickResolved(payloadJson)`
- `PushRoundResolved(payloadJson)`
- `PushCreditsUpdated(payloadJson)`
- `PushPausedForReconnect(disconnectedPeerId, reconnectDeadlineUnixSeconds)`
- `PushMatchEnded(payloadJson)`
- `PushServerMessage(message)`

## Rules-engine events broadcast by coordinator

`MatchCoordinator.PushEventNotifications(...)` forwards selected `MatchEvent` types:

- `round_started`
- `card_played`
- `trick_resolved`
- `round_resolved`
- `credits_updated`
- `match_ended`

Kapothi events are currently state-driven via snapshots (and may be added to broadcast list later if needed for richer client-only effects).
