namespace MW.Hosting.AspNetCore.Options;

public class GraylogOptions
{
    public bool Enabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Facility { get; set; } = string.Empty;
    public string TransportType { get; set; } = "Udp";
}
