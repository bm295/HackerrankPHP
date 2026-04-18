namespace TDining.Api.Domain.Entities;

public sealed class InventoryItem
{
    private InventoryItem()
    {
    }

    public InventoryItem(Guid id, string name, string unit, decimal quantity)
    {
        Id = id;
        Name = name;
        Unit = unit;
        Quantity = quantity;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }

    public void Deduct(decimal amount)
    {
        if (amount <= 0) throw new InvalidOperationException("Deduction amount must be greater than zero.");
        if (Quantity < amount) throw new InvalidOperationException($"Insufficient inventory for {Name}.");
        Quantity -= amount;
    }
}
