namespace MW.Hosting.AspNetCore.Abstractions;

public interface IRequestTraceAccessor
{
    string? TraceId { get; }
    string? CorrelationId { get; }
}
