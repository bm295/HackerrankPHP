using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton(RestaurantStore.CreateSeeded());

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new
{
    service = "T.U.N.G Dining FnB management API",
    restaurant = "T.U.N.G Dining",
    seatingCapacity = "~60-70 seats",
    modules = new[] { "tables", "menu", "orders", "reservations", "dashboard" }
}));

app.MapGet("/dashboard", (RestaurantStore store) =>
{
    return Results.Ok(new
    {
        restaurant = store.Profile,
        tables = new
        {
            total = store.Tables.Count,
            seats = store.Tables.Sum(t => t.SeatCount),
            occupied = store.Tables.Count(t => t.Status == TableStatus.Occupied),
            available = store.Tables.Count(t => t.Status == TableStatus.Available)
        },
        menu = new
        {
            items = store.Menu.Count,
            avgPrice = Math.Round(store.Menu.Average(m => m.PriceVnd), 0)
        },
        activeOrders = store.Orders.Count(o => o.Status is OrderStatus.New or OrderStatus.Preparing or OrderStatus.Served),
        upcomingReservations = store.Reservations.Count(r => r.Status == ReservationStatus.Confirmed && r.BookingTime >= DateTime.UtcNow)
    });
});

app.MapGet("/tables", (RestaurantStore store) => Results.Ok(store.Tables));

app.MapPatch("/tables/{tableCode}/status", (string tableCode, UpdateTableStatusRequest request, RestaurantStore store) =>
{
    var table = store.Tables.FirstOrDefault(t => t.Code.Equals(tableCode, StringComparison.OrdinalIgnoreCase));
    if (table is null)
    {
        return Results.NotFound(new { error = $"Table '{tableCode}' was not found." });
    }

    table.Status = request.Status;
    return Results.Ok(table);
});

app.MapGet("/menu", (RestaurantStore store) => Results.Ok(store.Menu));

app.MapPost("/menu", (CreateMenuItemRequest request, RestaurantStore store) =>
{
    var item = new MenuItem(Guid.NewGuid(), request.Name.Trim(), request.Category.Trim(), request.PriceVnd, request.IsAvailable);
    store.Menu.Add(item);
    return Results.Created($"/menu/{item.Id}", item);
});

app.MapGet("/orders", (RestaurantStore store) => Results.Ok(store.Orders));

app.MapPost("/orders", (CreateOrderRequest request, RestaurantStore store) =>
{
    var table = store.Tables.FirstOrDefault(t => t.Code.Equals(request.TableCode, StringComparison.OrdinalIgnoreCase));
    if (table is null)
    {
        return Results.BadRequest(new { error = $"Table '{request.TableCode}' was not found." });
    }

    var missingItem = request.Items.FirstOrDefault(i => store.Menu.All(m => m.Id != i.MenuItemId));
    if (missingItem is not null)
    {
        return Results.BadRequest(new { error = $"Menu item '{missingItem.MenuItemId}' does not exist." });
    }

    var orderItems = request.Items
        .Select(i =>
        {
            var menu = store.Menu.First(m => m.Id == i.MenuItemId);
            return new OrderItem(i.MenuItemId, menu.Name, i.Quantity, menu.PriceVnd, i.Quantity * menu.PriceVnd);
        })
        .ToList();

    var order = new Order(
        Guid.NewGuid(),
        table.Code,
        request.CustomerName.Trim(),
        OrderStatus.New,
        DateTime.UtcNow,
        orderItems,
        orderItems.Sum(i => i.LineTotalVnd));

    store.Orders.Add(order);
    table.Status = TableStatus.Occupied;

    return Results.Created($"/orders/{order.Id}", order);
});

app.MapPatch("/orders/{orderId:guid}/status", (Guid orderId, UpdateOrderStatusRequest request, RestaurantStore store) =>
{
    var order = store.Orders.FirstOrDefault(o => o.Id == orderId);
    if (order is null)
    {
        return Results.NotFound(new { error = $"Order '{orderId}' was not found." });
    }

    order.Status = request.Status;
    if (request.Status is OrderStatus.Paid or OrderStatus.Cancelled)
    {
        var table = store.Tables.FirstOrDefault(t => t.Code.Equals(order.TableCode, StringComparison.OrdinalIgnoreCase));
        if (table is not null)
        {
            table.Status = TableStatus.Cleaning;
        }
    }

    return Results.Ok(order);
});

app.MapGet("/reservations", (RestaurantStore store) => Results.Ok(store.Reservations));

app.MapPost("/reservations", (CreateReservationRequest request, RestaurantStore store) =>
{
    var reservedGuests = store.Reservations
        .Where(r => r.Status == ReservationStatus.Confirmed && r.BookingTime.Date == request.BookingTime.Date)
        .Sum(r => r.GuestCount);

    if (reservedGuests + request.GuestCount > store.Profile.MaxCapacity)
    {
        return Results.BadRequest(new
        {
            error = "Reservation exceeds the configured seating capacity.",
            capacity = store.Profile.MaxCapacity,
            requestedGuests = request.GuestCount,
            currentlyReserved = reservedGuests
        });
    }

    var reservation = new Reservation(
        Guid.NewGuid(),
        request.CustomerName.Trim(),
        request.PhoneNumber.Trim(),
        request.GuestCount,
        request.BookingTime,
        ReservationStatus.Confirmed,
        request.Note?.Trim());

    store.Reservations.Add(reservation);

    return Results.Created($"/reservations/{reservation.Id}", reservation);
});

