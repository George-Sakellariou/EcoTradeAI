using FluentValidation;

namespace EcoTradeAI.Application.Users.Commands.RegisterUser;

/// <summary>
/// Validates RegisterUserCommand input.
/// Ensures all business rules for registration are met.
/// </summary>
/// <remarks>
/// FLUENTVALIDATION CONCEPTS:
/// 
/// 1. RuleFor(x => x.Property): Defines a rule for a property
/// 2. Chaining: Multiple rules can be chained (.NotEmpty().EmailAddress())
/// 3. Custom messages: .WithMessage("Custom error message")
/// 4. Conditional rules: .When(x => condition)
/// 5. Custom validators: .Must(x => CustomLogic(x))
/// 
/// VALIDATION LAYERS:
/// - Format validation (here): Email format, length, required fields
/// - Business validation (handler): Email uniqueness, domain rules
/// 
/// Why split?
/// - Format is fast, synchronous, no dependencies
/// - Business requires database access, slower
/// - Fail fast: Don't query DB if format is invalid
/// </remarks>
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        // EMAIL VALIDATION
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required")
            .EmailAddress()
                .WithMessage("Invalid email format")
            .MaximumLength(255)
                .WithMessage("Email must not exceed 255 characters");
        // Note: Email uniqueness checked in handler (requires database access)

        // PASSWORD VALIDATION
        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password is required")
            .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters")
            .MaximumLength(100)
                .WithMessage("Password must not exceed 100 characters")
            .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]")
                .WithMessage("Password must contain at least one digit")
            .Matches(@"[\W_]")
                .WithMessage("Password must contain at least one special character");

        // PASSWORD CONFIRMATION VALIDATION
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
                .WithMessage("Password confirmation is required")
            .Equal(x => x.Password)
                .WithMessage("Passwords do not match");

        // FIRST NAME VALIDATION
        RuleFor(x => x.FirstName)
            .NotEmpty()
                .WithMessage("First name is required")
            .MaximumLength(50)
                .WithMessage("First name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

        // LAST NAME VALIDATION
        RuleFor(x => x.LastName)
            .NotEmpty()
                .WithMessage("Last name is required")
            .MaximumLength(50)
                .WithMessage("Last name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

        // USER TYPE VALIDATION
        RuleFor(x => x.UserType)
            .IsInEnum()
                .WithMessage("Invalid user type");
    }
}

/*
 * VALIDATION EXAMPLES:
 * 
 * ✅ VALID:
 * Email: "john.doe@ecotrade.ai"
 * Password: "SecurePass123!"
 * ConfirmPassword: "SecurePass123!"
 * FirstName: "John"
 * LastName: "O'Brien-Smith"
 * UserType: UserType.Both
 * 
 * ❌ INVALID EXAMPLES:
 * 
 * 1. Weak password:
 *    Password: "password"
 *    Errors: No uppercase, no digit, no special char
 * 
 * 2. Mismatched passwords:
 *    Password: "SecurePass123!"
 *    ConfirmPassword: "SecurePass123"
 *    Error: Passwords do not match
 * 
 * 3. Invalid name characters:
 *    FirstName: "John123"
 *    Error: First name can only contain letters...
 * 
 * 4. Invalid email:
 *    Email: "not-an-email"
 *    Error: Invalid email format
 */