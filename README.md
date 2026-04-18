# T Dining API (.NET 10, C# preview)

This repository implements a restaurant backend for **T Dining** (target **60-70 seats**) using a **Hexagonal Architecture (Ports and Adapters)** structure with SQLite persistence and a transactional outbox for order creation.

## Architecture layout

```text
src/TDining.Api
  /Domain
    /Entities
    /Services
    /Events
  /Application
    /Ports/In
    /Ports/Out
    /UseCases
    /DTOs
  /Infrastructure
    /Persistence
    /Outbox
  Program.cs (API adapter/composition root)
```

## Implemented operations

- Order management
  - Create order for a table
  - Add/remove order items
  - Send order to kitchen
  - Close order
- Payment processing
  - Process payments per order
- Inventory tracking
  - Ingredient-level inventory seeded and deducted at payment
- Table/seat management
  - List tables and update table status
- Basic reporting
  - Daily report endpoint
- Reservation management with 70-seat capacity guard

## Run

```bash
dotnet run --project src/TDining.Api/TDining.Api.csproj
```

The app creates a local SQLite database file at `src/TDining.Api/tdining.db` on first run.

## Main endpoints

- `GET /`
- `GET /tables`
- `PATCH /tables/{tableCode}/status`
- `GET /menu`
- `GET /inventory`
- `GET /orders`
- `POST /orders`
- `POST /orders/{orderId}/items/add`
- `POST /orders/{orderId}/items/remove`
- `POST /orders/{orderId}/send-to-kitchen`
- `POST /orders/{orderId}/payments`
- `POST /orders/{orderId}/close`
- `GET /reservations`
- `POST /reservations`
- `GET /reports/daily/{date}`
