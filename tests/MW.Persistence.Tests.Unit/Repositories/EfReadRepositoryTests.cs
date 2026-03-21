using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Unit.Repositories;

/// <summary>
/// PTST-010: Unit tests for EfReadRepository behavior.
/// Uses SQLite in-memory to test read operations.
/// </summary>
public class EfReadRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfReadRepository<TestEntity, Guid> _repository;

    public EfReadRepositoryTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _repository = new EfReadRepository<TestEntity, Guid>(_context);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnEntity_WhenExists()
    {
        var entity = TestEntityBuilder.Default().Build();
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(entity.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_WhenNotExists()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnAllEntities()
    {
        var entities = Enumerable.Range(1, 3)
            .Select(i => TestEntityBuilder.Default().WithName($"Entity {i}").Build())
            .ToList();
        _context.TestEntities.AddRange(entities);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnEmptyList_WhenNoEntities()
    {
        var result = await _repository.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_WithPredicate_Should_ReturnMatchingEntities()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Alpha").WithValue(10).Build());
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Beta").WithValue(20).Build());
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Gamma").WithValue(30).Build());
        await _context.SaveChangesAsync();

        var result = await _repository.FindAsync(e => e.Value >= 20);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.Value >= 20);
    }

    [Fact]
    public async Task FindAsync_WithPredicate_Should_ReturnEmpty_WhenNoMatch()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithValue(5).Build());
        await _context.SaveChangesAsync();

        var result = await _repository.FindAsync(e => e.Value > 100);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnTrue_WhenMatches()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithName("Exists").Build());
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(e => e.Name == "Exists");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnFalse_WhenNoMatch()
    {
        var result = await _repository.ExistsAsync(e => e.Name == "NonExistent");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_Should_ReturnTotalCount()
    {
        _context.TestEntities.AddRange(
            TestEntityBuilder.Default().Build(),
            TestEntityBuilder.Default().Build());
        await _context.SaveChangesAsync();

        var result = await _repository.CountAsync();

        result.Should().Be(2);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_Should_ReturnFilteredCount()
    {
        _context.TestEntities.Add(TestEntityBuilder.Default().WithValue(10).Build());
        _context.TestEntities.Add(TestEntityBuilder.Default().WithValue(20).Build());
        _context.TestEntities.Add(TestEntityBuilder.Default().WithValue(30).Build());
        await _context.SaveChangesAsync();

        var result = await _repository.CountAsync(e => e.Value > 15);

        result.Should().Be(2);
    }

    [Fact]
    public async Task CountAsync_Should_ReturnZero_WhenEmpty()
    {
        var result = await _repository.CountAsync();

        result.Should().Be(0);
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenDbContextIsNull()
    {
        Action act = () => new EfReadRepository<TestEntity, Guid>(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dbContext");
    }

    [Fact]
    public async Task GetByIdAsync_Should_SupportCancellation()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Func<Task> act = () => _repository.GetByIdAsync(Guid.NewGuid(), cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
