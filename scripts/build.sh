#!/usr/bin/env bash
set -euo pipefail

SCRIPTS_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$(cd "$SCRIPTS_DIR/.." && pwd)"
CONFIG="${1:-Release}"

echo ""
echo "Building SaveManagerMSC ($CONFIG)..."
echo ""

rm -rf "$PROJECT_DIR/Output"
mkdir -p "$PROJECT_DIR/Output"

dotnet publish "$PROJECT_DIR/SaveManagerMSC.csproj" \
    -c "$CONFIG" \
    -r win-x64 \
    -o "$PROJECT_DIR/Output"

FILE_COUNT=$(find "$PROJECT_DIR/Output" -type f | wc -l)
EXE_PATH="$PROJECT_DIR/Output/SaveManagerMSC.exe"
EXE_SIZE=$(du -h "$EXE_PATH" | cut -f1)

echo ""
echo "Build finished: $CONFIG"
echo "Files: $FILE_COUNT"
echo "Output: $EXE_PATH ($EXE_SIZE)"
