using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Unit.Repositories;

/// <summary>
/// PTST-012: Unit tests for EfRepository (combined read + write) behavior.
/// </summary>
public class EfRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfRepository<TestEntity, Guid> _repository;

    public EfRepositoryTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _repository = new EfRepository<TestEntity, Guid>(_context);
    }

    [Fact]
    public async Task Should_SupportBothReadAndWriteOperations()
    {
        // Write
        var entity = TestEntityBuilder.Default().WithName("Combined").Build();
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Read
        var found = await _repository.GetByIdAsync(entity.Id);
        found.Should().NotBeNull();
        found!.Name.Should().Be("Combined");
    }

    [Fact]
    public async Task Should_SupportAddThenFind()
    {
        var entity = TestEntityBuilder.Default().WithName("FindMe").WithValue(42).Build();
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        var results = await _repository.FindAsync(e => e.Value == 42);
        results.Should().HaveCount(1);
        results[0].Name.Should().Be("FindMe");
    }

    [Fact]
    public async Task Should_SupportAddThenCount()
    {
        await _repository.AddRangeAsync(new[]
        {
            TestEntityBuilder.Default().Build(),
            TestEntityBuilder.Default().Build(),
            TestEntityBuilder.Default().Build()
        });
        await _context.SaveChangesAsync();

        var count = await _repository.CountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task Should_SupportUpdateAndRead()
    {
        var entity = TestEntityBuilder.Default().WithName("Before").Build();
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        entity.Name = "After";
        _repository.Update(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(entity.Id);
        result!.Name.Should().Be("After");
    }

    [Fact]
    public async Task Should_SupportRemoveAndVerify()
    {
        var entity = TestEntityBuilder.Default().Build();
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        _repository.Remove(entity);
        await _context.SaveChangesAsync();

        var exists = await _repository.ExistsAsync(e => e.Id == entity.Id);
        exists.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
