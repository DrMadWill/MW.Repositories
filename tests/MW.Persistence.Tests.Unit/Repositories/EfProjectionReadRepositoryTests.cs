using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Unit.Repositories;

/// <summary>
/// PTST-018: Unit tests for EfProjectionReadRepository behavior.
/// Validates projection query operations.
/// </summary>
public class EfProjectionReadRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfProjectionReadRepository<TestEntity, Guid> _repository;

    public EfProjectionReadRepositoryTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _repository = new EfProjectionReadRepository<TestEntity, Guid>(_context);
    }

    [Fact]
    public async Task ProjectAsync_Should_ReturnProjectedResults()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Alpha").WithValue(10).Build());
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Beta").WithValue(20).Build());
        await _context.SaveChangesAsync();

        var result = await _repository.ProjectAsync(
            e => true,
            e => new { e.Name, e.Value });

        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Name == "Alpha" && r.Value == 10);
        result.Should().Contain(r => r.Name == "Beta" && r.Value == 20);
    }

    [Fact]
    public async Task ProjectAsync_Should_FilterWithPredicate()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Match").WithValue(100).Build());
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("NoMatch").WithValue(5).Build());
        await _context.SaveChangesAsync();

        var result = await _repository.ProjectAsync(
            e => e.Value >= 50,
            e => e.Name);

        result.Should().HaveCount(1);
        result[0].Should().Be("Match");
    }

    [Fact]
    public async Task ProjectByIdAsync_Should_ReturnProjectedEntity()
    {
        var entity = TestEntityBuilder.Default().WithName("Target").WithValue(42).Build();
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.ProjectByIdAsync(
            entity.Id,
            e => new { e.Name, e.Value });

        result.Should().NotBeNull();
        result!.Name.Should().Be("Target");
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task ProjectByIdAsync_Should_ReturnNull_WhenNotFound()
    {
        var result = await _repository.ProjectByIdAsync(
            Guid.NewGuid(),
            e => e.Name);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ProjectAsync_Should_ReturnEmpty_WhenNoMatch()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Only").Build());
        await _context.SaveChangesAsync();

        var result = await _repository.ProjectAsync(
            e => e.Name == "NonExistent",
            e => e.Name);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenDbContextIsNull()
    {
        Action act = () => new EfProjectionReadRepository<TestEntity, Guid>(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dbContext");
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
