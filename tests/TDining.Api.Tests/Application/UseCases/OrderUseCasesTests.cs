using TDining.Api.Application.DTOs;
using TDining.Api.Application.Ports.Out;
using TDining.Api.Application.UseCases;
using TDining.Api.Domain.Entities;
using TDining.Api.Domain.Services;

namespace TDining.Api.Tests.Application.UseCases;

public sealed class OrderUseCasesTests
{
    [Fact]
    public async Task CreateOrderAsync_WhenTableAndMenuItemsExist_CreatesOrderAndOccupiesTable()
    {
        var table = new DiningTable("A1", 4);
        var menuItemId = Guid.NewGuid();
        var menuItem = new MenuItem(menuItemId, "Pho", "Main", 75_000m, true, []);
        var orderRepository = new InMemoryOrderRepository();
        var tableRepository = new InMemoryTableRepository(table);
        var menuRepository = new InMemoryMenuRepository(menuItem);
        var unitOfWork = new CountingUnitOfWork();
        var useCases = CreateUseCases(orderRepository, tableRepository, menuRepository, unitOfWork: unitOfWork);

        var result = await useCases.CreateOrderAsync(
            new CreateOrderCommand("A1", "  Linh  ", [new CreateOrderLineCommand(menuItemId, 2)]),
            CancellationToken.None);

        Assert.Equal("A1", result.TableCode);
        Assert.Equal("Linh", result.CustomerName);
        Assert.Equal("New", result.Status);
        Assert.Equal(150_000m, result.TotalAmountVnd);
        Assert.Single(result.Lines);
        Assert.Equal(TableStatus.Occupied, table.Status);
        Assert.Single(orderRepository.Orders);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenTableDoesNotExist_ThrowsInvalidOperationException()
    {
        var useCases = CreateUseCases(tableRepository: new InMemoryTableRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCases.CreateOrderAsync(new CreateOrderCommand("Z9", "Linh", []), CancellationToken.None));

        Assert.Equal("Table 'Z9' was not found.", exception.Message);
    }

    private static OrderUseCases CreateUseCases(
        InMemoryOrderRepository? orderRepository = null,
        InMemoryTableRepository? tableRepository = null,
        InMemoryMenuRepository? menuRepository = null,
        InMemoryInventoryRepository? inventoryRepository = null,
        InMemoryPaymentRepository? paymentRepository = null,
        CountingUnitOfWork? unitOfWork = null) =>
        new(
            orderRepository ?? new InMemoryOrderRepository(),
            tableRepository ?? new InMemoryTableRepository(),
            menuRepository ?? new InMemoryMenuRepository(),
            inventoryRepository ?? new InMemoryInventoryRepository(),
            paymentRepository ?? new InMemoryPaymentRepository(),
            unitOfWork ?? new CountingUnitOfWork(),
            new InventoryConsumptionService());

    private sealed class InMemoryOrderRepository(params Order[] orders) : IOrderRepository
    {
        private readonly List<Order> _orders = [.. orders];
        public IReadOnlyCollection<Order> Orders => _orders;
        public Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            _orders.Add(order);
            return Task.CompletedTask;
        }
        public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken) => Task.FromResult(_orders.FirstOrDefault(order => order.Id == orderId));
        public Task<IReadOnlyCollection<Order>> ListAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<Order>>(_orders);
    }

    private sealed class InMemoryTableRepository(params DiningTable[] tables) : ITableRepository
    {
        private readonly List<DiningTable> _tables = [.. tables];
        public Task<DiningTable?> GetByCodeAsync(string tableCode, CancellationToken cancellationToken) => Task.FromResult(_tables.FirstOrDefault(table => table.Code == tableCode));
        public Task<IReadOnlyCollection<DiningTable>> ListAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<DiningTable>>(_tables);
    }

    private sealed class InMemoryMenuRepository(params MenuItem[] menuItems) : IMenuRepository
    {
        private readonly List<MenuItem> _menuItems = [.. menuItems];
        public Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(_menuItems.FirstOrDefault(item => item.Id == id));
        public Task<IReadOnlyCollection<MenuItem>> ListAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<MenuItem>>(_menuItems);
    }

    private sealed class InMemoryInventoryRepository(params InventoryItem[] inventoryItems) : IInventoryRepository
    {
        private readonly List<InventoryItem> _inventoryItems = [.. inventoryItems];
        public Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(_inventoryItems.FirstOrDefault(item => item.Id == id));
        public Task<IReadOnlyCollection<InventoryItem>> ListAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<InventoryItem>>(_inventoryItems);
    }

    private sealed class InMemoryPaymentRepository : IPaymentRepository
    {
        private readonly List<Payment> _payments = [];
        public Task AddAsync(Payment payment, CancellationToken cancellationToken)
        {
            _payments.Add(payment);
            return Task.CompletedTask;
        }
        public Task<IReadOnlyCollection<Payment>> ListByOrderAsync(Guid orderId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<Payment>>(_payments.Where(payment => payment.OrderId == orderId).ToList());
    }

    private sealed class CountingUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCount { get; private set; }
        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCount++;
            return Task.CompletedTask;
        }
    }
}
