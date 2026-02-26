# Match Runtime Flow (End to End)

This is the real runtime story from click -> backend -> UI.

## 1) Host creates room

Screen path:

- MainMenu -> HostScreen -> Create Host

Backend path:

- `NetworkManager.StartHostSession`
- `LobbyManager.CreateHostedRoom`
- `GameManager.LoadLobby`

## 2) Clients discover and join

Screen path:

- MainMenu -> JoinScreen
- choose LAN room or type direct IP

Backend path:

- discovery list from `NetworkManager.GetDiscoveredRooms()`
- join via `NetworkManager.JoinRoomByIp`
- on connect, client sends `RequestJoinRoom`
- host processes in `LobbyManager.ServerHandleJoinRequest`
- host broadcasts room snapshot

## 3) Host sets seats

Screen path:

- LobbyScreen seat selectors -> Apply Seats

Backend path:

- client/host sends `RequestSeatChange`
- host validates host authority + lobby state
- `RoomState.SetSeat` updates seating map
- host broadcasts updated room snapshot

## 4) Host starts match

Screen path:

- LobbyScreen -> Start Match

Backend path:

- `RequestStartMatch`
- `MatchCoordinator.ServerStartMatch`
- `OmiRulesEngine.StartRound`
- match lifecycle -> `InMatch`
- host pushes snapshots and match-start event

All clients auto-open GameScreen on match start event.

## 5) Gameplay actions from GameScreen

Action panel drives phase-specific commands:

- Shuffle phase:
  - `SendShuffleAgainRequest`
  - `SendFinishShuffleRequest`
- Cut phase:
  - `SendCutDeckRequest`
- Trump select phase:
  - `SendSelectTrumpRequest`
- Trick play:
  - click a card -> `SendPlayCardRequest`

## 6) Server authoritative loop

For each gameplay RPC:

1. `NetworkRpc.Request...` runs on host.
2. Host calls `MatchCoordinator.ServerHandle...`.
3. Coordinator resolves sender seat and applies `MatchCommand`.
4. `OmiRulesEngine` validates and mutates state.
5. Coordinator broadcasts:
   - event RPCs (`PushCardPlayed`, `PushTrickResolved`, etc.)
   - new snapshots via `LobbyManager.BroadcastMatchSnapshotToAll`

## 7) Client rendering model

GameScreen does not trust local guesses.

It redraws from snapshot:

- hand counts and visible cards
- turn seat
- phase
- trump
- team credits and trick counts
- current trick cards

Local interaction is enabled only when:

- local role is player
- local seat equals current turn seat
- phase allows action

## 8) Reconnect and forfeit flow

If seated player drops:

- host marks offline
- match pauses (`PausedReconnect`)
- deadline set to now + 90s

If same reconnect token returns in time:

- seat and identity are restored
- match resumes

If deadline passes:

- coordinator forfeits that team
- match ends
