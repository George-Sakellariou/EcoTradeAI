using AutoMapper;
using EcoTradeAI.Application.Common.Models;
using EcoTradeAI.Application.Users.DTOs;
using EcoTradeAI.Domain.Repositories;

namespace EcoTradeAI.Application.Users.Commands.UpdateUser;

/// <summary>
/// Handles user profile update operations.
/// </summary>
public class UpdateUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Executes the update user command.
    /// </summary>
    /// <remarks>
    /// UPDATE STRATEGY:
    /// 1. Load entity from database (with related entities)
    /// 2. Update only provided fields
    /// 3. Save changes
    /// 
    /// WHY LOAD FULL ENTITY?
    /// - EF Core change tracking: Knows what changed
    /// - Domain validation: Entity validates updates
    /// - Relationships: UserProfile might be null initially
    /// 
    /// PARTIAL UPDATES:
    /// - Only update fields that were provided
    /// - Null fields = "don't change this"
    /// - Prevents accidentally clearing data
    /// </remarks>
    public async Task<Result<UserDto>> HandleAsync(
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        // STEP 1: Load user with related entities
        // GetByIdWithDetailsAsync loads User + UserProfile + VerificationStatus
        var user = await _userRepository.GetByIdWithDetailsAsync(
            command.UserId,
            cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        // STEP 2: Update name if provided
        if (!string.IsNullOrWhiteSpace(command.FirstName) ||
            !string.IsNullOrWhiteSpace(command.LastName))
        {
            // Use existing values if new ones not provided
            var firstName = command.FirstName ?? user.FirstName;
            var lastName = command.LastName ?? user.LastName;

            user.UpdateName(firstName, lastName);
        }

        // STEP 3: Update profile fields if provided
        if (command.Bio != null || command.Location != null || command.AvatarUrl != null)
        {
            // Ensure user has a profile
            if (user.UserProfile == null)
            {
                // Create profile if it doesn't exist
                var newProfile = Domain.Entities.UserProfile.Create(user.Id);
                user.SetProfile(newProfile);
            }

            // At this point, UserProfile is guaranteed to be non-null
            // Use null-forgiving operator (!) to tell compiler we've handled the null case
            var bio = command.Bio ?? user.UserProfile!.Bio;
            var location = command.Location ?? user.UserProfile!.Location;
            var avatarUrl = command.AvatarUrl ?? user.UserProfile!.AvatarUrl;

            user.UserProfile!.UpdateProfile(bio, location, avatarUrl);
        }

        // STEP 4: Persist changes
        // UpdateAsync tells EF Core to track changes and save them
        await _userRepository.UpdateAsync(user, cancellationToken);

        // STEP 5: Map updated entity to DTO
        var userDto = _mapper.Map<UserDto>(user);

        return Result<UserDto>.Success(userDto);
    }
}

/*
 * NULL COALESCING EXPLAINED:
 * 
 * var firstName = command.FirstName ?? user.FirstName;
 * 
 * If command.FirstName is null → use user.FirstName (existing value)
 * If command.FirstName is not null → use command.FirstName (new value)
 * 
 * This allows partial updates:
 * - User wants to update only last name → FirstName stays same
 * - User wants to update both → Both change
 * 
 * Example:
 * Existing: { FirstName: "John", LastName: "Doe" }
 * Command: { FirstName: null, LastName: "Smith" }
 * Result: { FirstName: "John", LastName: "Smith" }
 */