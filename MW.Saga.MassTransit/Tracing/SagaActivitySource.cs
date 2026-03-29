using System.Diagnostics;

namespace MW.Saga.MassTransit.Tracing;

/// <summary>
/// Shared <see cref="ActivitySource"/> for MW.Saga infrastructure.
/// Consumers of this package can subscribe to "MW.Saga" to receive
/// saga-related activities via standard .NET distributed tracing.
/// </summary>
internal static class SagaActivitySource
{
    public const string SourceName = "MW.Saga";

    public static readonly ActivitySource Source = new(SourceName, "1.0.0");
}
