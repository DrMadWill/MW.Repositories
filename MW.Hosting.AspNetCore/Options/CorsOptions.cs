namespace MW.Hosting.AspNetCore.Options;

public class CorsOptions
{
    public string PolicyName { get; set; } = "DefaultPolicy";
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public bool AllowAnyHeader { get; set; } = true;
    public bool AllowAnyMethod { get; set; } = true;
    public bool AllowCredentials { get; set; }
    public string[]? ExposedHeaders { get; set; }
}
