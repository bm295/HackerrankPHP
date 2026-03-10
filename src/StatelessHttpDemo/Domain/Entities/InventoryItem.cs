namespace StatelessHttpDemo.Domain.Entities;

public sealed class InventoryItem(Guid id, string name, string unit, decimal quantity)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Unit { get; } = unit;
    public decimal Quantity { get; private set; } = quantity;

    public void Deduct(decimal amount)
    {
        if (amount <= 0) throw new InvalidOperationException("Deduction amount must be greater than zero.");
        if (Quantity < amount) throw new InvalidOperationException($"Insufficient inventory for {Name}.");
        Quantity -= amount;
    }
}
