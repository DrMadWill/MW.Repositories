using FluentAssertions;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Unit.Repositories;

/// <summary>
/// PTST-012 (extended): Unit tests for EfAggregateRepository behavior.
/// Validates that aggregate repository correctly constrains to IAggregateRoot types.
/// </summary>
public class EfAggregateRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfAggregateRepository<TestAggregate, Guid> _repository;

    public EfAggregateRepositoryTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _repository = new EfAggregateRepository<TestAggregate, Guid>(_context);
    }

    [Fact]
    public async Task Should_AddAndRetrieveAggregate()
    {
        var aggregate = TestAggregateBuilder.Default()
            .WithTitle("Test Aggregate")
            .WithDescription("Test Description")
            .Build();

        await _repository.AddAsync(aggregate);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(aggregate.Id);
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Aggregate");
    }

    [Fact]
    public async Task Should_InheritAllRepositoryOperations()
    {
        var aggregates = new[]
        {
            TestAggregateBuilder.Default().WithTitle("First").Build(),
            TestAggregateBuilder.Default().WithTitle("Second").Build()
        };

        await _repository.AddRangeAsync(aggregates);
        await _context.SaveChangesAsync();

        var all = await _repository.GetAllAsync();
        all.Should().HaveCount(2);

        var count = await _repository.CountAsync();
        count.Should().Be(2);

        var exists = await _repository.ExistsAsync(a => a.Title == "First");
        exists.Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
