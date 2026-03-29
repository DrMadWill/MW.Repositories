using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MW.Saga.Models;

namespace MW.Saga.MassTransit.Persistence;

/// <summary>
/// Reusable EF Core mapping helper for common saga state fields.
/// Avoids repetitive mapping code for fields such as CorrelationId, CurrentState,
/// lifecycle timestamps, and version/concurrency fields.
/// </summary>
/// <typeparam name="TSaga">The saga state type that inherits from <see cref="SagaStateBase"/>.</typeparam>
public abstract class SagaStateBaseMap<TSaga> : IEntityTypeConfiguration<TSaga>
    where TSaga : SagaStateBase
{
    /// <summary>
    /// Applies the common saga state field mappings and then calls
    /// <see cref="ConfigureAdditional"/> for workflow-specific fields.
    /// </summary>
    public void Configure(EntityTypeBuilder<TSaga> builder)
    {
        builder.HasKey(s => s.CorrelationId);

        builder.Property(s => s.CorrelationId)
            .ValueGeneratedNever();

        builder.Property(s => s.CurrentState)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        builder.Property(s => s.CompletedAt);

        builder.Property(s => s.FailedAt);

        builder.Property(s => s.Version)
            .IsConcurrencyToken();

        ConfigureAdditional(builder);
    }

    /// <summary>
    /// Override this method to configure additional workflow-specific properties.
    /// </summary>
    protected virtual void ConfigureAdditional(EntityTypeBuilder<TSaga> builder)
    {
    }
}
