namespace MW.Hosting.AspNetCore.Options;

public class HealthEndpointOptions
{
    public string Path { get; set; } = "/api/health";
    public string? ReadinessPath { get; set; }
    public string? LivenessPath { get; set; }
    public bool UsePlainTextResponse { get; set; } = true;
    public string PlainText { get; set; } = "Ok";
}
