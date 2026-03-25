namespace MW.Messaging.MassTransit.Options;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; set; } = "localhost";
    public ushort Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// RabbitMQ username. Defaults to "guest" for local development.
    /// Must be explicitly configured for non-development environments.
    /// </summary>
    public string Username { get; set; } = "guest";

    /// <summary>
    /// RabbitMQ password. Defaults to "guest" for local development.
    /// Must be explicitly configured for non-development environments.
    /// </summary>
    public string Password { get; set; } = "guest";

    public bool UseSsl { get; set; }
    public string? SslServerName { get; set; }
    public string? SslCertificatePath { get; set; }

    /// <summary>
    /// When true, bypasses SSL certificate validation.
    /// WARNING: Only use in development/testing environments.
    /// </summary>
    public bool SslTrustCertificate { get; set; }
}
