using EcoTradeAI.Domain.Common;

namespace EcoTradeAI.Domain.Entities;

/// <summary>
/// Tracks whether a user has been verified and how.
/// Verified users get trust badges and higher transaction limits.
/// </summary>
public class VerificationStatus : BaseEntity
{
    /// <summary>
    /// Foreign key to the verified user.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Whether the user is currently verified.
    /// </summary>
    public bool IsVerified { get; private set; }

    /// <summary>
    /// When verification was completed (null if not verified).
    /// </summary>
    public DateTime? VerificationDate { get; private set; }

    /// <summary>
    /// Type of verification performed.
    /// </summary>
    public VerificationType VerificationType { get; private set; }

    /// <summary>
    /// Optional notes from verification process.
    /// </summary>
    public string? VerificationNotes { get; private set; }

    /// <summary>
    /// Navigation property back to user.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private VerificationStatus() : base()
    {
    }

    /// <summary>
    /// Creates an unverified status for a new user.
    /// </summary>
    public static VerificationStatus CreateUnverified(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        return new VerificationStatus
        {
            UserId = userId,
            IsVerified = false,
            VerificationType = VerificationType.None
        };
    }

    /// <summary>
    /// Marks the user as verified.
    /// </summary>
    public void MarkAsVerified(VerificationType verificationType, string? notes = null)
    {
        if (verificationType == VerificationType.None)
            throw new ArgumentException("Cannot verify with type 'None'", nameof(verificationType));

        IsVerified = true;
        VerificationDate = DateTime.UtcNow;
        VerificationType = verificationType;
        VerificationNotes = notes;
        MarkAsModified();
    }

    /// <summary>
    /// Revokes verification (e.g., if documents expire).
    /// </summary>
    public void RevokeVerification(string reason)
    {
        IsVerified = false;
        VerificationNotes = $"Revoked: {reason}";
        MarkAsModified();
    }
}

/// <summary>
/// Types of verification available on the platform.
/// </summary>
public enum VerificationType
{
    /// <summary>Not verified</summary>
    None = 0,

    /// <summary>Email address verified</summary>
    Email = 1,

    /// <summary>Phone number verified</summary>
    Phone = 2,

    /// <summary>Government ID verified</summary>
    GovernmentId = 3,

    /// <summary>Both email and phone verified</summary>
    EmailAndPhone = 4,

    /// <summary>Full verification (ID + email + phone)</summary>
    Full = 5
}