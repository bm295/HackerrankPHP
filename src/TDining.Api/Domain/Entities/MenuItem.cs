namespace TDining.Api.Domain.Entities;

public sealed class MenuItem
{
    private MenuItem()
    {
    }

    public MenuItem(Guid id, string name, string category, decimal priceVnd, bool isAvailable, Dictionary<Guid, int> recipe)
    {
        Id = id;
        Name = name;
        Category = category;
        PriceVnd = priceVnd;
        IsAvailable = isAvailable;
        Recipe = recipe;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal PriceVnd { get; private set; }
    public bool IsAvailable { get; private set; }
    public Dictionary<Guid, int> Recipe { get; private set; } = [];

    public void SetAvailability(bool isAvailable) => IsAvailable = isAvailable;
}
