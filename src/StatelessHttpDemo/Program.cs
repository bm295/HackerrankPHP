using StatelessHttpDemo.Adapters.Persistence;
using StatelessHttpDemo.Application.DTOs;
using StatelessHttpDemo.Application.Ports.In;
using StatelessHttpDemo.Application.Ports.Out;
using StatelessHttpDemo.Application.UseCases;
using StatelessHttpDemo.Domain.Entities;
using StatelessHttpDemo.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddSingleton(InMemoryRestaurantContext.CreateSeeded());
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<ITableRepository, InMemoryTableRepository>();
builder.Services.AddSingleton<IMenuRepository, InMemoryMenuRepository>();
builder.Services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
builder.Services.AddSingleton<IReservationRepository, InMemoryReservationRepository>();

builder.Services.AddSingleton<InventoryConsumptionService>();
builder.Services.AddSingleton<IOrderUseCases, OrderUseCases>();
builder.Services.AddSingleton<IReservationUseCases, ReservationUseCases>();
builder.Services.AddSingleton<IReportingUseCases, ReportingUseCases>();

var app = builder.Build();
app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new
{
    service = "T Dining FnB management API",
    architecture = "Hexagonal (Ports & Adapters)",
    seatingCapacity = "60-70 seats"
}));

app.MapGet("/tables", async (ITableRepository tableRepository, CancellationToken ct) =>
{
    var tables = await tableRepository.ListAsync(ct);
    return Results.Ok(tables.Select(t => new { t.Code, t.Seats, status = t.Status.ToString() }));
});

app.MapPatch("/tables/{tableCode}/status", async (string tableCode, UpdateTableStatusRequest request, ITableRepository tableRepository, CancellationToken ct) =>
{
    var table = await tableRepository.GetByCodeAsync(tableCode, ct);
    if (table is null) return Results.NotFound(new { error = "Table not found." });

    table.UpdateStatus(request.Status);
    return Results.Ok(new { table.Code, table.Seats, status = table.Status.ToString() });
});

app.MapGet("/menu", async (IMenuRepository menuRepository, CancellationToken ct) =>
{
    var menu = await menuRepository.ListAsync(ct);
    return Results.Ok(menu.Select(m => new { m.Id, m.Name, m.Category, m.PriceVnd, m.IsAvailable }));
});

app.MapGet("/inventory", async (IInventoryRepository inventoryRepository, CancellationToken ct) =>
{
    var inventory = await inventoryRepository.ListAsync(ct);
    return Results.Ok(inventory.Select(i => new { i.Id, i.Name, i.Unit, i.Quantity }));
});

app.MapGet("/orders", async (IOrderUseCases useCases, CancellationToken ct) => Results.Ok(await useCases.ListOrdersAsync(ct)));

app.MapPost("/orders", async (CreateOrderCommand command, IOrderUseCases useCases, CancellationToken ct) =>
{
    return await Execute(async () => Results.Created("/orders", await useCases.CreateOrderAsync(command, ct)));
});

app.MapPost("/orders/{orderId:guid}/items/add", async (Guid orderId, UpdateOrderItemCommand command, IOrderUseCases useCases, CancellationToken ct) =>
    await Execute(async () => Results.Ok(await useCases.AddItemAsync(orderId, command, ct))));

app.MapPost("/orders/{orderId:guid}/items/remove", async (Guid orderId, UpdateOrderItemCommand command, IOrderUseCases useCases, CancellationToken ct) =>
    await Execute(async () => Results.Ok(await useCases.RemoveItemAsync(orderId, command, ct))));

app.MapPost("/orders/{orderId:guid}/send-to-kitchen", async (Guid orderId, IOrderUseCases useCases, CancellationToken ct) =>
    await Execute(async () => Results.Ok(await useCases.SendToKitchenAsync(orderId, ct))));

app.MapPost("/orders/{orderId:guid}/payments", async (Guid orderId, ProcessPaymentCommand command, IOrderUseCases useCases, CancellationToken ct) =>
    await Execute(async () => Results.Ok(await useCases.ProcessPaymentAsync(orderId, command, ct))));

app.MapPost("/orders/{orderId:guid}/close", async (Guid orderId, IOrderUseCases useCases, CancellationToken ct) =>
    await Execute(async () => Results.Ok(await useCases.CloseOrderAsync(orderId, ct))));

app.MapGet("/reservations", async (IReservationUseCases useCases, CancellationToken ct) => Results.Ok(await useCases.ListReservationsAsync(ct)));
app.MapPost("/reservations", async (CreateReservationCommand command, IReservationUseCases useCases, CancellationToken ct) =>
    await Execute(async () => Results.Created("/reservations", await useCases.CreateReservationAsync(command, ct))));

app.MapGet("/reports/daily/{date}", async (string date, IReportingUseCases useCases, CancellationToken ct) =>
{
    if (!DateOnly.TryParse(date, out var parsed)) return Results.BadRequest(new { error = "Date must be yyyy-MM-dd." });
    return Results.Ok(await useCases.GetDailyReportAsync(parsed, ct));
});

app.Run();

static async Task<IResult> Execute(Func<Task<IResult>> run)
{
    try
    {
        return await run();
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}

public sealed record UpdateTableStatusRequest(TableStatus Status);
