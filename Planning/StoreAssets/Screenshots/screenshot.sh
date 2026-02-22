#!/bin/bash
# Usage: ./screenshot.sh <device> <name>
#   device: iphone | ipad | both
#   name:   screenshot name (e.g. 01_home, 02_map, 03_events)
#
# Examples:
#   ./screenshot.sh iphone 01_home
#   ./screenshot.sh ipad 02_map
#   ./screenshot.sh both 03_events

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
IPHONE_DIR="$SCRIPT_DIR/iPhone_6.9"
IPAD_DIR="$SCRIPT_DIR/iPad_13"

IPHONE_UUID="6C786817-7318-4B18-97F7-043FD62B40EB"
IPAD_UUID="27EF59B9-8930-4DD7-98BB-C323A65716C8"

if [ -z "$1" ] || [ -z "$2" ]; then
    echo "Usage: ./screenshot.sh <device> <name>"
    echo "  device: iphone | ipad | both"
    echo "  name:   screenshot name (e.g. 01_home, 02_map)"
    exit 1
fi

DEVICE="$1"
NAME="$2"

take_screenshot() {
    local uuid="$1"
    local dir="$2"
    local device_name="$3"
    local file="$dir/${NAME}.png"

    if xcrun simctl io "$uuid" screenshot "$file" 2>/dev/null; then
        echo "Saved $device_name screenshot: $file"
    else
        echo "ERROR: Failed to capture $device_name. Is the simulator booted?"
    fi
}

case "$DEVICE" in
    iphone)
        take_screenshot "$IPHONE_UUID" "$IPHONE_DIR" "iPhone 17 Pro Max"
        ;;
    ipad)
        take_screenshot "$IPAD_UUID" "$IPAD_DIR" "iPad Pro 13\""
        ;;
    both)
        take_screenshot "$IPHONE_UUID" "$IPHONE_DIR" "iPhone 17 Pro Max"
        take_screenshot "$IPAD_UUID" "$IPAD_DIR" "iPad Pro 13\""
        ;;
    *)
        echo "Unknown device: $DEVICE (use iphone, ipad, or both)"
        exit 1
        ;;
esac
