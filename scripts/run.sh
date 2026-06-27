#!/usr/bin/env bash
set -euo pipefail

SCRIPTS_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$(cd "$SCRIPTS_DIR/.." && pwd)"
CONFIG="${1:-Release}"
APP_PATH="$PROJECT_DIR/Output/SaveManagerMSC.exe"

if [ ! -f "$APP_PATH" ]; then
    echo "Build not found at: $APP_PATH"
    echo "Run ./scripts/build.sh $CONFIG first."
    exit 1
fi

REQUIRED_FRAMEWORK="Microsoft.WindowsDesktop.App"
FRAMEWORK_VERSION="8.0.0"

if ! dotnet --list-runtimes 2>/dev/null | grep -q "$REQUIRED_FRAMEWORK.*$FRAMEWORK_VERSION"; then
    echo ""
    echo "This application requires Windows Forms ($REQUIRED_FRAMEWORK v$FRAMEWORK_VERSION)"
    echo "and can only run on Windows."
    echo ""
    echo "Run on Windows:"
    echo "  Output/SaveManagerMSC.exe"
    echo ""
    echo "Or publish as single-file:"
    echo "  ./scripts/publish.sh"
    echo "  publish/win-x64/SaveManagerMSC.exe"
    exit 1
fi

echo ""
echo "Running SaveManagerMSC ($CONFIG)..."
echo ""

dotnet "$APP_PATH"
