namespace MW.Messaging.MassTransit.Options;

public class MassTransitOptions
{
    public const string SectionName = "Messaging";

    public RabbitMqOptions RabbitMq { get; set; } = new();
    public RetryOptions Retry { get; set; } = new();
    public RedeliveryOptions Redelivery { get; set; } = new();
    public string ServiceName { get; set; } = string.Empty;
    public bool EnableHealthChecks { get; set; } = true;
}
