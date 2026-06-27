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

rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

dotnet publish "$PROJECT_DIR/SaveManagerMSC.csproj" \
    -c "$CONFIG" \
    -r "$RID" \
    -o "$OUTPUT_DIR"

FILE_COUNT=$(find "$OUTPUT_DIR" -type f | wc -l)

echo ""
echo "Published: $CONFIG / $RID"
echo "Output: $OUTPUT_DIR/"
echo ""
echo "Files in publish: $FILE_COUNT"

if [ "$FILE_COUNT" -eq "1" ]; then
    EXE_PATH=$(find "$OUTPUT_DIR" -type f | head -1)
    EXE_SIZE=$(du -h "$EXE_PATH" | cut -f1)
    echo "Single file: $(basename "$EXE_PATH") ($EXE_SIZE)"
    echo ""
    echo "Run on Windows:"
    echo "  $EXE_PATH"
else
    echo "Warning: expected 1 file, got $FILE_COUNT"
    ls -lh "$OUTPUT_DIR"
fi
