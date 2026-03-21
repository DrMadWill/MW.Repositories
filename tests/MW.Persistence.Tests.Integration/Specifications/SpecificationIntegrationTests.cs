using FluentAssertions;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Entities;
using MW.Persistence.Tests.Shared.Infrastructure;
using MW.Persistence.Tests.Shared.Specifications;

namespace MW.Persistence.Tests.Integration.Specifications;

/// <summary>
/// PTST-027: Integration tests for specification queries against a real database.
/// </summary>
public class SpecificationIntegrationTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfReadRepository<TestEntity, Guid> _repository;

    public SpecificationIntegrationTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _repository = new EfReadRepository<TestEntity, Guid>(_context);
        SeedData();
    }

    private void SeedData()
    {
        _context.TestEntities.AddRange(
            TestEntityBuilder.Default().WithName("Alpha").WithValue(10).Build(),
            TestEntityBuilder.Default().WithName("Beta").WithValue(20).Build(),
            TestEntityBuilder.Default().WithName("Charlie").WithValue(30).Build(),
            TestEntityBuilder.Default().WithName("Delta").WithValue(40).Build(),
            TestEntityBuilder.Default().WithName("Echo").WithValue(50).Build()
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task FindAsync_WithNameSpecification_Should_ReturnMatchingEntity()
    {
        var spec = new TestEntityByNameSpecification("Charlie");

        var result = await _repository.FindAsync(spec);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Charlie");
    }

    [Fact]
    public async Task FindAsync_WithPagedSpecification_Should_ApplyPaging()
    {
        var spec = new TestEntityPagedSpecification(skip: 1, take: 2);

        var result = await _repository.FindAsync(spec);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task FindAsync_WithOrderedDescSpecification_Should_OrderDescending()
    {
        var spec = new TestEntityOrderedDescSpecification();

        var result = await _repository.FindAsync(spec);

        result.Should().HaveCount(5);
        result.Should().BeInDescendingOrder(e => e.Value);
    }

    [Fact]
    public async Task FindAsync_WithFilteredPagedSpecification_Should_FilterAndPage()
    {
        var spec = new TestEntityFilteredPagedSpecification(minValue: 20, skip: 0, take: 2);

        var result = await _repository.FindAsync(spec);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.Value >= 20);
    }

    [Fact]
    public async Task FindAsync_WithAllSpecification_Should_ReturnAll()
    {
        var spec = new TestEntityAllSpecification();

        var result = await _repository.FindAsync(spec);

        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task FindAsync_WithNonMatchingSpec_Should_ReturnEmpty()
    {
        var spec = new TestEntityByNameSpecification("NonExistent");

        var result = await _repository.FindAsync(spec);

        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
