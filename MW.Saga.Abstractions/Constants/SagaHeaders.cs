namespace MW.Saga.Constants;

/// <summary>
/// Shared header names for optional saga-aware messaging metadata.
/// These constants standardize header keys that can be used across
/// saga-related message publishing and consumption.
/// <para>
/// Infrastructure packages such as <c>MW.Saga.MassTransit</c> can reuse
/// these header names for consistent saga metadata propagation.
/// </para>
/// </summary>
public static class SagaHeaders
{
    /// <summary>
    /// Header name for the saga instance correlation identifier.
    /// </summary>
    public const string SagaId = "X-Saga-Id";

    /// <summary>
    /// Header name for the saga name.
    /// </summary>
    public const string SagaName = "X-Saga-Name";

    /// <summary>
    /// Header name for the current saga state.
    /// </summary>
    public const string SagaState = "X-Saga-State";

    /// <summary>
    /// Header name for the high-level saga lifecycle status.
    /// </summary>
    public const string SagaStatus = "X-Saga-Status";
}
