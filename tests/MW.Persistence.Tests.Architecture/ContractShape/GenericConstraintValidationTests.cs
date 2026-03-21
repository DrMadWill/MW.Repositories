using System.Reflection;
using FluentAssertions;
using MW.Core.AggregateRoots;
using MW.Core.Entities;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.EntityFrameworkCore.Repositories;

namespace MW.Persistence.Tests.Architecture.ContractShape;

/// <summary>
/// PTST-009: Validates that generic constraints are properly defined on repository interfaces and implementations.
/// </summary>
public class GenericConstraintValidationTests
{
    [Fact]
    public void IReadRepository_Should_ConstrainTEntityToClassAndIEntity()
    {
        var type = typeof(IReadRepository<,>);
        var tEntity = type.GetGenericArguments()[0];
        var constraints = tEntity.GetGenericParameterConstraints();

        tEntity.GenericParameterAttributes.Should().HaveFlag(
            GenericParameterAttributes.ReferenceTypeConstraint,
            "TEntity should have class constraint");

        constraints.Should().Contain(c =>
            c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IEntity<>),
            "TEntity should implement IEntity<TId>");
    }

    [Fact]
    public void IWriteRepository_Should_ConstrainTEntityToClassAndIEntity()
    {
        var type = typeof(IWriteRepository<,>);
        var tEntity = type.GetGenericArguments()[0];
        var constraints = tEntity.GetGenericParameterConstraints();

        tEntity.GenericParameterAttributes.Should().HaveFlag(
            GenericParameterAttributes.ReferenceTypeConstraint);

        constraints.Should().Contain(c =>
            c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IEntity<>));
    }

    [Fact]
    public void IRepository_Should_ConstrainTEntityToClassAndIEntity()
    {
        var type = typeof(IRepository<,>);
        var tEntity = type.GetGenericArguments()[0];
        var constraints = tEntity.GetGenericParameterConstraints();

        tEntity.GenericParameterAttributes.Should().HaveFlag(
            GenericParameterAttributes.ReferenceTypeConstraint);

        constraints.Should().Contain(c =>
            c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IEntity<>));
    }

    [Fact]
    public void IAggregateRepository_Should_ConstrainToIAggregateRoot()
    {
        var type = typeof(IAggregateRepository<,>);
        var tAggregate = type.GetGenericArguments()[0];
        var constraints = tAggregate.GetGenericParameterConstraints();

        tAggregate.GenericParameterAttributes.Should().HaveFlag(
            GenericParameterAttributes.ReferenceTypeConstraint);

        constraints.Should().Contain(c =>
            c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IAggregateRoot<>),
            "TAggregate should implement IAggregateRoot<TId>");
    }

    [Fact]
    public void IProjectionReadRepository_Should_ConstrainTEntityToClassAndIEntity()
    {
        var type = typeof(IProjectionReadRepository<,>);
        var tEntity = type.GetGenericArguments()[0];
        var constraints = tEntity.GetGenericParameterConstraints();

        tEntity.GenericParameterAttributes.Should().HaveFlag(
            GenericParameterAttributes.ReferenceTypeConstraint);

        constraints.Should().Contain(c =>
            c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IEntity<>));
    }

    [Fact]
    public void EfAggregateRepository_Should_ConstrainToIAggregateRoot()
    {
        var type = typeof(EfAggregateRepository<,>);
        var tAggregate = type.GetGenericArguments()[0];
        var constraints = tAggregate.GetGenericParameterConstraints();

        constraints.Should().Contain(c =>
            c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IAggregateRoot<>));
    }

    [Fact]
    public void IReadRepository_TId_Should_BeContravariant()
    {
        var type = typeof(IReadRepository<,>);
        var tId = type.GetGenericArguments()[1];

        tId.GenericParameterAttributes.Should().HaveFlag(
            GenericParameterAttributes.Contravariant,
            "TId should be declared with 'in' keyword (contravariant)");
    }

    [Fact]
    public void IWriteRepository_TId_Should_BeContravariant()
    {
        var type = typeof(IWriteRepository<,>);
        var tId = type.GetGenericArguments()[1];

        tId.GenericParameterAttributes.Should().HaveFlag(
            GenericParameterAttributes.Contravariant);
    }
}
