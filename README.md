# up_poc

Proof of concept for coordinated updates of a Kubernetes-hosted backend and a Linux desktop app packaged as `.deb`.

## What is included

- `backend/UpPoc.Api` — ASP.NET Core Web API that orchestrates updates
- `scripts/` — Linux host scripts, sudoers sample, and systemd service sample
- `k8s/` — Kubernetes manifests for running the backend in cluster
- `avalonia-sample/` — sample Avalonia ViewModel that triggers update and polls status

## Main flow

1. Desktop app checks `GET /api/update/version`
2. User clicks update in Avalonia
3. Avalonia calls `POST /api/update/start`
4. API updates cluster workloads
5. API triggers host script to update the desktop package
6. Desktop app polls `GET /api/update/status`
7. App informs the user that restart is required or that update completed

## Local VM testing

Recommended setup:
- Windows host
- Debian 12 or Ubuntu 24.04 VM
- .NET 8 SDK
- optional: k3s for Kubernetes testing

### Backend local run

```bash
cd backend/UpPoc.Api
dotnet restore
dotnet run
```

### Fake desktop update test

Set in `appsettings.json`:
- `UseFakeDesktopUpdateScript: true`
- `UpdateScriptPath: /absolute/path/to/scripts/fake-update-desktop-app.sh`

Then call:

```bash
curl -X POST http://localhost:5080/api/update/start
curl http://localhost:5080/api/update/status
```

## Linux host notes

For real host-triggered updates:
- mount the script directory into the pod using `hostPath`
- allow the container user to execute the script with sudo or use a safer watcher design
- the sample here uses `sudo` for simplicity

## Production notes

This sample is a POC. Before production use, add:
- authentication/authorization on update endpoints
- persistent update state store
- rollout verification and rollback strategy
- package signing and trusted APT repository
- audit logging
