namespace EcoTradeAI.Application.Common.Interfaces;

/// <summary>
/// Service for hashing and verifying passwords.
/// Implementation will be in Infrastructure layer.
/// </summary>
/// <remarks>
/// Why interface in Application?
/// - Application layer needs to hash passwords during registration
/// - Application shouldn't depend on Infrastructure (Clean Architecture rule)
/// - Infrastructure will provide concrete implementation (bcrypt, Argon2, etc.)
/// 
/// This is DEPENDENCY INVERSION:
/// - High-level module (Application) defines what it needs (interface)
/// - Low-level module (Infrastructure) provides implementation
/// - Both depend on abstraction (interface)
/// </remarks>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain text password.
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password (safe to store in database)</returns>
    /// <remarks>
    /// Never store plain text passwords!
    /// Even encrypted passwords are risky (if encryption key is compromised).
    /// Hashing is one-way: hash("password123") → "abc123xyz..." (can't reverse)
    /// </remarks>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plain text password against a hash.
    /// </summary>
    /// <param name="hashedPassword">Stored password hash from database</param>
    /// <param name="providedPassword">Password user entered during login</param>
    /// <returns>True if password matches, false otherwise</returns>
    /// <remarks>
    /// During login:
    /// 1. User enters password: "password123"
    /// 2. Fetch user's hash from database: "abc123xyz..."
    /// 3. VerifyPassword("abc123xyz...", "password123") → true/false
    /// </remarks>
    bool VerifyPassword(string hashedPassword, string providedPassword);
}