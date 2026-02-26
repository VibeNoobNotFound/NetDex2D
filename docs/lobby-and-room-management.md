# Lobby and Room Management

Main backend file: `src/scripts/lobby/LobbyManager.cs`  
Core room model: `src/scripts/lobby/RoomState.cs`

## Room model

A room stores:

- room name
- host name and host peer id
- room instance id
- game type (`Omi` for now)
- match lifecycle (`Lobby`, `InMatch`, `PausedReconnect`, `MatchEnded`)
- participants dictionary
- seat assignments (`Bottom`, `Right`, `Top`, `Left`)

## Participant model

Participant fields:

- `PeerId`
- `Name`
- `Role` (`Player` or `Spectator`)
- `ReconnectToken`
- `IsConnected`
- `IsHost`
- `Seat` (nullable)

File: `src/scripts/lobby/ParticipantInfo.cs`

## Host authority rules

Host-only operations:

- seat assignment changes
- starting match
- final server decision on joins and leaves

Server validates host authority before applying actions.

## Join behavior

### New join

- Player join request:
  - if room has empty seat and match is not running -> auto-seat
  - else becomes spectator
- Spectator join request:
  - always spectator

### Reconnect join

If reconnect token matches a disconnected participant:

- old peer id mapping is replaced with new peer id
- old seat is restored
- if match was paused for that player, match resumes

## Seat assignment rules

Seat assignment is in `RoomState.SetSeat(...)`:

- One seat can have only one participant.
- One participant can occupy only one seat.
- Moving participant to new seat clears old seat.
- Setting empty value clears seat.

Lobby UI sends per-seat changes through RPC.

## Match lifecycle states

`RoomMatchLifecycle` values:

- `Lobby`
- `InMatch`
- `PausedReconnect`
- `MatchEnded`

When match starts, lifecycle switches to `InMatch`.

## Leave and disconnect behavior

### Voluntary leave

- Client sends leave request.
- Host removes participant or marks disconnected depending on context.

### Network disconnect

If seated player disconnects during running match:

- lifecycle becomes `PausedReconnect`
- match pause is triggered
- reconnect timer starts

If host disconnects:

- room is terminated locally and peers eventually receive server disconnect.

## Snapshot broadcasting

`LobbyManager` broadcasts:

- Room snapshot to all peers
- Match snapshot per peer (private visibility rules)

Spectators receive spectator snapshot (all hands visible).  
Players receive player snapshot (own hand visible, others hidden).
