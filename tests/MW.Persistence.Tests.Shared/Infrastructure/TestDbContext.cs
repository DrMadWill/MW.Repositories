using Microsoft.EntityFrameworkCore;
using MW.Persistence.Tests.Shared.Entities;

namespace MW.Persistence.Tests.Shared.Infrastructure;

/// <summary>
/// Test DbContext for persistence tests using SQLite in-memory provider.
/// </summary>
public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    public DbSet<SoftDeletableEntity> SoftDeletableEntities => Set<SoftDeletableEntity>();
    public DbSet<TestAggregate> TestAggregates => Set<TestAggregate>();
    public DbSet<ConcurrentEntity> ConcurrentEntities => Set<ConcurrentEntity>();

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<SoftDeletableEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<TestAggregate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<ConcurrentEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.ConcurrencyStamp).IsConcurrencyToken();
        });
    }
}
