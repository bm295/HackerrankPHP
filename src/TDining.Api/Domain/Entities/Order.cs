using TDining.Api.Domain.Events;

namespace TDining.Api.Domain.Entities;

public enum OrderStatus
{
    New,
    Preparing,
    Served,
    Closed,
    Cancelled
}

public sealed class Order
{
    private readonly List<IDomainEvent> _domainEvents = [];

    private Order()
    {
    }

    public Order(Guid id, string tableCode, string customerName, DateTime createdAtUtc)
    {
        Id = id;
        TableCode = tableCode;
        CustomerName = customerName;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }
    public string TableCode { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.New;
    public List<OrderLine> Lines { get; private set; } = [];
    public decimal PaidAmountVnd { get; private set; }
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public decimal TotalAmountVnd => Lines.Sum(l => l.LineTotalVnd);
    public bool IsFullyPaid => PaidAmountVnd >= TotalAmountVnd;

    public void AddItem(Guid menuItemId, string menuItemName, int quantity, decimal unitPriceVnd)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");
        if (Status is OrderStatus.Closed or OrderStatus.Cancelled) throw new InvalidOperationException("Cannot modify a closed/cancelled order.");

        var existing = Lines.FirstOrDefault(l => l.MenuItemId == menuItemId);
        if (existing is null)
        {
            Lines.Add(new OrderLine(menuItemId, menuItemName, quantity, unitPriceVnd));
            return;
        }

        existing.Increase(quantity);
    }

    public void RemoveItem(Guid menuItemId, int quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");
        if (Status is OrderStatus.Closed or OrderStatus.Cancelled) throw new InvalidOperationException("Cannot modify a closed/cancelled order.");

        var line = Lines.FirstOrDefault(l => l.MenuItemId == menuItemId)
            ?? throw new InvalidOperationException("Menu item is not in this order.");

        line.Decrease(quantity);
        if (line.Quantity == 0)
        {
            Lines.Remove(line);
        }
    }

    public void SendToKitchen()
    {
        if (Lines.Count == 0) throw new InvalidOperationException("Cannot send empty order to kitchen.");
        if (Status != OrderStatus.New) throw new InvalidOperationException("Only new orders can be sent to kitchen.");
        Status = OrderStatus.Preparing;
    }

    public void MarkServed()
    {
        if (Status != OrderStatus.Preparing) throw new InvalidOperationException("Only preparing orders can be marked served.");
        Status = OrderStatus.Served;
    }

    public void ApplyPayment(decimal amountVnd)
    {
        if (amountVnd <= 0) throw new InvalidOperationException("Payment amount must be greater than zero.");
        if (Status is OrderStatus.Closed or OrderStatus.Cancelled) throw new InvalidOperationException("Cannot pay closed/cancelled order.");
        PaidAmountVnd += amountVnd;
    }

    public void Close()
    {
        if (!IsFullyPaid) throw new InvalidOperationException("Order cannot be closed before full payment.");
        Status = OrderStatus.Closed;
    }

    public void RecordCreatedEvent()
    {
        _domainEvents.Add(new OrderCreatedDomainEvent(
            Id,
            TableCode,
            CustomerName,
            CreatedAtUtc,
            TotalAmountVnd,
            Lines.Select(line => new OrderCreatedLineEvent(line.MenuItemId, line.MenuItemName, line.Quantity, line.UnitPriceVnd)).ToList()));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}

public sealed class OrderLine
{
    private OrderLine()
    {
    }

    public OrderLine(Guid menuItemId, string menuItemName, int quantity, decimal unitPriceVnd)
    {
        MenuItemId = menuItemId;
        MenuItemName = menuItemName;
        Quantity = quantity;
        UnitPriceVnd = unitPriceVnd;
    }

    public Guid MenuItemId { get; private set; }
    public string MenuItemName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPriceVnd { get; private set; }
    public decimal LineTotalVnd => Quantity * UnitPriceVnd;

    public void Increase(int quantity) => Quantity += quantity;

    public void Decrease(int quantity)
    {
        if (quantity > Quantity) throw new InvalidOperationException("Cannot remove more than current quantity.");
        Quantity -= quantity;
    }
}
