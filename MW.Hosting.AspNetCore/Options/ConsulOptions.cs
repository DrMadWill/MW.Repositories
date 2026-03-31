namespace MW.Hosting.AspNetCore.Options;

public class ConsulOptions
{
    public bool Enabled { get; set; }
    public string ServiceId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ConsulAddress { get; set; } = string.Empty;
    public string ServiceAddress { get; set; } = string.Empty;
    public string HealthCheckPath { get; set; } = "/api/health";
    public string HealthCheckInterval { get; set; } = "10s";
    public string HealthCheckTimeout { get; set; } = "5s";
    public string DeregisterCriticalServiceAfter { get; set; } = "30s";
    public string[]? Tags { get; set; }
    public Dictionary<string, string>? Meta { get; set; }
}
