using EcoTradeAI.Domain.Entities;

namespace EcoTradeAI.Application.Users.Queries.GetAllUsers;

/// <summary>
/// Query to retrieve all users with optional filtering.
/// </summary>
/// <remarks>
/// PAGINATION:
/// - Not implemented yet (coming in later sprint)
/// - Real apps MUST paginate (imagine 1M users!)
/// - For now: GetAllUsersQuery returns all users (fine for development)
/// 
/// Future: Add PageNumber, PageSize, TotalCount
/// </remarks>
public class GetAllUsersQuery
{
    /// <summary>
    /// Optional filter by user type.
    /// Null = return all user types.
    /// </summary>
    public UserType? UserType { get; init; }

    /// <summary>
    /// Optional filter by verification status.
    /// Null = return all users (verified and unverified).
    /// </summary>
    public bool? IsVerified { get; init; }

    /// <summary>
    /// Optional minimum reputation score filter.
    /// Null = no reputation filter.
    /// </summary>
    /// <remarks>
    /// Use case: Show only trusted sellers (reputation > 80)
    /// </remarks>
    public decimal? MinReputationScore { get; init; }
}