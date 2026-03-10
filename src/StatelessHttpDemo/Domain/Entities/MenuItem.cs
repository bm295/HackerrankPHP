namespace StatelessHttpDemo.Domain.Entities;

public sealed class MenuItem(Guid id, string name, string category, decimal priceVnd, bool isAvailable, Dictionary<Guid, int> recipe)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Category { get; } = category;
    public decimal PriceVnd { get; } = priceVnd;
    public bool IsAvailable { get; private set; } = isAvailable;
    public IReadOnlyDictionary<Guid, int> Recipe => recipe;

    public void SetAvailability(bool isAvailable) => IsAvailable = isAvailable;
}
