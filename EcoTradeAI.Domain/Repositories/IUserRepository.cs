using EcoTradeAI.Domain.Entities;

namespace EcoTradeAI.Domain.Repositories;

/// <summary>
/// Repository interface for User entity operations.
/// Inherits common CRUD from IRepository and adds user-specific queries.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    // Remove GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync, DeleteAsync, CountAsync
    // (they're inherited from IRepository<User>)
    
    // Keep only user-specific methods:
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByUserTypeAsync(UserType userType, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetVerifiedUsersWithMinReputationAsync(decimal minReputationScore, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
}