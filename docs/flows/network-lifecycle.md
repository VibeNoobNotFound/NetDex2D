# Network Lifecycle

## Host
1. `NetworkManager.StartHostSession` validates names and creates ENet server (`7777`).
2. Multiplayer peer assigned to `Multiplayer.MultiplayerPeer`.
3. Host room created in `LobbyManager`.
4. LAN advertisement starts on discovery channel (`7778` range handling in code).

## Client
1. Discovery listener receives room ads (`PacketPeerUdp`).
2. Client joins by selected room IP or direct IP fallback.
3. On `ConnectedToServer`, client sends `RequestJoinRoom` via `NetworkRpc`.
4. Host validates and pushes room snapshot/match snapshot.

## Reliability model
- Gameplay commands are host-authoritative (`Request*` -> server validate -> snapshot/event push).
- Disconnects and reconnects are handled with timeout/forfeit flow in `MatchCoordinator` + lobby state broadcasts.
