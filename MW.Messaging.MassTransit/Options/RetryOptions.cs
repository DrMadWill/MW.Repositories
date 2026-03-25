namespace MW.Messaging.MassTransit.Options;

public class RetryOptions
{
    public const string SectionName = "Messaging:Retry";

    public int RetryCount { get; set; } = 3;
    public int[] RetryIntervalsInSeconds { get; set; } = [1, 2, 4];
    public string[]? ExceptionTypeFilters { get; set; }
}
