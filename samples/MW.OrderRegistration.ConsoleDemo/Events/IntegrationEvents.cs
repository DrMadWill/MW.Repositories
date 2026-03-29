using MW.Messaging.Contracts;

namespace MW.OrderRegistration.ConsoleDemo.Events;

public class OrderRegistrationStarted : IntegrationEvent
{
    public override string EventName => "order.registration.started.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
    public string BuyerId { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}

public class InventoryReservationRequested : IntegrationEvent
{
    public override string EventName => "inventory.reservation.requested.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
}

public class InventoryReserved : IntegrationEvent
{
    public override string EventName => "inventory.reserved.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
    public Guid ReservationId { get; init; }
}

public class InventoryReservationFailed : IntegrationEvent
{
    public override string EventName => "inventory.reservation.failed.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public class PaymentRequested : IntegrationEvent
{
    public override string EventName => "payment.requested.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
}

public class PaymentSucceeded : IntegrationEvent
{
    public override string EventName => "payment.succeeded.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
    public Guid PaymentId { get; init; }
}

public class PaymentFailed : IntegrationEvent
{
    public override string EventName => "payment.failed.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public class OrderRegistrationCompleted : IntegrationEvent
{
    public override string EventName => "order.registration.completed.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
}

public class OrderRegistrationFailed : IntegrationEvent
{
    public override string EventName => "order.registration.failed.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public class OrderRegistrationTimedOut : IntegrationEvent
{
    public override string EventName => "order.registration.timed-out.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
}

/// <summary>
/// Schedule message used for payment timeout detection.
/// </summary>
public class PaymentTimeoutExpired : IntegrationEvent
{
    public override string EventName => "payment.timeout.expired.v1";
    public override string EventVersion => "v1";
    public override string SourceService => "order-registration-demo";

    public Guid OrderId { get; init; }
}
