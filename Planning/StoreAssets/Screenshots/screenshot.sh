#!/bin/bash
# Usage: ./screenshot.sh <device> <name>
#   device: iphone | ipad | both
#   name:   screenshot name (e.g. 01_welcome, 02_home)
#
# Screenshots are saved to fastlane/screenshots/en-US/ with device-prefixed
# names matching the convention expected by frameit and deliver:
#   iPhone_6.9_01_welcome.png, iPad_13_01_welcome.png
#
# Names should match the Framefile.json filter values (e.g. 01_welcome, 02_home).
#
# Examples:
#   ./screenshot.sh iphone 01_welcome
#   ./screenshot.sh ipad 02_home
#   ./screenshot.sh both 03_explore

REPO_ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
SCREENSHOTS_DIR="$REPO_ROOT/fastlane/screenshots/en-US"

IPHONE_UUID="6C786817-7318-4B18-97F7-043FD62B40EB"
IPAD_UUID="27EF59B9-8930-4DD7-98BB-C323A65716C8"

if [ -z "$1" ] || [ -z "$2" ]; then
    echo "Usage: ./screenshot.sh <device> <name>"
    echo "  device: iphone | ipad | both"
    echo "  name:   screenshot name (e.g. 01_welcome, 02_home)"
    echo ""
    echo "Screenshots are saved to fastlane/screenshots/en-US/"
    exit 1
fi

DEVICE="$1"
NAME="$2"

mkdir -p "$SCREENSHOTS_DIR"

take_screenshot() {
    local uuid="$1"
    local device_prefix="$2"
    local device_name="$3"
    local file="$SCREENSHOTS_DIR/${device_prefix}_${NAME}.png"

    if xcrun simctl io "$uuid" screenshot "$file" 2>/dev/null; then
        echo "Saved $device_name screenshot: $file"
    else
        echo "ERROR: Failed to capture $device_name. Is the simulator booted?"
    fi
}

case "$DEVICE" in
    iphone)
        take_screenshot "$IPHONE_UUID" "iPhone_6.9" "iPhone 17 Pro Max"
        ;;
    ipad)
        take_screenshot "$IPAD_UUID" "iPad_13" "iPad Pro 13\""
        ;;
    both)
        take_screenshot "$IPHONE_UUID" "iPhone_6.9" "iPhone 17 Pro Max"
        take_screenshot "$IPAD_UUID" "iPad_13" "iPad Pro 13\""
        ;;
    *)
        echo "Unknown device: $DEVICE (use iphone, ipad, or both)"
        exit 1
        ;;
esac
