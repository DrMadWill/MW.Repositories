using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MW.Hosting.AspNetCore.HealthChecks;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Tests.Integration;

public class HealthEndpointTests
{
    [Fact]
    public async Task HealthEndpoint_ReturnsOk_WhenHealthy()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddDefaultHealthChecks();

        var app = builder.Build();
        app.MapDefaultHealthEndpoints();
        await app.StartAsync();

        var client = app.GetTestClient();
        var response = await client.GetAsync("/api/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Ok");

        await app.StopAsync();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk_WithCustomPath()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddDefaultHealthChecks();
        builder.Services.Configure<HealthEndpointOptions>(opts =>
        {
            opts.Path = "/health";
        });

        var app = builder.Build();
        app.MapDefaultHealthEndpoints();
        await app.StartAsync();

        var client = app.GetTestClient();
        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await app.StopAsync();
    }

    [Fact]
    public async Task HealthEndpoint_ReadinessEndpoint_WhenConfigured()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddDefaultHealthChecks();
        builder.Services.Configure<HealthEndpointOptions>(opts =>
        {
            opts.ReadinessPath = "/api/ready";
        });

        var app = builder.Build();
        app.MapDefaultHealthEndpoints();
        await app.StartAsync();

        var client = app.GetTestClient();
        var response = await client.GetAsync("/api/ready");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await app.StopAsync();
    }

    [Fact]
    public async Task HealthEndpoint_LivenessEndpoint_WhenConfigured()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddDefaultHealthChecks();
        builder.Services.Configure<HealthEndpointOptions>(opts =>
        {
            opts.LivenessPath = "/api/live";
        });

        var app = builder.Build();
        app.MapDefaultHealthEndpoints();
        await app.StartAsync();

        var client = app.GetTestClient();
        var response = await client.GetAsync("/api/live");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await app.StopAsync();
    }
}
