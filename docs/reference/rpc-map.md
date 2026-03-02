# RPC Map

Primary RPC bridge: `src/scripts/network/NetworkRpc.cs`.

## `[Rpc]` methods

| File | Method | Parameters | RPC Attribute |
|---|---|---|---|
| `scripts/network/NetworkRpc.cs` | `PushCardPlayed` | `string payloadJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushCreditsUpdated` | `string payloadJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushMatchEnded` | `string payloadJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushMatchSnapshot` | `string snapshotJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushMatchStarted` | `string payloadJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushPausedForReconnect` | `int disconnectedPeerId, double reconnectDeadlineUnixSeconds` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushPrivateHand` | `string payloadJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushRoomSnapshot` | `string snapshotJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushRoundResolved` | `string payloadJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushSeatMap` | `string seatSnapshotJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushServerMessage` | `string message` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushSpectatorHands` | `string payloadJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `PushTrickResolved` | `string payloadJson` | `MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestCutDeck` | `int cutIndex` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestFinishShuffle` | `none` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestJoinRoom` | `string playerName, int requestedRole, string reconnectToken` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestLeaveRoom` | `none` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestPlayCard` | `string cardId` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestSeatChange` | `int targetSeat, int targetPeerId = -1` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestSelectTrump` | `int suit` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestSetAiOptions` | `bool autoFill, int difficulty` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestShuffleAgain` | `int seed` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |
| `scripts/network/NetworkRpc.cs` | `RequestStartMatch` | `none` | `MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable` |

## Runtime flow summary

- Client wrappers (`Send*`) call `RpcId(1, ...)` to host authority.
- Host receives `Request*` RPCs, validates via `LobbyManager`/`MatchCoordinator`, then broadcasts snapshots/events.
- Client receives `Push*` RPCs and updates local room/match stores.
