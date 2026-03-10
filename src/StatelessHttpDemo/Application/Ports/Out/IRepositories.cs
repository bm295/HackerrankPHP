using StatelessHttpDemo.Domain.Entities;

namespace StatelessHttpDemo.Application.Ports.Out;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Order>> ListAsync(CancellationToken cancellationToken);
}

public interface ITableRepository
{
    Task<DiningTable?> GetByCodeAsync(string tableCode, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DiningTable>> ListAsync(CancellationToken cancellationToken);
}

public interface IMenuRepository
{
    Task<IReadOnlyCollection<MenuItem>> ListAsync(CancellationToken cancellationToken);
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<InventoryItem>> ListAsync(CancellationToken cancellationToken);
}

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Payment>> ListByOrderAsync(Guid orderId, CancellationToken cancellationToken);
}

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Reservation>> ListAsync(CancellationToken cancellationToken);
}
