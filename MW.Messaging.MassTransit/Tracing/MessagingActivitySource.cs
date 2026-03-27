using System.Diagnostics;

namespace MW.Messaging.MassTransit.Tracing;

/// <summary>
/// Shared <see cref="ActivitySource"/> for MW.Messaging infrastructure.
/// Consumers of this package can subscribe to "MW.Messaging" to receive
/// publish and consume activities via standard .NET distributed tracing.
/// </summary>
internal static class MessagingActivitySource
{
    public const string SourceName = "MW.Messaging";

    public static readonly ActivitySource Source = new(SourceName, "1.0.0");
}
