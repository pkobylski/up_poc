#!/usr/bin/env bash
set -euo pipefail

TARGET_VERSION="${1:-1.0.1}"
STATE_DIR="/tmp/up_poc"
LOG_FILE="${STATE_DIR}/fake-desktop-update.log"
VERSION_FILE="${STATE_DIR}/desktop-version.txt"

mkdir -p "$STATE_DIR"

echo "[$(date '+%Y-%m-%d %H:%M:%S')] Simulating desktop update to ${TARGET_VERSION}" >> "$LOG_FILE"
sleep 4
echo "$TARGET_VERSION" > "$VERSION_FILE"
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Desktop update simulation completed" >> "$LOG_FILE"
