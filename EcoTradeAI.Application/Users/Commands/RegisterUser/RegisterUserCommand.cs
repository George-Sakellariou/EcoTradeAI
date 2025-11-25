using EcoTradeAI.Domain.Entities;

namespace EcoTradeAI.Application.Users.Commands.RegisterUser;

/// <summary>
/// Command to register a new user in the system.
/// Represents the user's intent to create an account.
/// </summary>
/// <remarks>
/// WHY A COMMAND?
/// - Separates input data from domain logic
/// - Makes intent explicit (not just "CreateUserDto")
/// - Easy to validate and test
/// - Can be logged/audited as a business action
/// 
/// IMMUTABILITY:
/// - Properties use 'init' (C# 9+): Can only be set during object initialization
/// - Why? Commands should not be modified after creation
/// - Benefits: Thread-safe, prevents accidental mutation, clearer intent
/// 
/// NO VALIDATION HERE:
/// - Commands are just data containers
/// - Validation is in RegisterUserCommandValidator
/// - Separation of concerns: Data structure vs validation rules
/// </remarks>
public class RegisterUserCommand
{
    /// <summary>
    /// Email address for the new user account.
    /// Must be unique in the system.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Plain text password provided by the user.
    /// Will be hashed before storage - NEVER store this directly!
    /// </summary>
    /// <remarks>
    /// Security Note:
    /// - This is plain text in memory temporarily during registration
    /// - Hashed immediately by IPasswordHasher
    /// - Never logged or stored
    /// - Sent over HTTPS only
    /// </remarks>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Password confirmation to prevent typos.
    /// Must match Password field.
    /// </summary>
    public string ConfirmPassword { get; init; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Type of account to create (Buyer, Seller, Both).
    /// </summary>
    public UserType UserType { get; init; }
}