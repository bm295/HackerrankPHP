# T.U.N.G Dining - FnB Application Requirement Note

## Basic information
- Restaurant name: **T.U.N.G Dining**
- Seating capacity target: **~60-70 seats**

## Product goal
Build an FnB management application for day-to-day restaurant operations.

## Functional scope
1. **Table management**
   - Track table code, seating, and status (available/reserved/occupied/cleaning).
2. **Menu management**
   - Maintain dish list with category, price, and availability.
3. **Order management**
   - Create dine-in orders by table.
   - Track order lifecycle (new/preparing/served/paid/cancelled).
4. **Reservation management**
   - Store booking time, guest count, and contact details.
   - Prevent overbooking beyond restaurant seating capacity.
5. **Dashboard overview**
   - Show table utilization, active orders, and upcoming reservations.

## Non-functional expectations
- API-first backend for integration with future web/mobile frontend.
- Simple seed data to accelerate local demo and testing.
- Clean, extensible structure for future modules (inventory, staff, reporting, POS).

