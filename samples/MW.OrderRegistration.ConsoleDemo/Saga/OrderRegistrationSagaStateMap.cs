using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MW.Saga.MassTransit.Persistence;

namespace MW.OrderRegistration.ConsoleDemo.Saga;

/// <summary>
/// EF Core mapping for the order registration saga state.
/// Extends the shared SagaStateBaseMap to add workflow-specific properties.
/// </summary>
public class OrderRegistrationSagaStateMap : SagaStateBaseMap<OrderRegistrationSagaState>
{
    protected override void ConfigureAdditional(EntityTypeBuilder<OrderRegistrationSagaState> builder)
    {
        builder.Property(s => s.OrderId);
        builder.Property(s => s.BuyerId).HasMaxLength(256);
        builder.Property(s => s.TotalAmount).HasPrecision(18, 2);
        builder.Property(s => s.InventoryReservationId);
        builder.Property(s => s.PaymentAttemptId);
        builder.Property(s => s.FailureReason).HasMaxLength(1024);
        builder.Property(s => s.Status);
        builder.Property(s => s.PaymentTimeoutTokenId);
    }
}
