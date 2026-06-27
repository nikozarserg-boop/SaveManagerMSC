#!/usr/bin/env bash
set -euo pipefail

SCRIPTS_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$(cd "$SCRIPTS_DIR/.." && pwd)"
CONFIG="${1:-Release}"

echo ""
echo "Rebuilding SaveManagerMSC ($CONFIG)..."
echo ""

"$SCRIPTS_DIR/clean.sh"
echo ""
"$SCRIPTS_DIR/build.sh" "$CONFIG" "${@:2}"

echo ""
echo "Rebuild finished: $CONFIG"
