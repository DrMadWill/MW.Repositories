using System.Reflection;
using FluentAssertions;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.Specifications;
using MW.Persistence.Abstractions.Transactions;
using MW.Persistence.Abstractions.UnitOfWork;
using MW.Persistence.Abstractions.Queries;

namespace MW.Persistence.Tests.Architecture.ContractShape;

/// <summary>
/// PTST-006: Validates that all persistence abstraction interfaces have the expected shape.
/// </summary>
public class ContractShapeTests
{
    private static readonly Assembly AbstractionsAssembly = typeof(IReadRepository<,>).Assembly;

    [Fact]
    public void IReadRepository_Should_DeclareExpectedMethods()
    {
        var type = typeof(IReadRepository<,>);
        var methods = type.GetMethods();

        methods.Should().Contain(m => m.Name == "GetByIdAsync");
        methods.Should().Contain(m => m.Name == "GetAllAsync");
        methods.Should().Contain(m => m.Name == "FindAsync");
        methods.Should().Contain(m => m.Name == "ExistsAsync");
        methods.Should().Contain(m => m.Name == "CountAsync");
    }

    [Fact]
    public void IWriteRepository_Should_DeclareExpectedMethods()
    {
        var type = typeof(IWriteRepository<,>);
        var methods = type.GetMethods();

        methods.Should().Contain(m => m.Name == "AddAsync");
        methods.Should().Contain(m => m.Name == "AddRangeAsync");
        methods.Should().Contain(m => m.Name == "Update");
        methods.Should().Contain(m => m.Name == "Remove");
        methods.Should().Contain(m => m.Name == "RemoveRange");
    }

    [Fact]
    public void IRepository_Should_InheritBothReadAndWrite()
    {
        var type = typeof(IRepository<,>);

        type.GetInterfaces().Should().Contain(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadRepository<,>));
        type.GetInterfaces().Should().Contain(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IWriteRepository<,>));
    }

    [Fact]
    public void IAggregateRepository_Should_InheritFromIRepository()
    {
        var type = typeof(IAggregateRepository<,>);

        type.GetInterfaces().Should().Contain(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<,>));
    }

    [Fact]
    public void IProjectionReadRepository_Should_DeclareProjectionMethods()
    {
        var type = typeof(IProjectionReadRepository<,>);
        var methods = type.GetMethods();

        methods.Should().Contain(m => m.Name == "ProjectAsync");
        methods.Should().Contain(m => m.Name == "ProjectByIdAsync");
    }

    [Fact]
    public void ISpecification_Should_DeclareExpectedProperties()
    {
        var type = typeof(ISpecification<>);
        var properties = type.GetProperties();

        properties.Should().Contain(p => p.Name == "Criteria");
        properties.Should().Contain(p => p.Name == "OrderBy");
        properties.Should().Contain(p => p.Name == "OrderByDescending");
        properties.Should().Contain(p => p.Name == "Skip");
        properties.Should().Contain(p => p.Name == "Take");
    }

    [Fact]
    public void ISpecification_Should_DeclareIsSatisfiedByMethod()
    {
        var type = typeof(ISpecification<>);
        var methods = type.GetMethods();

        methods.Should().Contain(m => m.Name == "IsSatisfiedBy");
    }

    [Fact]
    public void IQuerySpecification_Should_InheritFromISpecification()
    {
        var type = typeof(IQuerySpecification<,>);

        type.GetInterfaces().Should().Contain(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISpecification<>));
    }

    [Fact]
    public void IQuerySpecification_Should_DeclareSelectorProperty()
    {
        var type = typeof(IQuerySpecification<,>);
        var properties = type.GetProperties();

        properties.Should().Contain(p => p.Name == "Selector");
    }

    [Fact]
    public void IUnitOfWork_Should_NotInheritFromIDisposable()
    {
        typeof(IUnitOfWork).Should().NotImplement<IDisposable>();
    }

    [Fact]
    public void IUnitOfWork_Should_DeclareSaveChangesAsync()
    {
        var methods = typeof(IUnitOfWork).GetMethods();

        methods.Should().Contain(m => m.Name == "SaveChangesAsync");
    }

    [Fact]
    public void ITransactionManager_Should_DeclareBeginTransactionAsync()
    {
        var methods = typeof(ITransactionManager).GetMethods();

        methods.Should().Contain(m => m.Name == "BeginTransactionAsync");
    }

    [Fact]
    public void ITransactionScope_Should_InheritFromIAsyncDisposable()
    {
        typeof(ITransactionScope).Should().Implement<IAsyncDisposable>();
    }

    [Fact]
    public void ITransactionScope_Should_DeclareCommitAndRollback()
    {
        var methods = typeof(ITransactionScope).GetMethods();

        methods.Should().Contain(m => m.Name == "CommitAsync");
        methods.Should().Contain(m => m.Name == "RollbackAsync");
    }

    [Fact]
    public void IQueryOptions_Should_DeclareExpectedProperties()
    {
        var properties = typeof(IQueryOptions).GetProperties();

        properties.Should().Contain(p => p.Name == "AsNoTracking");
        properties.Should().Contain(p => p.Name == "IgnoreQueryFilters");
        properties.Should().Contain(p => p.Name == "IncludeSoftDeleted");
    }

    [Fact]
    public void ISoftDeleteFilter_Should_DeclareExpectedProperties()
    {
        var properties = typeof(ISoftDeleteFilter).GetProperties();

        properties.Should().Contain(p => p.Name == "IncludeDeleted");
        properties.Should().Contain(p => p.Name == "OnlyDeleted");
    }

    [Fact]
    public void AbstractionsAssembly_Should_ContainExpectedInterfaceCount()
    {
        var interfaces = AbstractionsAssembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .ToList();

        // Should have at minimum the 12 core interfaces
        interfaces.Count.Should().BeGreaterThanOrEqualTo(12);
    }
}
