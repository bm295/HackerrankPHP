# HackerrankPHP → C# 14 Stateless HTTP Demo

This repository now contains a C# web API demo focused on **stateless HTTP handling**.

## What changed

- All original PHP practice files were moved into [`php/`](./php) for archival.
- A new ASP.NET Core minimal API was added in [`src/StatelessHttpDemo`](./src/StatelessHttpDemo).
- The project is configured for **C# 14 (preview)** via `<LangVersion>preview</LangVersion>` and `net10.0`.

## Stateless HTTP handling demonstrated

The API avoids server-side session state. Every request carries all required context:

- `Authorization: Bearer <userId>:<role>`
- `X-Request-Id` (optional for request tracing)
- `X-Tenant-Id` (required for `/echo`)

Endpoints:

- `GET /` — service metadata
- `GET /whoami` — derives user info from token on each request
- `POST /echo` — validates tenant header and returns payload + contextual identity

## Run

```bash
dotnet run --project src/StatelessHttpDemo/StatelessHttpDemo.csproj
```

## Quick test

```powershell
curl -i http://localhost:5000/whoami

curl.exe -i http://localhost:5000/whoami `
  -H "Authorization: Bearer alice:admin" `
  -H "X-Request-Id: req-123"

curl.exe -i http://localhost:5000/echo `
  -H "Content-Type: application/json" `
  -H "Authorization: Bearer alice:admin" `
  -H "X-Tenant-Id: tenant-a" `
  -d "{\"message\":\"hello\",\"timestampUtc\":\"2026-01-01T00:00:00Z\"}"
```
