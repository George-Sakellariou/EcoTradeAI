namespace EcoTradeAI.Application.Users.DTOs;

/// <summary>
/// Data Transfer Object for User entity.
/// Used to send user data to API/UI without exposing domain entities.
/// </summary>
/// <remarks>
/// Why DTOs instead of returning entities directly?
/// 
/// 1. SECURITY: Don't expose PasswordHash to API clients
///    Entity: Has PasswordHash property
///    DTO: Doesn't include sensitive data
/// 
/// 2. DECOUPLING: API responses shouldn't change if entity changes
///    Entity: Adds new internal property
///    DTO: API response stays the same (no breaking change)
/// 
/// 3. PERFORMANCE: Return only what's needed
///    Entity: 20 properties + navigation properties
///    DTO: 5 properties for list view
/// 
/// 4. SERIALIZATION: DTOs are designed for JSON/XML
///    Entity: Might have circular references (User -> Profile -> User)
///    DTO: Flat structure, serialization-friendly
/// </remarks>
public class UserDto
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's full name (computed).
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Type of user (Buyer, Seller, Both).
    /// </summary>
    public string UserType { get; set; } = string.Empty;

    /// <summary>
    /// User's reputation score (0-100).
    /// </summary>
    public decimal ReputationScore { get; set; }

    /// <summary>
    /// Whether user is verified.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// When the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Detailed DTO with profile and verification info.
/// Used when full user details are needed.
/// </summary>
public class UserDetailDto : UserDto
{
    /// <summary>
    /// User's profile information.
    /// </summary>
    public UserProfileDto? Profile { get; set; }

    /// <summary>
    /// User's verification status.
    /// </summary>
    public VerificationStatusDto? VerificationStatus { get; set; }
}

/// <summary>
/// DTO for user profile information.
/// </summary>
public class UserProfileDto
{
    public Guid Id { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Location { get; set; }
}

/// <summary>
/// DTO for verification status.
/// </summary>
public class VerificationStatusDto
{
    public bool IsVerified { get; set; }
    public DateTime? VerificationDate { get; set; }
    public string VerificationType { get; set; } = string.Empty;
}