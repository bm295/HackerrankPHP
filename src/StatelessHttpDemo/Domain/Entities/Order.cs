namespace StatelessHttpDemo.Domain.Entities;

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
    private readonly List<OrderLine> _lines = [];

    public Order(Guid id, string tableCode, string customerName, DateTime createdAtUtc)
    {
        Id = id;
        TableCode = tableCode;
        CustomerName = customerName;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public string TableCode { get; }
    public string CustomerName { get; }
    public DateTime CreatedAtUtc { get; }
    public OrderStatus Status { get; private set; } = OrderStatus.New;
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
    public decimal PaidAmountVnd { get; private set; }

    public decimal TotalAmountVnd => _lines.Sum(l => l.LineTotalVnd);
    public bool IsFullyPaid => PaidAmountVnd >= TotalAmountVnd;

    public void AddItem(Guid menuItemId, string menuItemName, int quantity, decimal unitPriceVnd)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");
        if (Status is OrderStatus.Closed or OrderStatus.Cancelled) throw new InvalidOperationException("Cannot modify a closed/cancelled order.");

        var existing = _lines.FirstOrDefault(l => l.MenuItemId == menuItemId);
        if (existing is null)
        {
            _lines.Add(new OrderLine(menuItemId, menuItemName, quantity, unitPriceVnd));
            return;
        }

        existing.Increase(quantity);
    }

    public void RemoveItem(Guid menuItemId, int quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");
        if (Status is OrderStatus.Closed or OrderStatus.Cancelled) throw new InvalidOperationException("Cannot modify a closed/cancelled order.");

        var line = _lines.FirstOrDefault(l => l.MenuItemId == menuItemId)
            ?? throw new InvalidOperationException("Menu item is not in this order.");

        line.Decrease(quantity);
        if (line.Quantity == 0)
        {
            _lines.Remove(line);
        }
    }

    public void SendToKitchen()
    {
        if (_lines.Count == 0) throw new InvalidOperationException("Cannot send empty order to kitchen.");
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
}

public sealed class OrderLine(Guid menuItemId, string menuItemName, int quantity, decimal unitPriceVnd)
{
    public Guid MenuItemId { get; } = menuItemId;
    public string MenuItemName { get; } = menuItemName;
    public int Quantity { get; private set; } = quantity;
    public decimal UnitPriceVnd { get; } = unitPriceVnd;
    public decimal LineTotalVnd => Quantity * UnitPriceVnd;

    public void Increase(int quantity) => Quantity += quantity;

    public void Decrease(int quantity)
    {
        if (quantity > Quantity) throw new InvalidOperationException("Cannot remove more than current quantity.");
        Quantity -= quantity;
    }
}
