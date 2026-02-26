# Troubleshooting and Local Testing

This document covers common issues and quick checks.

## 1) "Discovery bind failed"

Cause:

- Multiple local instances trying same discovery port.

Current behavior:

- Client now binds first free port in `7778..7789`.
- Host advertises to entire same range.

If still failing:

- More than 12 local listeners may be running.
- Close extra instances.
- Restart game instances.

Android note:

- If error includes `CantCreate`, check Android export permissions in `src/export_presets.cfg`.
- Required: `internet`, `access_network_state`, `access_wifi_state`, `change_wifi_multicast_state`.
- You can validate with: `bash src/tools/check_android_export_permissions.sh`
- If auto discovery is blocked by network/router, use Direct IP join.

## 2) Room list flickers / selection resets

Current behavior includes stabilization:

- room expiry: 6s
- UI update throttle: 0.25s
- fingerprint-based update emit
- Join UI keeps selected room key when list updates

If selection still disappears:

- room may actually be expiring
- host may not be advertising (host paused/stopped/disconnected)

## 3) Cannot start match

Host start validation requires:

- requester must be host
- all 4 seats assigned
- all seated players connected

Check lobby status text and participant list.

## 4) Cannot play card

Possible reasons:

- not your turn
- not in `TrickPlay` phase
- attempted card does not exist in your hand
- you did not follow lead suit while still holding lead suit

Server rejects invalid plays.

## 5) Match pauses unexpectedly

Cause:

- seated player disconnected during match.

Behavior:

- phase becomes `PausedReconnect`
- reconnect deadline starts (90s)
- if deadline misses, disconnected team forfeits

## 6) Leave room seems wrong

Expected behavior:

- from lobby: `DisconnectSession("Left room")` + return to MainMenu
- from pause menu in match: `DisconnectSession("Left match")` + return to MainMenu

On host leave:

- server closes, clients should receive disconnect and return to menu.

## 7) Local multi-instance test checklist (same machine)

1. Open first instance -> Host -> create room.
2. Open second instance -> Join -> select room -> join player.
3. Open third instance -> Join -> select room -> join player/spectator.
4. Repeat until seats filled.
5. Start match from host lobby.
6. Test:
   - shuffle stage actions
   - cut
   - trump selection
   - trick plays
   - back-to-lobby and return-to-game
   - disconnect/reconnect inside 90s

## 8) Useful dev commands

From repository root:

- `cd src`
- `dotnet build NetDex.sln`

Open project:

- `src/project.godot`
