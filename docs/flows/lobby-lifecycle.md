# Lobby Lifecycle

## Room state ownership
- Host owns room truth in `LobbyManager` + `RoomState`.
- Clients render snapshots and cannot mutate authoritative room state directly.

## Core operations
- Join/reconnect validation with reconnect token.
- Seat assignment by host (including bot placement behavior).
- AI options (auto-fill + difficulty) host-controlled.
- Start match gate checks before transitioning to in-match lifecycle.

## Snapshot distribution
- Host broadcasts room snapshots to all peers.
- Clients update local state stores and UI subscribes to `RoomStateChanged`.
