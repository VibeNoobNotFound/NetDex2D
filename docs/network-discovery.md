# Network and Discovery

This document explains how hosting, joining, and LAN room discovery work.

## Transport basics

Transport class: `ENetMultiplayerPeer`  
Main file: `src/scripts/network/NetworkManager.cs`

Constants:

- Game TCP/UDP peer port: `7777`
- Discovery UDP base port: `7778`
- Discovery port range size: `12` (so `7778..7789`)
- Discovery protocol version: `1`
- Reconnect timeout: `90s`

## Host flow

Triggered by `HostScreen` -> `NetworkManager.StartHostSession`.

What happens:

1. Existing session is disconnected.
2. Discovery listener is stopped on host (host now advertises instead).
3. ENet server starts on port `7777`.
4. Multiplayer peer is assigned to `Multiplayer.MultiplayerPeer`.
5. Lobby room is created in `LobbyManager`.
6. Discovery advertise timer starts (every 1 second).

## Join flow

Triggered by `JoinScreen`:

- LAN room selection join, or
- direct IP join.

What happens:

1. Existing session is disconnected.
2. ENet client connects to host on `7777`.
3. On `ConnectedToServer`, client sends `RequestJoinRoom(...)` via RPC.
4. Client switches to Lobby screen.

## LAN discovery flow

### Advertise (host side)

- Host sends UDP broadcast packet each second.
- Packet includes room name, host name, counts, match state, room id, protocol version.
- Packet is broadcast to every discovery port in the range (`7778..7789`).

### Listen (client side)

- Client tries to bind first free port in `7778..7789`.
- This allows many local instances on same machine (important for local multi-instance testing).
- Poll timer reads packets every `0.2s`.
- Rooms expire after `6s` if no heartbeat.

### UI update stabilization

- Discovery UI updates are throttled (`0.25s`).
- `DiscoveryUpdated` only emits when room list fingerprint changes.
- This prevents rapid list flicker.

## Reconnect identity

Profile file: `user://player_profile.cfg`

Saved values:

- player name
- reconnect token

Reconnect token lets host match a returning player to old identity/seat.

## Disconnect behavior

Method: `NetworkManager.DisconnectSession(...)`

- Client leaving: sends `RequestLeaveRoom` to host, then closes peer.
- Host leaving: can broadcast message, closes server.
- Discovery listener restarts after disconnect.
- Local lobby/match state is cleared.

## Connection events

Handled by `NetworkManager`:

- `PeerConnected`
- `PeerDisconnected`
- `ConnectedToServer`
- `ConnectionFailed`
- `ServerDisconnected`

These events update room state and UI status labels.
