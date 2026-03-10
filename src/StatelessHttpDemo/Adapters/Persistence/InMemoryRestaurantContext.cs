using StatelessHttpDemo.Domain.Entities;

namespace StatelessHttpDemo.Adapters.Persistence;

public sealed class InMemoryRestaurantContext
{
    public List<DiningTable> Tables { get; } = [];
    public List<MenuItem> MenuItems { get; } = [];
    public List<InventoryItem> InventoryItems { get; } = [];
    public List<Order> Orders { get; } = [];
    public List<Payment> Payments { get; } = [];
    public List<Reservation> Reservations { get; } = [];

    public static InMemoryRestaurantContext CreateSeeded()
    {
        var brothId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var noodleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var beefId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var coffeeId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var milkId = Guid.Parse("55555555-5555-5555-5555-555555555555");

        var context = new InMemoryRestaurantContext();

        context.Tables.AddRange([
            new DiningTable("T1", 2), new DiningTable("T2", 2), new DiningTable("T3", 4),
            new DiningTable("T4", 4), new DiningTable("T5", 6), new DiningTable("T6", 6),
            new DiningTable("P1", 10), new DiningTable("P2", 12), new DiningTable("P3", 24)
        ]);

        context.InventoryItems.AddRange([
            new InventoryItem(brothId, "Beef broth", "ml", 50000),
            new InventoryItem(noodleId, "Rice noodle", "g", 30000),
            new InventoryItem(beefId, "Beef slice", "g", 25000),
            new InventoryItem(coffeeId, "Coffee", "g", 8000),
            new InventoryItem(milkId, "Condensed milk", "ml", 5000)
        ]);

        context.MenuItems.AddRange([
            new MenuItem(Guid.Parse("d2f9b7dc-722f-4410-ad5d-24e4ca099301"), "Phở bò tái", "Main", 95000, true,
                new Dictionary<Guid, int> { [brothId] = 400, [noodleId] = 180, [beefId] = 120 }),
            new MenuItem(Guid.Parse("7398a160-7b24-44ea-9f36-5a15bf54ed4d"), "Gỏi cuốn tôm thịt", "Starter", 70000, true,
                new Dictionary<Guid, int>()),
            new MenuItem(Guid.Parse("ce98ded5-5be8-44e3-92e2-239f84c8887c"), "Cà phê sữa đá", "Beverage", 45000, true,
                new Dictionary<Guid, int> { [coffeeId] = 18, [milkId] = 60 })
        ]);

        return context;
    }
}
