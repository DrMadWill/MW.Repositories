using Microsoft.EntityFrameworkCore;
using MW.Persistence.Abstractions.Transactions;

namespace MW.Persistence.EntityFrameworkCore.Transactions;

/// <summary>
/// EF Core implementation of the transaction manager abstraction.
/// Provides explicit transaction control using <see cref="DbContext.Database"/>.
/// </summary>
public class EfTransactionManager : ITransactionManager
{
    private readonly DbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfTransactionManager"/> class.
    /// </summary>
    /// <param name="dbContext">The EF Core database context.</param>
    public EfTransactionManager(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc />
    public virtual async Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new EfTransactionScope(transaction);
    }
}
