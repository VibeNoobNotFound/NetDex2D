# Backend Overview

This document explains the backend in simple terms.

## What "backend" means here

In this game, there is no separate cloud server.  
The host player's game process acts as the server (authoritative server).

That means:

- Host decides what is valid.
- Clients send requests.
- Host validates and updates the real game state.
- Host sends updated state back to everyone.

## Main backend pieces

### 1) `NetworkManager` (transport + discovery)

File: `src/scripts/network/NetworkManager.cs`

Responsibilities:

- Start host server (`ENetMultiplayerPeer.CreateServer`)
- Join host (`ENetMultiplayerPeer.CreateClient`)
- LAN discovery via UDP broadcast
- Disconnect/session teardown
- Connection event handling (connected/disconnected/failure)
- Reconnect token storage in `user://player_profile.cfg`

### 2) `NetworkRpc` (RPC contract bridge)

File: `src/scripts/network/NetworkRpc.cs`

Responsibilities:

- Defines all client->server and server->client RPC methods
- Forwards valid requests to backend services (`LobbyManager`, `MatchCoordinator`)
- Broadcasts game/lobby events to peers

### 3) `LobbyManager` (room + participants + seats + snapshots)

File: `src/scripts/lobby/LobbyManager.cs`

Responsibilities:

- Create room when host starts
- Track participants (player/spectator, connected/offline, host flag)
- Manage seat assignments
- Validate host-only lobby actions
- Handle reconnect logic at room level
- Build and send room snapshots + per-peer match snapshots

### 4) `MatchCoordinator` (server match orchestrator)

File: `src/scripts/game/MatchCoordinator.cs`

Responsibilities:

- Start match
- Apply gameplay commands on server
- Push gameplay events (card played, trick resolved, etc.)
- Auto-start next round after round scoring
- Pause and resume on reconnect
- Forfeit on reconnect timeout

### 5) `OmiRulesEngine` (pure game rules/state machine)

File: `src/scripts/core/OmiRulesEngine.cs`

Responsibilities:

- Enforce Omi rules
- Validate every command by phase + turn + rule constraints
- Update deterministic match state
- Produce rule events

### 6) `AiCoordinator` + AI policy (host bot decisions)

Files:

- `src/scripts/ai/AiCoordinator.cs`
- `src/scripts/ai/OmiBotPolicy.cs`

Responsibilities:

- Detect when current turn belongs to a bot participant
- Build strict-fair bot perception (no hidden opponent hands)
- Compute command in background thread
- Submit command back through authoritative server flow (`MatchCoordinator`)
- Keep decisions safe with state-version checks

## Autoload services (always alive)

These are configured in `src/project.godot`:

- `GameManager`
- `LobbyManager`
- `MatchCoordinator`
- `AiCoordinator`
- `NetworkRpc`
- `NetworkManager`

Because they are autoloads, all scenes can access shared backend state.

## How frontend and backend connect

- UI sends request through `NetworkRpc`.
- Host validates and updates via `LobbyManager`/`MatchCoordinator`/`OmiRulesEngine`.
- Host broadcasts new snapshots.
- UI receives snapshot and redraws from backend truth.

Important: UI does not own the truth. Backend does.
