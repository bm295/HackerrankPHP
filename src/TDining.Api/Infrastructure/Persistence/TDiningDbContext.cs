using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TDining.Api.Domain.Entities;
using TDining.Api.Infrastructure.Outbox;

namespace TDining.Api.Infrastructure.Persistence;

public sealed class TDiningDbContext(DbContextOptions<TDiningDbContext> options) : DbContext(options)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public DbSet<DiningTable> Tables => Set<DiningTable>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var recipeConverter = new ValueConverter<Dictionary<Guid, int>, string>(
            recipe => JsonSerializer.Serialize(recipe, JsonOptions),
            json => string.IsNullOrWhiteSpace(json)
                ? new Dictionary<Guid, int>()
                : JsonSerializer.Deserialize<Dictionary<Guid, int>>(json, JsonOptions) ?? new Dictionary<Guid, int>());
        var recipeComparer = new ValueComparer<Dictionary<Guid, int>>(
            (left, right) => Serialize(left) == Serialize(right),
            value => Serialize(value).GetHashCode(),
            value => DeserializeDictionary(Serialize(value)));
        var orderLineConverter = new ValueConverter<List<OrderLine>, string>(
            lines => JsonSerializer.Serialize(lines, JsonOptions),
            json => string.IsNullOrWhiteSpace(json)
                ? new List<OrderLine>()
                : JsonSerializer.Deserialize<List<OrderLine>>(json, JsonOptions) ?? new List<OrderLine>());
        var orderLineComparer = new ValueComparer<List<OrderLine>>(
            (left, right) => Serialize(left) == Serialize(right),
            value => Serialize(value).GetHashCode(),
            value => DeserializeLines(Serialize(value)));

        modelBuilder.Entity<DiningTable>(entity =>
        {
            entity.ToTable("tables");
            entity.HasKey(table => table.Code);
            entity.Property(table => table.Code).HasMaxLength(20);
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.ToTable("menu_items");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(200);
            entity.Property(item => item.Category).HasMaxLength(100);
            entity.Property(item => item.Recipe)
                .HasColumnName("RecipeJson")
                .HasConversion(recipeConverter)
                .Metadata.SetValueComparer(recipeComparer);
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.ToTable("inventory_items");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(200);
            entity.Property(item => item.Unit).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(order => order.Id);
            entity.Property(order => order.TableCode).HasMaxLength(20);
            entity.Property(order => order.CustomerName).HasMaxLength(200);
            entity.Property(order => order.Lines)
                .HasColumnName("LinesJson")
                .HasConversion(orderLineConverter)
                .Metadata.SetValueComparer(orderLineComparer);
            entity.Ignore(order => order.TotalAmountVnd);
            entity.Ignore(order => order.IsFullyPaid);
            entity.Ignore(order => order.DomainEvents);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(payment => payment.Id);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("reservations");
            entity.HasKey(reservation => reservation.Id);
            entity.Property(reservation => reservation.CustomerName).HasMaxLength(200);
            entity.Property(reservation => reservation.PhoneNumber).HasMaxLength(50);
            entity.Property(reservation => reservation.Note).HasMaxLength(500);
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(message => message.Id);
            entity.Property(message => message.Type).HasMaxLength(250);
            entity.Property(message => message.Payload).HasColumnType("TEXT");
            entity.HasIndex(message => new { message.ProcessedOnUtc, message.OccurredOnUtc });
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = ChangeTracker.Entries<Order>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .Select(entry => entry.Entity)
            .ToList();

        if (entitiesWithEvents.Count > 0)
        {
            var outboxMessages = entitiesWithEvents
                .SelectMany(order => order.DomainEvents)
                .Select(@event => new OutboxMessage(
                    Guid.NewGuid(),
                    @event.GetType().Name,
                    JsonSerializer.Serialize(@event, @event.GetType(), JsonOptions),
                    @event.OccurredOnUtc))
                .ToList();

            await OutboxMessages.AddRangeAsync(outboxMessages, cancellationToken);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        return result;
    }

    private static string Serialize<TValue>(TValue value) => JsonSerializer.Serialize(value, JsonOptions);

    private static Dictionary<Guid, int> DeserializeDictionary(string json)
        => JsonSerializer.Deserialize<Dictionary<Guid, int>>(json, JsonOptions) ?? new Dictionary<Guid, int>();

    private static List<OrderLine> DeserializeLines(string json)
        => JsonSerializer.Deserialize<List<OrderLine>>(json, JsonOptions) ?? new List<OrderLine>();
}
