namespace EcoTradeAI.Domain.Common;

/// <summary>
/// Base class for all entities in the domain.
/// Provides common properties and behaviors that all entities share.
/// </summary>
/// <remarks>
/// Why abstract? 
/// - We never instantiate BaseEntity directly - only specific entities like User, Product
/// - Forces inheritors to provide meaningful constructors
/// 
/// Why Guid for Id?
/// - Globally unique (no collision risk across distributed systems)
/// - Can be generated client-side (doesn't require database round-trip)
/// - Works well with Azure cloud services
/// </remarks>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for this entity.
    /// Protected set: Only the entity itself (or inheritors) can change the ID.
    /// Public get: Anyone can read the ID.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// When this entity was first created.
    /// Useful for auditing and sorting by creation date.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// When this entity was last modified.
    /// Updated automatically when entity changes.
    /// </summary>
    public DateTime UpdatedAt { get; protected set; }

    /// <summary>
    /// Protected constructor prevents direct instantiation.
    /// Initializes common properties with sensible defaults.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid(); // Generate unique ID on creation
        CreatedAt = DateTime.UtcNow; // Use UTC to avoid timezone issues
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Constructor for EF Core to use when loading from database.
    /// Accepts ID to reconstruct entities with existing IDs.
    /// </summary>
    protected BaseEntity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Updates the UpdatedAt timestamp.
    /// Call this from child entity methods when state changes.
    /// </summary>
    protected void MarkAsModified()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Entities are equal if they have the same ID.
    /// Two User entities with ID=123 are the same user, even if other properties differ.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        // Two new entities (default Guid) are never equal
        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// Entities with the same ID have the same hash code.
    /// Required when overriding Equals.
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// Equality operators for convenient comparison.
    /// Allows: if (user1 == user2) instead of user1.Equals(user2)
    /// </summary>
    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
        return !(left == right);
    }
}