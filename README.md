# T.U.N.G Dining FnB Management API (ASP.NET Core)

This repository now provides a lightweight FnB backend for **T.U.N.G Dining** with target capacity **~60-70 seats**.

## Features

- Restaurant dashboard summary
- Table management (status updates)
- Menu management (list + create item)
- Order management (create order + update status)
- Reservation management with seating-capacity guardrails
- Stateless identity demo endpoint (`/whoami`)

## Run

```bash
dotnet run --project src/StatelessHttpDemo/StatelessHttpDemo.csproj
```

## Main endpoints

- `GET /` - API metadata
- `GET /dashboard` - high-level operational KPIs
- `GET /tables`
- `PATCH /tables/{tableCode}/status`
- `GET /menu`
- `POST /menu`
- `GET /orders`
- `POST /orders`
- `PATCH /orders/{orderId}/status`
- `GET /reservations`
- `POST /reservations`
- `GET /whoami`

## Example request

```bash
curl -X POST http://localhost:5000/reservations \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Nguyen Van A",
    "phoneNumber": "0900000000",
    "guestCount": 6,
    "bookingTime": "2026-05-01T12:00:00Z",
    "note": "Birthday table"
  }'
```

## Requirement note

See [`docs/tung-dining-requirements.md`](docs/tung-dining-requirements.md).
