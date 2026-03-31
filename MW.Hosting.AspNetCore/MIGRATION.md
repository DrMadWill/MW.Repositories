# Migration Guide: Old Helpers → MW.Hosting.AspNetCore

## Overview

This guide maps old hosting helper methods to the new `MW.Hosting.AspNetCore` package API.

## Mapping Table

| Old API | New API | Notes |
|---------|---------|-------|
| `ConsulConfigModel` | `ConsulOptions` | Moved to `MW.Hosting.AspNetCore.Options` |
| `ConfigureConsul()` | `AddConsulClient()` + `AddConsulRegistration()` | Split into client setup and registration lifecycle |
| `RegisterWithConsul()` | Automatic via `ConsulRegistrationHostedService` | Uses `IHostedService` pattern |
| `AddCrosServices()` | `AddDefaultCors()` | Fixed typo: **Cros** → **Cors** |
| `UsingCrosServices()` | `UseDefaultCors()` | Fixed typo: **Cros** → **Cors** |
| Old health extension | `AddDefaultHealthChecks()` + `MapDefaultHealthEndpoints()` | Separated service registration from endpoint mapping |

## Breaking Changes

### 1. Configuration Keys Renamed

**Before:**
```json
{
  "ConsulConfig": {
    "ServiceID": "...",
    "ConsulAddress": "..."
  }
}
```

**After:**
```json
{
  "Consul": {
    "ServiceId": "...",
    "ConsulAddress": "..."
  }
}
```

### 2. Static State Removed

Old code used static fields for configuration. The new package uses the **Options Pattern** (`IOptions<T>`) and dependency injection exclusively. No static mutable state exists.

### 3. Naming: `Cros` → `Cors`

All method names now use correct spelling:
- `AddCrosServices()` → `AddDefaultCors()`
- `UsingCrosServices()` → `UseDefaultCors()`

### 4. New Options Sections

| Section | Options Class |
|---------|--------------|
| `Cors` | `CorsOptions` |
| `HealthEndpoints` | `HealthEndpointOptions` |
| `Consul` | `ConsulOptions` |
| `Graylog` | `GraylogOptions` |

### 5. Startup Usage Changes

**Before:**
```csharp
// Old Program.cs
builder.Services.AddCrosServices(builder.Configuration);
builder.Services.ConfigureConsul(builder.Configuration);

var app = builder.Build();
app.UsingCrosServices(builder.Configuration);
app.RegisterWithConsul(app.Lifetime);
```

**After:**
```csharp
// New Program.cs
builder.Services.AddMyBidHostingDefaults(builder.Configuration);

var app = builder.Build();
app.UseMyBidHostingDefaults();
```

Or with individual methods:
```csharp
builder.Services.AddDefaultCors(builder.Configuration);
builder.Services.AddDefaultHealthChecks();
builder.Services.AddConsulClient(builder.Configuration);
builder.Services.AddConsulRegistration(builder.Configuration);

var app = builder.Build();
app.UseDefaultCors();
app.MapDefaultHealthEndpoints();
```

## Consul Registration

The new Consul registration uses `IHostedService`:
- **Register** happens automatically on app start
- **Deregister** happens automatically on app stop
- No manual lifecycle management needed
- Uses async APIs (no `.Wait()` or `.Result()`)

## Options Validation

All options are validated on startup using `IValidateOptions<T>`. Invalid configuration fails early with descriptive error messages.
