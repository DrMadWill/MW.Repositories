using System.Reflection;
using FluentAssertions;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.Specifications;
using MW.Persistence.Abstractions.Transactions;
using MW.Persistence.Abstractions.UnitOfWork;
using MW.Persistence.Abstractions.Queries;

namespace MW.Persistence.Tests.Architecture.ContractShape;

/// <summary>
/// PTST-008: Validates that persistence contracts follow async-first design.
/// All query and mutation operations should be async with CancellationToken support.
/// </summary>
public class AsyncFirstContractTests
{
    [Theory]
    [InlineData(typeof(IReadRepository<,>), "GetByIdAsync")]
    [InlineData(typeof(IReadRepository<,>), "GetAllAsync")]
    [InlineData(typeof(IReadRepository<,>), "ExistsAsync")]
    [InlineData(typeof(IReadRepository<,>), "CountAsync")]
    [InlineData(typeof(IWriteRepository<,>), "AddAsync")]
    [InlineData(typeof(IWriteRepository<,>), "AddRangeAsync")]
    [InlineData(typeof(IProjectionReadRepository<,>), "ProjectAsync")]
    [InlineData(typeof(IProjectionReadRepository<,>), "ProjectByIdAsync")]
    [InlineData(typeof(IUnitOfWork), "SaveChangesAsync")]
    [InlineData(typeof(ITransactionManager), "BeginTransactionAsync")]
    [InlineData(typeof(ITransactionScope), "CommitAsync")]
    [InlineData(typeof(ITransactionScope), "RollbackAsync")]
    public void AsyncMethod_Should_ReturnTask(Type interfaceType, string methodName)
    {
        var methods = interfaceType.GetMethods()
            .Where(m => m.Name == methodName)
            .ToList();

        methods.Should().NotBeEmpty($"{interfaceType.Name} should declare {methodName}");

        foreach (var method in methods)
        {
            method.ReturnType.Should().Match(t =>
                t == typeof(Task) ||
                (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>)) ||
                t == typeof(ValueTask) ||
                (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ValueTask<>)),
                $"{interfaceType.Name}.{methodName} should return Task or Task<T>");
        }
    }

    [Theory]
    [InlineData(typeof(IReadRepository<,>), "GetByIdAsync")]
    [InlineData(typeof(IReadRepository<,>), "GetAllAsync")]
    [InlineData(typeof(IReadRepository<,>), "ExistsAsync")]
    [InlineData(typeof(IReadRepository<,>), "CountAsync")]
    [InlineData(typeof(IWriteRepository<,>), "AddAsync")]
    [InlineData(typeof(IWriteRepository<,>), "AddRangeAsync")]
    [InlineData(typeof(IProjectionReadRepository<,>), "ProjectAsync")]
    [InlineData(typeof(IProjectionReadRepository<,>), "ProjectByIdAsync")]
    [InlineData(typeof(IUnitOfWork), "SaveChangesAsync")]
    [InlineData(typeof(ITransactionManager), "BeginTransactionAsync")]
    [InlineData(typeof(ITransactionScope), "CommitAsync")]
    [InlineData(typeof(ITransactionScope), "RollbackAsync")]
    public void AsyncMethod_Should_AcceptCancellationToken(Type interfaceType, string methodName)
    {
        var methods = interfaceType.GetMethods()
            .Where(m => m.Name == methodName)
            .ToList();

        methods.Should().NotBeEmpty();

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            parameters.Should().Contain(p => p.ParameterType == typeof(CancellationToken),
                $"{interfaceType.Name}.{methodName} should accept CancellationToken");
        }
    }

    [Theory]
    [InlineData(typeof(IReadRepository<,>))]
    [InlineData(typeof(IWriteRepository<,>))]
    [InlineData(typeof(IProjectionReadRepository<,>))]
    [InlineData(typeof(IUnitOfWork))]
    [InlineData(typeof(ITransactionManager))]
    [InlineData(typeof(ITransactionScope))]
    public void AsyncMethods_Should_HaveAsyncSuffix(Type interfaceType)
    {
        var asyncMethods = interfaceType.GetMethods()
            .Where(m => m.ReturnType == typeof(Task) ||
                        (m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)) ||
                        m.ReturnType == typeof(ValueTask) ||
                        (m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
            .ToList();

        foreach (var method in asyncMethods)
        {
            method.Name.Should().EndWith("Async",
                $"{interfaceType.Name}.{method.Name} should follow Async naming convention");
        }
    }
}
