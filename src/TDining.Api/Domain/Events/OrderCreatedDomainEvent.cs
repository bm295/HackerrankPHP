namespace TDining.Api.Domain.Events;

public sealed record OrderCreatedLineEvent(Guid MenuItemId, string MenuItemName, int Quantity, decimal UnitPriceVnd);

public sealed record OrderCreatedDomainEvent(
    Guid OrderId,
    string TableCode,
    string CustomerName,
    DateTime CreatedAtUtc,
    decimal TotalAmountVnd,
    IReadOnlyCollection<OrderCreatedLineEvent> Lines) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
