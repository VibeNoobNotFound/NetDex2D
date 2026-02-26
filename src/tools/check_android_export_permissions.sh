#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CFG_PATH="$ROOT_DIR/export_presets.cfg"

if [[ ! -f "$CFG_PATH" ]]; then
  echo "ERROR: export_presets.cfg not found at: $CFG_PATH"
  exit 1
fi

ANDROID_PRESET_IDS=()
while IFS= read -r preset_id; do
  if [[ -n "$preset_id" ]]; then
    ANDROID_PRESET_IDS+=("$preset_id")
  fi
done < <(
  awk '
    /^\[preset\.[0-9]+\]$/ {
      section = $0
      gsub(/^\[preset\./, "", section)
      gsub(/\]$/, "", section)
      current_id = section
      next
    }

    /^platform="Android"$/ {
      if (current_id != "") {
        print current_id
      }
    }
  ' "$CFG_PATH"
)

if [[ "${#ANDROID_PRESET_IDS[@]}" -eq 0 ]]; then
  echo "ERROR: No Android preset found in $CFG_PATH"
  exit 1
fi

required_keys=(
  "permissions/internet"
  "permissions/access_network_state"
  "permissions/access_wifi_state"
  "permissions/change_wifi_multicast_state"
)

status=0

read_option_value() {
  local preset_id="$1"
  local key="$2"

  awk -v section="[preset.${preset_id}.options]" -v key="$key" '
    $0 == section {
      in_section = 1
      next
    }

    /^\[/ && in_section {
      exit
    }

    in_section && index($0, key "=") == 1 {
      sub(/^[^=]+=/, "", $0)
      print $0
      exit
    }
  ' "$CFG_PATH"
}

for preset_id in "${ANDROID_PRESET_IDS[@]}"; do
  echo "Checking Android preset.$preset_id ..."
  for key in "${required_keys[@]}"; do
    value="$(read_option_value "$preset_id" "$key")"
    if [[ "$value" != "true" ]]; then
      echo "  ERROR: ${key} is '${value:-<missing>}' (must be 'true')"
      status=1
    else
      echo "  OK: ${key}=true"
    fi
  done
done

if [[ "$status" -ne 0 ]]; then
  echo "Android networking permission check failed."
  exit 1
fi

echo "Android networking permission check passed."
