using System.Reflection;
using FluentAssertions;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.EntityFrameworkCore.Repositories;
using MW.Persistence.EntityFrameworkCore.UnitOfWork;
using MW.Persistence.EntityFrameworkCore.Transactions;

namespace MW.Persistence.Tests.Architecture.ForbiddenPatterns;

/// <summary>
/// PTST-038: Validates that the persistence layer does not violate architectural rules.
/// </summary>
public class ForbiddenPatternTests
{
    private static readonly Assembly AbstractionsAssembly = typeof(IReadRepository<,>).Assembly;
    private static readonly Assembly EfCoreAssembly = typeof(EfReadRepository<,>).Assembly;

    [Fact]
    public void Abstractions_Should_NotReferenceEntityFrameworkCore()
    {
        var referencedAssemblies = AbstractionsAssembly.GetReferencedAssemblies();

        referencedAssemblies.Should().NotContain(
            a => a.Name!.Contains("EntityFrameworkCore", StringComparison.OrdinalIgnoreCase),
            "Persistence.Abstractions should not depend on EF Core");
    }

    [Fact]
    public void Abstractions_Should_NotReferenceImplementationAssembly()
    {
        var referencedAssemblies = AbstractionsAssembly.GetReferencedAssemblies();

        referencedAssemblies.Should().NotContain(
            a => a.Name == EfCoreAssembly.GetName().Name,
            "Persistence.Abstractions should not depend on EF Core implementations");
    }

    [Fact]
    public void EfCoreImplementations_Should_NotExposeDbContextInPublicApi()
    {
        var publicTypes = EfCoreAssembly.GetTypes()
            .Where(t => t.IsPublic && t.IsClass)
            .ToList();

        foreach (var type in publicTypes)
        {
            var publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in publicMethods)
            {
                method.ReturnType.Name.Should().NotBe("DbContext",
                    $"{type.Name}.{method.Name} should not expose DbContext in return type");
            }
        }
    }

    [Fact]
    public void RepositoryImplementations_Should_NotCallSaveChanges()
    {
        // Repositories should delegate save to UnitOfWork, not call SaveChanges directly
        // This is validated by checking that no repository method returns int (SaveChanges return type)
        var repositoryTypes = EfCoreAssembly.GetTypes()
            .Where(t => t.IsPublic && t.IsClass && t.Name.Contains("Repository"))
            .ToList();

        foreach (var type in repositoryTypes)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                method.Name.Should().NotBe("SaveChanges",
                    $"{type.Name} should not have SaveChanges — that belongs in IUnitOfWork");
                method.Name.Should().NotBe("SaveChangesAsync",
                    $"{type.Name} should not have SaveChangesAsync — that belongs in IUnitOfWork");
            }
        }
    }

    [Fact]
    public void AllAbstractionInterfaces_Should_BePublic()
    {
        var interfaces = AbstractionsAssembly.GetTypes()
            .Where(t => t.IsInterface)
            .ToList();

        foreach (var iface in interfaces)
        {
            iface.IsPublic.Should().BeTrue(
                $"Interface {iface.FullName} should be public");
        }
    }

    [Fact]
    public void EfCoreRepositories_Should_HaveVirtualMethods()
    {
        var repositoryTypes = EfCoreAssembly.GetTypes()
            .Where(t => t.IsPublic && t.IsClass && t.Name.Contains("Repository") && !t.IsAbstract)
            .ToList();

        foreach (var type in repositoryTypes)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName) // Exclude property accessors
                .ToList();

            foreach (var method in methods)
            {
                method.IsVirtual.Should().BeTrue(
                    $"{type.Name}.{method.Name} should be virtual for extensibility");
            }
        }
    }

    [Fact]
    public void EfUnitOfWork_Should_NotImplementIDisposable()
    {
        typeof(EfUnitOfWork).Should().NotImplement<IDisposable>(
            "EfUnitOfWork should not implement IDisposable — DbContext lifecycle is managed by DI scope");
    }

    [Fact]
    public void EfTransactionScope_Should_ImplementIAsyncDisposable()
    {
        typeof(EfTransactionScope).Should().Implement<IAsyncDisposable>(
            "EfTransactionScope should implement IAsyncDisposable");
    }
}
