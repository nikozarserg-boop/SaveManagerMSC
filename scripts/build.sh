#!/usr/bin/env bash
set -euo pipefail

SCRIPTS_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$(cd "$SCRIPTS_DIR/.." && pwd)"
CONFIG="${1:-Release}"

echo ""
echo "Building SaveManagerMSC ($CONFIG)..."
echo ""

dotnet build "$PROJECT_DIR/SaveManagerMSC.csproj" -c "$CONFIG" "${@:2}"

echo ""
echo "Build finished: $CONFIG"
echo "Output: $PROJECT_DIR/Output/"
