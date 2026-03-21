using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Integration.Repositories;

/// <summary>
/// PTST-031: Integration tests for concurrency handling.
/// </summary>
public class ConcurrencyIntegrationTests : IDisposable
{
    private readonly TestDbContextFactory _factory;

    public ConcurrencyIntegrationTests()
    {
        _factory = new TestDbContextFactory();
    }

    [Fact]
    public async Task ConcurrentUpdate_Should_ThrowDbUpdateConcurrencyException()
    {
        // Arrange: seed entity
        var entityId = Guid.NewGuid();
        using (var seedContext = _factory.CreateContext())
        {
            seedContext.ConcurrentEntities.Add(new ConcurrentEntity
            {
                Id = entityId,
                Name = "Original",
                ConcurrencyStamp = "stamp-v1"
            });
            await seedContext.SaveChangesAsync();
        }

        // Act: simulate two concurrent contexts
        using var context1 = _factory.CreateContext();
        using var context2 = _factory.CreateContext();

        var entity1 = await context1.ConcurrentEntities.FindAsync(entityId);
        var entity2 = await context2.ConcurrentEntities.FindAsync(entityId);

        // First update succeeds
        entity1!.Name = "Updated by context1";
        entity1.ConcurrencyStamp = "stamp-v2";
        await context1.SaveChangesAsync();

        // Second update should fail due to concurrency conflict
        entity2!.Name = "Updated by context2";
        entity2.ConcurrencyStamp = "stamp-v3";

        Func<Task> act = () => context2.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}
