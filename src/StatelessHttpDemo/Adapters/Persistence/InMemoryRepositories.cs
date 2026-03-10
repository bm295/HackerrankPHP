using StatelessHttpDemo.Application.Ports.Out;
using StatelessHttpDemo.Domain.Entities;

namespace StatelessHttpDemo.Adapters.Persistence;

public sealed class InMemoryOrderRepository(InMemoryRestaurantContext context) : IOrderRepository
{
    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        context.Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
        => Task.FromResult(context.Orders.FirstOrDefault(o => o.Id == orderId));

    public Task<IReadOnlyCollection<Order>> ListAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyCollection<Order>>(context.Orders);
}

public sealed class InMemoryTableRepository(InMemoryRestaurantContext context) : ITableRepository
{
    public Task<DiningTable?> GetByCodeAsync(string tableCode, CancellationToken cancellationToken)
        => Task.FromResult(context.Tables.FirstOrDefault(t => t.Code.Equals(tableCode, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyCollection<DiningTable>> ListAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyCollection<DiningTable>>(context.Tables);
}

public sealed class InMemoryMenuRepository(InMemoryRestaurantContext context) : IMenuRepository
{
    public Task<IReadOnlyCollection<MenuItem>> ListAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyCollection<MenuItem>>(context.MenuItems);

    public Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(context.MenuItems.FirstOrDefault(m => m.Id == id));
}

public sealed class InMemoryInventoryRepository(InMemoryRestaurantContext context) : IInventoryRepository
{
    public Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(context.InventoryItems.FirstOrDefault(x => x.Id == id));

    public Task<IReadOnlyCollection<InventoryItem>> ListAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyCollection<InventoryItem>>(context.InventoryItems);
}

public sealed class InMemoryPaymentRepository(InMemoryRestaurantContext context) : IPaymentRepository
{
    public Task AddAsync(Payment payment, CancellationToken cancellationToken)
    {
        context.Payments.Add(payment);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Payment>> ListByOrderAsync(Guid orderId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyCollection<Payment>>(context.Payments.Where(p => p.OrderId == orderId).ToList());
}

public sealed class InMemoryReservationRepository(InMemoryRestaurantContext context) : IReservationRepository
{
    public Task AddAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        context.Reservations.Add(reservation);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Reservation>> ListAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyCollection<Reservation>>(context.Reservations);
}
