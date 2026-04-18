namespace TDining.Api.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
