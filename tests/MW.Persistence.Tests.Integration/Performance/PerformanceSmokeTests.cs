using FluentAssertions;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.EntityFrameworkCore.UnitOfWork;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;
using System.Diagnostics;

namespace MW.Persistence.Tests.Integration.Performance;

/// <summary>
/// PTST-039: Performance smoke tests.
/// These are not strict benchmarks — they validate that operations complete within reasonable bounds.
/// </summary>
public class PerformanceSmokeTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfRepository<TestEntity, Guid> _repository;
    private readonly EfUnitOfWork _unitOfWork;

    public PerformanceSmokeTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _repository = new EfRepository<TestEntity, Guid>(_context);
        _unitOfWork = new EfUnitOfWork(_context);
    }

    [Fact]
    public async Task BulkInsert_100Entities_Should_CompleteWithinReasonableTime()
    {
        var entities = Enumerable.Range(1, 100)
            .Select(i => TestEntityBuilder.Default().WithName($"Perf-{i}").WithValue(i).Build())
            .ToList();

        var sw = Stopwatch.StartNew();
        await _repository.AddRangeAsync(entities);
        await _unitOfWork.SaveChangesAsync();
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            "Inserting 100 entities should complete within 5 seconds");

        var count = await _repository.CountAsync();
        count.Should().Be(100);
    }

    [Fact]
    public async Task GetAllAsync_With100Entities_Should_CompleteWithinReasonableTime()
    {
        // Seed
        var entities = Enumerable.Range(1, 100)
            .Select(i => TestEntityBuilder.Default().WithName($"Read-{i}").Build())
            .ToList();
        _context.TestEntities.AddRange(entities);
        await _context.SaveChangesAsync();

        var sw = Stopwatch.StartNew();
        var result = await _repository.GetAllAsync();
        sw.Stop();

        result.Should().HaveCount(100);
        sw.ElapsedMilliseconds.Should().BeLessThan(2000,
            "Reading 100 entities should complete within 2 seconds");
    }

    [Fact]
    public async Task CountAsync_Should_BeEfficientOnLargeDataset()
    {
        var entities = Enumerable.Range(1, 100)
            .Select(i => TestEntityBuilder.Default().WithValue(i).Build())
            .ToList();
        _context.TestEntities.AddRange(entities);
        await _context.SaveChangesAsync();

        var sw = Stopwatch.StartNew();
        var count = await _repository.CountAsync(e => e.Value > 50);
        sw.Stop();

        count.Should().Be(50);
        sw.ElapsedMilliseconds.Should().BeLessThan(1000,
            "Count with predicate should complete within 1 second");
    }

    [Fact]
    public async Task ExistsAsync_Should_BeEfficientOnLargeDataset()
    {
        var entities = Enumerable.Range(1, 100)
            .Select(i => TestEntityBuilder.Default().WithName($"Entity-{i}").Build())
            .ToList();
        _context.TestEntities.AddRange(entities);
        await _context.SaveChangesAsync();

        var sw = Stopwatch.StartNew();
        var exists = await _repository.ExistsAsync(e => e.Name == "Entity-50");
        sw.Stop();

        exists.Should().BeTrue();
        sw.ElapsedMilliseconds.Should().BeLessThan(1000,
            "ExistsAsync should complete within 1 second");
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
