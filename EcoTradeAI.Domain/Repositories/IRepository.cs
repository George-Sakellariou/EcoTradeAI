namespace EcoTradeAI.Domain.Repositories;

/// <summary>
/// Base repository interface with common CRUD operations.
/// Generic over entity type T.
/// </summary>
/// <typeparam name="T">Entity type this repository manages</typeparam>
/// <remarks>
/// Why generic?
/// - Reduces code duplication (every entity needs GetById, Add, Update, Delete)
/// - Type-safe: Can't accidentally pass Product to IUserRepository
/// - Enables generic repository implementations
/// 
/// Why not just use this for everything?
/// - Domain-specific queries (GetByEmail, GetVerifiedUsers) don't fit here
/// - Balance: Common operations in base, specific operations in derived interfaces
/// </remarks>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities of this type.
    /// Warning: Use with caution - consider pagination for large datasets.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts total entities of this type.
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}