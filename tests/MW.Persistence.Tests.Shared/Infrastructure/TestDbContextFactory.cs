using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MW.Persistence.Tests.Shared.Infrastructure;

/// <summary>
/// Factory for creating TestDbContext instances backed by SQLite in-memory databases.
/// Each factory instance maintains its own connection for test isolation.
/// Implements IDisposable to properly clean up the SQLite connection.
/// </summary>
public class TestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    /// <summary>
    /// Creates a new TestDbContext using the shared in-memory SQLite connection.
    /// The database schema is created on the first call.
    /// </summary>
    public TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new TestDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
