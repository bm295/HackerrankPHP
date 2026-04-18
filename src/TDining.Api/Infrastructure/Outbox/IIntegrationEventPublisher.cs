namespace TDining.Api.Infrastructure.Outbox;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken);
}
