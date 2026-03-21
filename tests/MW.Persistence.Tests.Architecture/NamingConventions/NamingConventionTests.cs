using System.Reflection;
using FluentAssertions;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.Specifications;
using MW.Persistence.Abstractions.Transactions;
using MW.Persistence.Abstractions.UnitOfWork;
using MW.Persistence.Abstractions.Queries;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.EntityFrameworkCore.Specifications;
using MW.Persistence.EntityFrameworkCore.Evaluators;
using MW.Persistence.EntityFrameworkCore.UnitOfWork;
using MW.Persistence.EntityFrameworkCore.Transactions;
using MW.Persistence.EntityFrameworkCore.Querying;

namespace MW.Persistence.Tests.Architecture.NamingConventions;

/// <summary>
/// PTST-007: Validates naming conventions across the persistence layer.
/// </summary>
public class NamingConventionTests
{
    [Theory]
    [InlineData(typeof(IReadRepository<,>))]
    [InlineData(typeof(IWriteRepository<,>))]
    [InlineData(typeof(IRepository<,>))]
    [InlineData(typeof(IAggregateRepository<,>))]
    [InlineData(typeof(IProjectionReadRepository<,>))]
    [InlineData(typeof(ISpecification<>))]
    [InlineData(typeof(IQuerySpecification<,>))]
    [InlineData(typeof(IUnitOfWork))]
    [InlineData(typeof(ITransactionManager))]
    [InlineData(typeof(ITransactionScope))]
    [InlineData(typeof(IQueryOptions))]
    [InlineData(typeof(ISoftDeleteFilter))]
    public void AbstractionInterfaces_Should_StartWithI(Type interfaceType)
    {
        interfaceType.Name.Should().StartWith("I",
            $"{interfaceType.Name} should follow I-prefix naming convention");
    }

    [Theory]
    [InlineData(typeof(EfReadRepository<,>), "Ef")]
    [InlineData(typeof(EfWriteRepository<,>), "Ef")]
    [InlineData(typeof(EfRepository<,>), "Ef")]
    [InlineData(typeof(EfAggregateRepository<,>), "Ef")]
    [InlineData(typeof(EfProjectionReadRepository<,>), "Ef")]
    [InlineData(typeof(EfUnitOfWork), "Ef")]
    [InlineData(typeof(EfTransactionManager), "Ef")]
    [InlineData(typeof(EfTransactionScope), "Ef")]
    public void EfCoreImplementations_Should_HaveEfPrefix(Type implementationType, string expectedPrefix)
    {
        implementationType.Name.Should().StartWith(expectedPrefix,
            $"{implementationType.Name} should follow Ef-prefix naming convention for EF Core implementations");
    }

    [Fact]
    public void AllPublicInterfaces_InAbstractions_Should_StartWithI()
    {
        var abstractionsAssembly = typeof(IReadRepository<,>).Assembly;
        var publicInterfaces = abstractionsAssembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .ToList();

        foreach (var iface in publicInterfaces)
        {
            iface.Name.Should().StartWith("I",
                $"Public interface {iface.FullName} should follow I-prefix naming convention");
        }
    }

    [Fact]
    public void AllPublicClasses_InEfCore_Should_NotBeInterfaces()
    {
        var efCoreAssembly = typeof(EfReadRepository<,>).Assembly;
        var publicClasses = efCoreAssembly.GetTypes()
            .Where(t => t.IsClass && t.IsPublic)
            .ToList();

        foreach (var cls in publicClasses)
        {
            cls.IsInterface.Should().BeFalse(
                $"{cls.FullName} is declared as an interface but should be a class in EF Core project");
        }
    }

    [Fact]
    public void RepositoryAbstractions_Should_UseConsistentSuffix()
    {
        var abstractionsAssembly = typeof(IReadRepository<,>).Assembly;
        var repositoryInterfaces = abstractionsAssembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic && t.Namespace?.Contains("Repositories") == true)
            .ToList();

        foreach (var iface in repositoryInterfaces)
        {
            iface.Name.Should().Contain("Repository",
                $"{iface.Name} in Repositories namespace should contain 'Repository'");
        }
    }

    [Fact]
    public void SpecificationInterfaces_Should_ContainSpecification()
    {
        var abstractionsAssembly = typeof(ISpecification<>).Assembly;
        var specInterfaces = abstractionsAssembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic && t.Namespace?.Contains("Specifications") == true)
            .ToList();

        foreach (var iface in specInterfaces)
        {
            iface.Name.Should().Contain("Specification",
                $"{iface.Name} in Specifications namespace should contain 'Specification'");
        }
    }
}
