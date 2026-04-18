using Microsoft.EntityFrameworkCore;
using TDining.Api.Application.Ports.Out;
using TDining.Api.Domain.Entities;

namespace TDining.Api.Infrastructure.Persistence;

public sealed class EfOrderRepository(TDiningDbContext dbContext) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken)
        => await dbContext.Orders.AddAsync(order, cancellationToken);

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
        => dbContext.Orders.FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);

    public async Task<IReadOnlyCollection<Order>> ListAsync(CancellationToken cancellationToken)
        => await dbContext.Orders
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToListAsync(cancellationToken);
}

public sealed class EfTableRepository(TDiningDbContext dbContext) : ITableRepository
{
    public Task<DiningTable?> GetByCodeAsync(string tableCode, CancellationToken cancellationToken)
    {
        var normalizedCode = tableCode.Trim().ToUpperInvariant();
        return dbContext.Tables.FirstOrDefaultAsync(table => table.Code.ToUpper() == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyCollection<DiningTable>> ListAsync(CancellationToken cancellationToken)
        => await dbContext.Tables
            .OrderBy(table => table.Code)
            .ToListAsync(cancellationToken);
}

public sealed class EfMenuRepository(TDiningDbContext dbContext) : IMenuRepository
{
    public async Task<IReadOnlyCollection<MenuItem>> ListAsync(CancellationToken cancellationToken)
        => await dbContext.MenuItems
            .OrderBy(item => item.Category)
            .ThenBy(item => item.Name)
            .ToListAsync(cancellationToken);

    public Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.MenuItems.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
}

public sealed class EfInventoryRepository(TDiningDbContext dbContext) : IInventoryRepository
{
    public Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.InventoryItems.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<InventoryItem>> ListAsync(CancellationToken cancellationToken)
        => await dbContext.InventoryItems
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);
}

public sealed class EfPaymentRepository(TDiningDbContext dbContext) : IPaymentRepository
{
    public async Task AddAsync(Payment payment, CancellationToken cancellationToken)
        => await dbContext.Payments.AddAsync(payment, cancellationToken);

    public async Task<IReadOnlyCollection<Payment>> ListByOrderAsync(Guid orderId, CancellationToken cancellationToken)
        => await dbContext.Payments
            .Where(payment => payment.OrderId == orderId)
            .OrderBy(payment => payment.PaidAtUtc)
            .ToListAsync(cancellationToken);
}

public sealed class EfReservationRepository(TDiningDbContext dbContext) : IReservationRepository
{
    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken)
        => await dbContext.Reservations.AddAsync(reservation, cancellationToken);

    public async Task<IReadOnlyCollection<Reservation>> ListAsync(CancellationToken cancellationToken)
        => await dbContext.Reservations
            .OrderBy(reservation => reservation.BookingTimeUtc)
            .ToListAsync(cancellationToken);
}

public sealed class EfUnitOfWork(TDiningDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await dbContext.SaveChangesAsync(cancellationToken);
}
