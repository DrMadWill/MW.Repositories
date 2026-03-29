using MW.OrderRegistration.ConsoleDemo.Domain.Enums;

namespace MW.OrderRegistration.ConsoleDemo.Configuration;

/// <summary>
/// Demo-specific configuration settings.
/// </summary>
public class DemoSettings
{
    public const string SectionName = "Demo";

    /// <summary>Active demo scenario name. Supported: success, inventory-fail, payment-fail, timeout.</summary>
    public string Scenario { get; set; } = "success";

    /// <summary>Payment timeout duration in seconds for the timeout scenario.</summary>
    public int PaymentTimeoutSeconds { get; set; } = 30;

    /// <summary>Delay (in seconds) after publishing the start event before querying results.</summary>
    public int ResultQueryDelaySeconds { get; set; } = 10;

    /// <summary>Resolved scenario enum value. Set at startup after resolving from config/CLI.</summary>
    public DemoScenario ResolvedScenario { get; set; } = DemoScenario.Success;
}

/// <summary>
/// Resolves the active DemoScenario from configuration/CLI args.
/// </summary>
public static class ScenarioResolver
{
    public static DemoScenario Resolve(string[] args, DemoSettings settings)
    {
        // CLI argument takes precedence: --scenario=success
        var cliScenario = args
            .FirstOrDefault(a => a.StartsWith("--scenario=", StringComparison.OrdinalIgnoreCase))
            ?.Split('=', 2).LastOrDefault();

        var scenarioName = cliScenario ?? settings.Scenario;

        var resolved = scenarioName.ToLowerInvariant() switch
        {
            "success" => DemoScenario.Success,
            "inventory-fail" => DemoScenario.InventoryFail,
            "payment-fail" => DemoScenario.PaymentFail,
            "timeout" => DemoScenario.Timeout,
            _ => throw new ArgumentException($"Unknown scenario: '{scenarioName}'. Supported: success, inventory-fail, payment-fail, timeout")
        };

        settings.ResolvedScenario = resolved;
        return resolved;
    }
}