app.MapGet("/whoami", (HttpRequest request) =>
{
    if (!TryParseBearerToken(request, out var claims, out _))
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new
    {
        subject = claims.FindFirstValue(ClaimTypes.NameIdentifier),
        role = claims.FindFirstValue(ClaimTypes.Role),
        requestId = request.Headers["X-Request-Id"].ToString()
    });
});

app.Run();

static bool TryParseBearerToken(HttpRequest request, out ClaimsPrincipal claims, out string? error)
{
    claims = new ClaimsPrincipal();
    error = null;

    var authHeader = request.Headers.Authorization.ToString();
    if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        error = "Missing Bearer token.";
        return false;
    }

    var token = authHeader["Bearer ".Length..].Trim();

    var parts = token.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (parts.Length != 2)
    {
        error = "Token format must be `userId:role`.";
        return false;
    }

    var identity = new ClaimsIdentity("Bearer");
    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, parts[0]));
    identity.AddClaim(new Claim(ClaimTypes.Role, parts[1]));
    claims = new ClaimsPrincipal(identity);

    return true;
}

internal sealed class RestaurantStore
{
    public RestaurantProfile Profile { get; init; } = default!;
    public List<DiningTable> Tables { get; } = [];
    public List<MenuItem> Menu { get; } = [];
    public List<Order> Orders { get; } = [];
    public List<Reservation> Reservations { get; } = [];

    public static RestaurantStore CreateSeeded()
    {
        var store = new RestaurantStore
        {
            Profile = new RestaurantProfile("T.U.N.G Dining", 60, 70)
        };

        store.Tables.AddRange([
            new DiningTable("T1", 2, TableStatus.Available),
            new DiningTable("T2", 2, TableStatus.Available),
            new DiningTable("T3", 4, TableStatus.Available),
            new DiningTable("T4", 4, TableStatus.Reserved),
            new DiningTable("T5", 6, TableStatus.Available),
            new DiningTable("T6", 6, TableStatus.Available),
            new DiningTable("P1", 10, TableStatus.Available),
            new DiningTable("P2", 12, TableStatus.Available),
            new DiningTable("P3", 24, TableStatus.Available)
        ]);

        store.Menu.AddRange([
            new MenuItem(Guid.Parse("d2f9b7dc-722f-4410-ad5d-24e4ca099301"), "Phở bò tái", "Main", 95000, true),
            new MenuItem(Guid.Parse("7398a160-7b24-44ea-9f36-5a15bf54ed4d"), "Gỏi cuốn tôm thịt", "Starter", 70000, true),
            new MenuItem(Guid.Parse("ce98ded5-5be8-44e3-92e2-239f84c8887c"), "Cà phê sữa đá", "Beverage", 45000, true)
        ]);

        return store;
    }
}

internal sealed record RestaurantProfile(string Name, int MinCapacity, int MaxCapacity);

internal sealed class DiningTable(string code, int seatCount, TableStatus status)
{
    public string Code { get; set; } = code;
    public int SeatCount { get; set; } = seatCount;
    public TableStatus Status { get; set; } = status;
}

internal enum TableStatus
{
    Available,
    Reserved,
    Occupied,
    Cleaning
}

internal sealed record MenuItem(Guid Id, string Name, string Category, decimal PriceVnd, bool IsAvailable);
internal sealed record CreateMenuItemRequest(string Name, string Category, decimal PriceVnd, bool IsAvailable = true);

internal sealed class Order(Guid id, string tableCode, string customerName, OrderStatus status, DateTime createdAtUtc, List<OrderItem> items, decimal totalAmountVnd)
{
    public Guid Id { get; set; } = id;
    public string TableCode { get; set; } = tableCode;
    public string CustomerName { get; set; } = customerName;
    public OrderStatus Status { get; set; } = status;
    public DateTime CreatedAtUtc { get; set; } = createdAtUtc;
    public List<OrderItem> Items { get; set; } = items;
    public decimal TotalAmountVnd { get; set; } = totalAmountVnd;
}

internal sealed record OrderItem(Guid MenuItemId, string MenuItemName, int Quantity, decimal UnitPriceVnd, decimal LineTotalVnd);
internal sealed record CreateOrderLineRequest(Guid MenuItemId, int Quantity);
internal sealed record CreateOrderRequest(string TableCode, string CustomerName, List<CreateOrderLineRequest> Items);
internal sealed record UpdateOrderStatusRequest(OrderStatus Status);

internal enum OrderStatus
{
    New,
    Preparing,
    Served,
    Paid,
    Cancelled
}

internal sealed record Reservation(Guid Id, string CustomerName, string PhoneNumber, int GuestCount, DateTime BookingTime, ReservationStatus Status, string? Note);
internal sealed record CreateReservationRequest(string CustomerName, string PhoneNumber, int GuestCount, DateTime BookingTime, string? Note);

internal enum ReservationStatus
{
    Confirmed,
    Seated,
    Cancelled,
    Completed
}

internal sealed record UpdateTableStatusRequest(TableStatus Status);
