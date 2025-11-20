using EcoTradeAI.Domain.Common;

namespace EcoTradeAI.Domain.Entities;

/// <summary>
/// Represents a user account in the EcoTrade AI platform.
/// Users can be buyers, sellers, or both.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// User's email address - used for login and communication.
    /// Must be unique across the platform.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Hashed password - never store plain text passwords!
    /// This will be set by the authentication service.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// Type of user: Buyer, Seller, or Both.
    /// Determines what features they can access.
    /// </summary>
    public UserType UserType { get; private set; }

    /// <summary>
    /// User's reputation score (0-100).
    /// Increases with positive transactions, decreases with disputes.
    /// </summary>
    public decimal ReputationScore { get; private set; }

    /// <summary>
    /// Navigation property to the user's extended profile.
    /// One-to-one relationship: Each User has exactly one UserProfile.
    /// Nullable because profile might not be created immediately.
    /// </summary>
    public UserProfile? UserProfile { get; private set; }

    /// <summary>
    /// Navigation property to verification status.
    /// Nullable because user might not be verified yet.
    /// </summary>
    public VerificationStatus? VerificationStatus { get; private set; }

    /// <summary>
    /// Private constructor for EF Core.
    /// EF Core needs a parameterless constructor to materialize entities from database.
    /// </summary>
    private User() : base()
    {
    }

    /// <summary>
    /// Public factory method for creating new users.
    /// Enforces business rules at creation time.
    /// </summary>
    /// <param name="email">Valid email address</param>
    /// <param name="passwordHash">Pre-hashed password</param>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <param name="userType">Type of user account</param>
    /// <returns>A new User entity</returns>
    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        UserType userType)
    {
        // Business rule validation
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        var user = new User
        {
            Email = email.ToLowerInvariant().Trim(), // Normalize email
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            UserType = userType,
            ReputationScore = 50m // New users start at neutral reputation
        };

        return user;
    }

    /// <summary>
    /// Updates the user's email address.
    /// Enforces business rules for email changes.
    /// </summary>
    public void UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("Email cannot be empty", nameof(newEmail));

        Email = newEmail.ToLowerInvariant().Trim();
        MarkAsModified(); // Update timestamp
    }

    /// <summary>
    /// Updates the user's name.
    /// </summary>
    public void UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// Updates the password hash.
    /// This should only be called by the authentication service.
    /// </summary>
    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        MarkAsModified();
    }

    /// <summary>
    /// Adjusts reputation score based on transaction outcome.
    /// Score is clamped between 0 and 100.
    /// </summary>
    /// <param name="points">Positive for good behavior, negative for bad</param>
    public void AdjustReputation(decimal points)
    {
        ReputationScore += points;

        // Clamp between 0 and 100
        if (ReputationScore < 0) ReputationScore = 0;
        if (ReputationScore > 100) ReputationScore = 100;

        MarkAsModified();
    }

    /// <summary>
    /// Associates a profile with this user.
    /// Called when user creates their profile.
    /// </summary>
    public void SetProfile(UserProfile profile)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        UserProfile = profile;
        MarkAsModified();
    }

    /// <summary>
    /// Associates verification status with this user.
    /// Called when user completes verification process.
    /// </summary>
    public void SetVerificationStatus(VerificationStatus verificationStatus)
    {
        if (verificationStatus == null)
            throw new ArgumentNullException(nameof(verificationStatus));

        VerificationStatus = verificationStatus;
        MarkAsModified();
    }

    /// <summary>
    /// Computed property for user's full name.
    /// Not stored in database - calculated on the fly.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Computed property to check if user is verified.
    /// Convenience method to avoid null checks everywhere.
    /// </summary>
    public bool IsVerified => VerificationStatus?.IsVerified ?? false;
}

/// <summary>
/// Enum representing the type of user account.
/// </summary>
public enum UserType
{
    /// <summary>User can only purchase items</summary>
    Buyer = 1,

    /// <summary>User can only sell items</summary>
    Seller = 2,

    /// <summary>User can both buy and sell</summary>
    Both = 3
}