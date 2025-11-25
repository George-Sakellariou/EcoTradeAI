namespace EcoTradeAI.Application.Users.Commands.UpdateUser;

/// <summary>
/// Command to update an existing user's profile information.
/// </summary>
/// <remarks>
/// PARTIAL UPDATES:
/// - Allows updating name without changing email
/// - Email update requires separate security flow (not included here)
/// - Password change requires separate endpoint with old password verification
/// 
/// WHY NOT UPDATE EVERYTHING?
/// - Security: Some fields need extra verification
/// - UX: Users often update name/bio separately from sensitive data
/// - Audit: Sensitive changes (email, password) need special logging
/// </remarks>
public class UpdateUserCommand
{
    /// <summary>
    /// ID of the user to update.
    /// Typically from authenticated user's claims.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Updated first name (optional - only provided if user wants to change it).
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Updated last name (optional).
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Updated biography (optional).
    /// </summary>
    public string? Bio { get; init; }

    /// <summary>
    /// Updated location (optional).
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// Updated avatar URL (optional).
    /// Typically set after separate image upload flow.
    /// </summary>
    public string? AvatarUrl { get; init; }
}