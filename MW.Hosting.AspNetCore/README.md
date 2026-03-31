# MW.Hosting.AspNetCore

A reusable ASP.NET Core hosting package for MyBid microservices that standardizes host-level concerns.

## Purpose

This package provides standardized infrastructure for:
- **CORS** configuration
- **Health checks** with configurable endpoints
- **Consul** service discovery and registration
- **Serilog** logging with enrichers (user, trace, correlation ID)
- **Bootstrap** extension methods for minimal `Program.cs`

## Installation

Add a project reference:

```xml
<ProjectReference Include="../MW.Hosting.AspNetCore/MW.Hosting.AspNetCore.csproj" />
```

Or install from NuGet (when published):

```bash
dotnet add package MW.Hosting.AspNetCore
```

## Quick Start

### Minimal Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register all hosting defaults (CORS, health, Consul, logging services)
builder.Services.AddMyBidHostingDefaults(builder.Configuration);

// Configure Serilog
builder.ConfigureDefaultSerilog();

builder.Services.AddControllers();

var app = builder.Build();

// Apply all hosting defaults (CORS middleware, health endpoints)
app.UseMyBidHostingDefaults();

app.MapControllers();
app.Run();
```

### Individual Registration

```csharp
var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddDefaultCors(builder.Configuration);

// Health checks
builder.Services.AddDefaultHealthChecks();

// Consul
builder.Services.AddConsulClient(builder.Configuration);
builder.Services.AddConsulRegistration(builder.Configuration);

// Logging
builder.ConfigureDefaultSerilog();

builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultCors();
app.MapDefaultHealthEndpoints();
app.MapControllers();
app.Run();
```

## Configuration

### appsettings.json

```json
{
  "Cors": {
    "PolicyName": "DefaultPolicy",
    "AllowedOrigins": ["http://localhost:3000", "https://mybid.example.com"],
    "AllowAnyHeader": true,
    "AllowAnyMethod": true,
    "AllowCredentials": true,
    "ExposedHeaders": ["X-Pagination"]
  },
  "HealthEndpoints": {
    "Path": "/api/health",
    "ReadinessPath": "/api/health/ready",
    "LivenessPath": "/api/health/live",
    "UsePlainTextResponse": true,
    "PlainText": "Ok"
  },
  "Consul": {
    "Enabled": true,
    "ServiceId": "order-service-1",
    "ServiceName": "order-service",
    "ConsulAddress": "http://s_consul:8500",
    "ServiceAddress": "http://c_api_order:33000",
    "HealthCheckPath": "/api/health",
    "HealthCheckInterval": "10s",
    "HealthCheckTimeout": "5s",
    "DeregisterCriticalServiceAfter": "30s",
    "Tags": ["api", "order"],
    "Meta": { "version": "1.0.0" }
  },
  "Graylog": {
    "Enabled": true,
    "Host": "c_graylog",
    "Port": 12201,
    "Facility": "order-service",
    "TransportType": "Udp"
  }
}
```

### Docker Compose Environment Variables

```yaml
environment:
  - Consul__Enabled=true
  - Consul__ServiceId=order-service-1
  - Consul__ServiceName=order-service
  - Consul__ConsulAddress=http://s_consul:8500
  - Consul__ServiceAddress=http://c_api_order:33000
  - Cors__AllowedOrigins__0=https://mybid.example.com
  - Graylog__Enabled=true
  - Graylog__Host=c_graylog
  - Graylog__Port=12201
```

## Features

### CORS

- Configured via `CorsOptions` (policy name, origins, headers, methods, credentials)
- Validated on startup
- Applied via `UseDefaultCors()` middleware

### Health Checks

- Default endpoint at `/api/health`
- Optional readiness (`/api/health/ready`) and liveness (`/api/health/live`) endpoints
- Plain text `"Ok"` response or JSON response

### Consul

- Service registration on app start, deregistration on app stop
- HTTP health check registration
- Config-driven via `ConsulOptions`
- No-op when `Enabled = false`

### Logging

- Serilog with console and optional Graylog sinks
- User enrichment (UserName, UserId, or "Anonymous")
- Trace enrichment (TraceId, CorrelationId from X-Correlation-ID header)
- Health endpoint request filtering
- Machine name and environment enrichment

## Migration from Old Helpers

See [MIGRATION.md](MIGRATION.md) for detailed migration guide.

## API Reference

### Service Collection Extensions

| Method | Description |
|--------|-------------|
| `AddMyBidHostingDefaults(IConfiguration)` | Register all hosting defaults |
| `AddDefaultCors(IConfiguration, string)` | Register CORS from config |
| `AddDefaultCors(Action<CorsOptions>)` | Register CORS with delegate |
| `AddDefaultHealthChecks()` | Register health checks |
| `AddConsulClient(IConfiguration, string)` | Register Consul client |
| `AddConsulRegistration(IConfiguration, string)` | Register Consul lifecycle |

### Application Builder Extensions

| Method | Description |
|--------|-------------|
| `UseMyBidHostingDefaults()` | Apply all hosting middleware |
| `UseDefaultCors(string?)` | Apply CORS middleware |
| `MapDefaultHealthEndpoints()` | Map health check endpoints |
| `ConfigureDefaultSerilog(string)` | Configure Serilog defaults |
