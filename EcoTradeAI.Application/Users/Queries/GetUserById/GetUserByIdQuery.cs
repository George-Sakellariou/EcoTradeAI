namespace EcoTradeAI.Application.Users.Queries.GetUserById;

/// <summary>
/// Query to retrieve a user by their ID.
/// </summary>
/// <remarks>
/// QUERY vs COMMAND:
/// - Query: Read-only, returns data, doesn't change state
/// - Command: Write operation, changes state, returns success/failure
/// 
/// QUERIES ARE SIMPLE:
/// - Just an ID (or filter criteria)
/// - No complex validation needed
/// - Handler does the work
/// </remarks>
public class GetUserByIdQuery
{
    /// <summary>
    /// ID of the user to retrieve.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Whether to include related entities (profile, verification status).
    /// </summary>
    /// <remarks>
    /// Performance consideration:
    /// - includeDetails = false: Lighter query, faster
    /// - includeDetails = true: More data, one query (no N+1 problem)
    /// 
    /// Choose based on use case:
    /// - User list: false (just name, email)
    /// - User profile page: true (show everything)
    /// </remarks>
    public bool IncludeDetails { get; init; }
}