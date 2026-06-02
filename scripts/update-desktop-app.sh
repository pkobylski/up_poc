#!/usr/bin/env bash
set -euo pipefail

LOG_FILE="/var/log/myapp-desktop-update.log"
PACKAGE_NAME="myapp-desktop"
SYSTEMD_SERVICE="myapp-desktop.service"

log() {
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

log "Starting desktop package update"
log "Running apt-get update"
apt-get update >> "$LOG_FILE" 2>&1

log "Upgrading package ${PACKAGE_NAME}"
apt-get install --only-upgrade -y "$PACKAGE_NAME" >> "$LOG_FILE" 2>&1

log "Restarting service ${SYSTEMD_SERVICE}"
systemctl restart "$SYSTEMD_SERVICE" >> "$LOG_FILE" 2>&1

log "Desktop package update completed"
