namespace TDining.Api.Infrastructure.Outbox;

public sealed class LoggingIntegrationEventPublisher(ILogger<LoggingIntegrationEventPublisher> logger) : IIntegrationEventPublisher
{
    public Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken)
    {
        logger.LogInformation("Published integration event {EventType}: {Payload}", eventType, payload);
        return Task.CompletedTask;
    }
}
