using EcoTradeAI.Domain.Common;

namespace EcoTradeAI.Domain.Entities;

/// <summary>
/// Extended profile information for a user.
/// Separated from User entity to keep core user data lightweight.
/// </summary>
public class UserProfile : BaseEntity
{
    /// <summary>
    /// Foreign key to the User this profile belongs to.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// User's biography or description (optional).
    /// </summary>
    public string? Bio { get; private set; }

    /// <summary>
    /// URL to user's avatar image in Azure Blob Storage.
    /// </summary>
    public string? AvatarUrl { get; private set; }

    /// <summary>
    /// User's location (city, country - not full address for privacy).
    /// </summary>
    public string? Location { get; private set; }

    /// <summary>
    /// JSON array of preferred product category IDs.
    /// Stored as string, deserialized in application layer.
    /// Example: "[1, 5, 12]"
    /// </summary>
    public string? PreferredCategories { get; private set; }

    /// <summary>
    /// Navigation property back to the owning user.
    /// Required for EF Core relationship configuration.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private UserProfile() : base()
    {
    }

    /// <summary>
    /// Factory method to create a user profile.
    /// </summary>
    public static UserProfile Create(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        var profile = new UserProfile
        {
            UserId = userId
        };

        return profile;
    }

    /// <summary>
    /// Updates profile information.
    /// </summary>
    public void UpdateProfile(string? bio, string? location, string? avatarUrl)
    {
        Bio = bio?.Trim();
        Location = location?.Trim();
        AvatarUrl = avatarUrl?.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// Sets preferred categories (as JSON string).
    /// In real implementation, you'd use a value object or collection.
    /// </summary>
    public void SetPreferredCategories(string? categoriesJson)
    {
        PreferredCategories = categoriesJson;
        MarkAsModified();
    }
}