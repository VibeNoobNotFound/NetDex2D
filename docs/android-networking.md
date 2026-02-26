# Android Networking (Host/Join/Discovery)

This page explains why Android can fail with errors like `CantCreate` and how to fix it.

## Typical symptoms

- Host create fails with `CantCreate`.
- Join fails with `CantCreate`.
- Discovery shows `Discovery bind failed ... CantCreate`.
- LAN room list stays empty on Android.

## Root cause

Android export permissions are missing in `src/export_presets.cfg`.

For LAN multiplayer + UDP discovery, Android preset must enable:

- `permissions/internet=true`
- `permissions/access_network_state=true`
- `permissions/access_wifi_state=true`
- `permissions/change_wifi_multicast_state=true`

If these are disabled, ENet/UDP socket creation can fail, which appears as `CantCreate`.

## Quick verification

Run from repository root:

```bash
bash src/tools/check_android_export_permissions.sh
```

Expected:

- `Android networking permission check passed.`

## If discovery still fails on some networks

- Some Wi-Fi routers/APs block broadcast/multicast traffic.
- In that case, use **Direct IP join** (already supported in Join UI).
- Host can share LAN IP shown on host screen status.

## Device test matrix

1. Android host + Android client on same Wi-Fi.
2. Android host + desktop client.
3. Desktop host + Android client.
4. Confirm LAN discovery and Direct IP join.

## Optional log capture

Use `adb logcat` while pressing Host/Join on Android to confirm socket creation and bind behavior.
