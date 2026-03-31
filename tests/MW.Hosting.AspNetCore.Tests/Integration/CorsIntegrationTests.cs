using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MW.Hosting.AspNetCore.Cors;
using MW.Hosting.AspNetCore.Options;

namespace MW.Hosting.AspNetCore.Tests.Integration;

public class CorsIntegrationTests
{
    [Fact]
    public async Task Cors_ReturnsAllowOriginHeader_ForConfiguredOrigin()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddDefaultCors(opts =>
        {
            opts.PolicyName = "TestPolicy";
            opts.AllowedOrigins = new[] { "https://example.com" };
            opts.AllowAnyHeader = true;
            opts.AllowAnyMethod = true;
        });
        builder.Services.AddRouting();

        var app = builder.Build();
        app.UseRouting();
        app.UseDefaultCors();
        app.MapGet("/test", () => Results.Ok("hello"));
        await app.StartAsync();

        var client = app.GetTestClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/test");
        request.Headers.Add("Origin", "https://example.com");
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Contains("Access-Control-Allow-Origin").Should().BeTrue();
        response.Headers.GetValues("Access-Control-Allow-Origin").Should().Contain("https://example.com");

        await app.StopAsync();
    }

    [Fact]
    public async Task Cors_DoesNotReturnAllowOriginHeader_ForUnknownOrigin()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddDefaultCors(opts =>
        {
            opts.PolicyName = "TestPolicy";
            opts.AllowedOrigins = new[] { "https://example.com" };
            opts.AllowAnyHeader = true;
            opts.AllowAnyMethod = true;
        });
        builder.Services.AddRouting();

        var app = builder.Build();
        app.UseRouting();
        app.UseDefaultCors();
        app.MapGet("/test", () => Results.Ok("hello"));
        await app.StartAsync();

        var client = app.GetTestClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/test");
        request.Headers.Add("Origin", "https://unknown.com");
        var response = await client.SendAsync(request);

        response.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();

        await app.StopAsync();
    }
}
