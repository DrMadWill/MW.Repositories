using Microsoft.EntityFrameworkCore;
using MW.Persistence.Abstractions.UnitOfWork;

namespace MW.Persistence.EntityFrameworkCore.UnitOfWork;

/// <summary>
/// EF Core implementation of the Unit of Work pattern.
/// Coordinates the commit of all pending changes tracked by the <see cref="DbContext"/> as a single atomic operation.
/// </summary>
public class EfUnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfUnitOfWork"/> class.
    /// </summary>
    /// <param name="dbContext">The EF Core database context.</param>
    public EfUnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
