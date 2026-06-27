#!/usr/bin/env bash
set -euo pipefail

SCRIPTS_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$(cd "$SCRIPTS_DIR/.." && pwd)"
CONFIG="${1:-Release}"
RID="${2:-win-x64}"
OUTPUT_DIR="$PROJECT_DIR/publish/$RID"

echo ""
echo "Publishing SaveManagerMSC ($CONFIG / $RID)"
echo ""

dotnet publish "$PROJECT_DIR/SaveManagerMSC.csproj" \
    -c "$CONFIG" \
    -r "$RID" \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:EnableCompressionInSingleFile=true \
    -o "$OUTPUT_DIR"

echo ""
echo "Published: $CONFIG / $RID"
echo "Output: $OUTPUT_DIR/"
echo ""
echo "Run on Windows:"
echo "  $OUTPUT_DIR/SaveManagerMSC.exe"
