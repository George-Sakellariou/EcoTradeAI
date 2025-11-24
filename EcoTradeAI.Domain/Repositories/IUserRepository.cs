using EcoTradeAI.Domain.Entities;

namespace EcoTradeAI.Domain.Repositories;

/// <summary>
/// Repository interface for User entity operations.
/// Defines the contract for data access without specifying implementation.
/// </summary>
/// <remarks>
/// Why async?
/// - Database I/O is slow (network calls, disk reads)
/// - Async prevents blocking threads while waiting
/// - Essential for scalable web applications
/// 
/// Why nullable return types (User?)?
/// - GetById might not find the user (deleted, never existed)
/// - Returning null is more explicit than throwing exceptions for "not found"
/// - Caller can handle null case (return 404, show error, etc.)
/// </remarks>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">User's unique ID</param>
    /// <param name="cancellationToken">Cancellation token for request cancellation</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their email address.
    /// Used during login and registration (checking for duplicates).
    /// </summary>
    /// <param name="email">User's email address (case-insensitive)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user with their profile and verification status included.
    /// Uses eager loading to avoid N+1 queries.
    /// </summary>
    /// <param name="id">User's unique ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User with related entities loaded, or null if not found</returns>
    Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all users (with optional filtering in future implementations).
    /// Warning: For admin use only - not suitable for public API without pagination.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all users</returns>
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves users by their type (Buyer, Seller, Both).
    /// Useful for analytics and targeted features.
    /// </summary>
    /// <param name="userType">The type of users to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of users matching the type</returns>
    Task<IEnumerable<User>> GetByUserTypeAsync(UserType userType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves verified users with reputation above a threshold.
    /// Used for "trusted seller" features.
    /// </summary>
    /// <param name="minReputationScore">Minimum reputation score</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of verified users with high reputation</returns>
    Task<IEnumerable<User>> GetVerifiedUsersWithMinReputationAsync(
        decimal minReputationScore, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the repository.
    /// Note: Does not call SaveChanges - that's handled by Unit of Work pattern.
    /// </summary>
    /// <param name="user">User entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user.
    /// Note: In EF Core, this might just mark the entity as modified.
    /// </summary>
    /// <param name="user">User entity with updated values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user from the repository.
    /// In production, consider soft delete (IsDeleted flag) instead.
    /// </summary>
    /// <param name="user">User entity to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the given email already exists.
    /// Useful for registration validation before creating the user.
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if email exists, false otherwise</returns>
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts total users in the system.
    /// Useful for analytics dashboards.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total number of users</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}