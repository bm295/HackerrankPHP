using TDining.Api.Domain.Entities;

namespace TDining.Api.Domain.Services;

public sealed class InventoryConsumptionService
{
    public List<(Guid IngredientId, decimal RequiredAmount)> CalculateConsumption(Order order, IEnumerable<MenuItem> menuItems)
    {
        var menuById = menuItems.ToDictionary(m => m.Id);
        var totals = new Dictionary<Guid, decimal>();

        foreach (var line in order.Lines)
        {
            if (!menuById.TryGetValue(line.MenuItemId, out var menu))
            {
                throw new InvalidOperationException($"Menu item '{line.MenuItemId}' was not found for inventory calculation.");
            }

            foreach (var ingredient in menu.Recipe)
            {
                var required = ingredient.Value * line.Quantity;
                totals[ingredient.Key] = totals.GetValueOrDefault(ingredient.Key) + required;
            }
        }

        return totals.Select(x => (x.Key, x.Value)).ToList();
    }
}
