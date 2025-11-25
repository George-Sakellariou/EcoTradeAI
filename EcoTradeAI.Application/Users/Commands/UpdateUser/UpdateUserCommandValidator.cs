using FluentValidation;

namespace EcoTradeAI.Application.Users.Commands.UpdateUser;

/// <summary>
/// Validates UpdateUserCommand.
/// Only validates provided fields (optional updates).
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        // USER ID VALIDATION
        RuleFor(x => x.UserId)
            .NotEmpty()
                .WithMessage("User ID is required");

        // FIRST NAME VALIDATION (only if provided)
        // When(): Only apply these rules if FirstName is not null/empty
        When(x => !string.IsNullOrWhiteSpace(x.FirstName), () =>
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50)
                    .WithMessage("First name must not exceed 50 characters")
                .Matches(@"^[a-zA-Z\s'-]+$")
                    .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");
        });

        // LAST NAME VALIDATION (only if provided)
        When(x => !string.IsNullOrWhiteSpace(x.LastName), () =>
        {
            RuleFor(x => x.LastName)
                .MaximumLength(50)
                    .WithMessage("Last name must not exceed 50 characters")
                .Matches(@"^[a-zA-Z\s'-]+$")
                    .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");
        });

        // BIO VALIDATION (only if provided)
        When(x => !string.IsNullOrWhiteSpace(x.Bio), () =>
        {
            RuleFor(x => x.Bio)
                .MaximumLength(500)
                    .WithMessage("Bio must not exceed 500 characters");
        });

        // LOCATION VALIDATION (only if provided)
        When(x => !string.IsNullOrWhiteSpace(x.Location), () =>
        {
            RuleFor(x => x.Location)
                .MaximumLength(100)
                    .WithMessage("Location must not exceed 100 characters");
        });

        // AVATAR URL VALIDATION (only if provided)
        When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl), () =>
        {
            RuleFor(x => x.AvatarUrl)
                .MaximumLength(500)
                    .WithMessage("Avatar URL must not exceed 500 characters")
                .Must(BeAValidUrl)
                    .WithMessage("Avatar URL must be a valid URL");
        });
    }

    /// <summary>
    /// Custom validator to check if a string is a valid URL.
    /// </summary>
    /// <remarks>
    /// CUSTOM VALIDATION:
    /// - When built-in validators aren't enough
    /// - Encapsulates complex logic
    /// - Reusable across validators
    /// - Testable in isolation
    /// </remarks>
    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true; // Null/empty is valid (field is optional)

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}

/*
 * CONDITIONAL VALIDATION EXPLAINED:
 * 
 * When(condition, rules):
 * - Only applies rules if condition is true
 * - Useful for optional fields
 * - Prevents errors like "First name required" when user didn't provide it
 * 
 * Example:
 * Command: { UserId: Guid, FirstName: "John", LastName: null, Bio: null }
 * Result: Only validates UserId and FirstName (others are null, so skipped)
 * 
 * vs.
 * 
 * Command: { UserId: Guid, FirstName: "John123", LastName: null, Bio: null }
 * Result: Validates UserId, fails on FirstName (invalid characters)
 */