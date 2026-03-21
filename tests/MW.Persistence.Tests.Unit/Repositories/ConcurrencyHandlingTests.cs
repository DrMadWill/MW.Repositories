using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Unit.Repositories;

/// <summary>
/// PTST-021: Unit tests for concurrency handling behavior.
/// </summary>
public class ConcurrencyHandlingTests : IDisposable
{
    private readonly TestDbContextFactory _factory;

    public ConcurrencyHandlingTests()
    {
        _factory = new TestDbContextFactory();
    }

    [Fact]
    public async Task ConcurrencyToken_Should_PreventStaleUpdates()
    {
        var entityId = Guid.NewGuid();

        // Seed
        using (var seedContext = _factory.CreateContext())
        {
            seedContext.ConcurrentEntities.Add(new ConcurrentEntity
            {
                Id = entityId,
                Name = "Original",
                ConcurrencyStamp = "version-1"
            });
            await seedContext.SaveChangesAsync();
        }

        // Load entity in two separate contexts
        using var context1 = _factory.CreateContext();
        using var context2 = _factory.CreateContext();

        var entity1 = await context1.ConcurrentEntities.FindAsync(entityId);
        var entity2 = await context2.ConcurrentEntities.FindAsync(entityId);

        // First writer updates
        entity1!.Name = "Updated by 1";
        entity1.ConcurrencyStamp = "version-2";
        await context1.SaveChangesAsync();

        // Second writer should get conflict
        entity2!.Name = "Updated by 2";
        entity2.ConcurrencyStamp = "version-3";

        Func<Task> act = () => context2.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    [Fact]
    public async Task ConcurrencyToken_Should_AllowUpdateWhenNotStale()
    {
        var entityId = Guid.NewGuid();

        using (var seedContext = _factory.CreateContext())
        {
            seedContext.ConcurrentEntities.Add(new ConcurrentEntity
            {
                Id = entityId,
                Name = "Original",
                ConcurrencyStamp = "version-1"
            });
            await seedContext.SaveChangesAsync();
        }

        using var context = _factory.CreateContext();
        var entity = await context.ConcurrentEntities.FindAsync(entityId);

        entity!.Name = "Updated";
        entity.ConcurrencyStamp = "version-2";

        Func<Task> act = () => context.SaveChangesAsync();

        await act.Should().NotThrowAsync();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}
