using MW.Messaging.Contracts;

namespace MW.OrderRegistration.ApiDemo.TestInfrastructure;

/// <summary>
/// Simple integration event used by debug/test endpoints to validate
/// messaging publish/consume flow without business logic.
/// </summary>
public class TestIntegrationEvent : IntegrationEvent
{
    public override string EventName => "test.debug.event.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-api-demo";

    public string Payload { get; init; } = string.Empty;
}
