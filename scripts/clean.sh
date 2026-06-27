#!/usr/bin/env bash
set -euo pipefail

SCRIPTS_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$(cd "$SCRIPTS_DIR/.." && pwd)"

echo ""
echo "Cleaning SaveManagerMSC..."
echo ""

if [ -d "$PROJECT_DIR/Output" ]; then
    rm -rf "$PROJECT_DIR/Output"
    echo "Removed Output/"
fi

if [ -d "$PROJECT_DIR/obj" ]; then
    rm -rf "$PROJECT_DIR/obj"
    echo "Removed obj/"
fi

echo ""
echo "Clean finished"
