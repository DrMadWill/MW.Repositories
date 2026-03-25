namespace MW.Messaging.MassTransit.Options;

public class RedeliveryOptions
{
    public const string SectionName = "Messaging:Redelivery";

    public int RedeliveryCount { get; set; } = 3;
    public int[] RedeliveryIntervalsInSeconds { get; set; } = [5, 15, 30];
}
