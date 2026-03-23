using FluentAssertions;
using MW.Persistence.EntityFrameworkCore.UnitOfWork;
using MW.Persistence.Tests.Shared.Builders;
using MW.Persistence.Tests.Shared.Infrastructure;

namespace MW.Persistence.Tests.Unit.UnitOfWork;

/// <summary>
/// PTST-013: Unit tests for EfUnitOfWork behavior.
/// </summary>
public class EfUnitOfWorkTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly TestDbContext _context;
    private readonly EfUnitOfWork _unitOfWork;

    public EfUnitOfWorkTests()
    {
        _factory = new TestDbContextFactory();
        _context = _factory.CreateContext();
        _unitOfWork = new EfUnitOfWork(_context);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_PersistChanges()
    {
        var entity = TestEntityBuilder.Default().Build();
        _context.TestEntities.Add(entity);

        var result = await _unitOfWork.SaveChangesAsync();

        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_ReturnZero_WhenNoChanges()
    {
        var result = await _unitOfWork.SaveChangesAsync();

        result.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_ReturnAffectedCount()
    {
        _context.TestEntities.AddRange(
            TestEntityBuilder.Default().Build(),
            TestEntityBuilder.Default().Build(),
            TestEntityBuilder.Default().Build());

        var result = await _unitOfWork.SaveChangesAsync();

        result.Should().Be(3);
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenDbContextIsNull()
    {
        Action act = () => new EfUnitOfWork(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dbContext");
    }

    [Fact]
    public async Task SaveChangesAsync_Should_SupportCancellation()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        _context.TestEntities.Add(TestEntityBuilder.Default().Build());

        Func<Task> act = () => _unitOfWork.SaveChangesAsync(cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    public void Dispose()
    {
        _context.Dispose();
        _factory.Dispose();
    }
}
