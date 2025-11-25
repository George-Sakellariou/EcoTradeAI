using AutoMapper;
using EcoTradeAI.Application.Common.Interfaces;
using EcoTradeAI.Application.Common.Models;
using EcoTradeAI.Application.Users.DTOs;
using EcoTradeAI.Domain.Entities;
using EcoTradeAI.Domain.Repositories;

namespace EcoTradeAI.Application.Users.Commands.RegisterUser;

/// <summary>
/// Handles the RegisterUserCommand use case.
/// Orchestrates user registration workflow.
/// </summary>
/// <remarks>
/// HANDLER RESPONSIBILITIES:
/// 1. Check business rules (email uniqueness)
/// 2. Hash password
/// 3. Create domain entities
/// 4. Persist to database
/// 5. Return result
/// 
/// DEPENDENCIES:
/// - IUserRepository: Database access
/// - IPasswordHasher: Password security
/// - IMapper: Entity to DTO conversion
/// 
/// This is DEPENDENCY INJECTION:
/// - Handler declares what it needs (constructor parameters)
/// - DI container provides implementations at runtime
/// - Handler doesn't know or care about concrete implementations
/// - Makes testing easy: inject mocks
/// </remarks>
public class RegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor injection of dependencies.
    /// </summary>
    /// <remarks>
    /// Why constructor injection?
    /// - Makes dependencies explicit
    /// - Dependencies are required (not null)
    /// - Easy to test (pass mocks in constructor)
    /// - Compile-time safety
    /// 
    /// Alternative patterns:
    /// ❌ Service Locator: GetService<IUserRepository>() - hidden dependencies
    /// ❌ Property Injection: public IUserRepository Repo { get; set; } - optional dependencies
    /// ✅ Constructor Injection: Forces you to provide dependencies
    /// </remarks>
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    /// <summary>
    /// Executes the registration command.
    /// </summary>
    /// <param name="command">Validated registration data</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>Result with UserDto on success, or errors on failure</returns>
    /// <remarks>
    /// EXECUTION FLOW:
    /// 1. Validate email uniqueness (business rule)
    /// 2. Hash password (security)
    /// 3. Create User entity (domain logic)
    /// 4. Create UserProfile and VerificationStatus
    /// 5. Save to database (transaction)
    /// 6. Map to DTO and return
    /// 
    /// ERROR HANDLING:
    /// - Business rule violations → Result.Failure (e.g., email exists)
    /// - Unexpected exceptions → Let bubble up (handled by global exception handler)
    /// 
    /// Why Result<T> pattern?
    /// - Explicit success/failure
    /// - No exceptions for expected failures
    /// - Forces caller to handle both cases
    /// - More performant than exceptions
    /// </remarks>
    public async Task<Result<UserDto>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        // STEP 1: Check if email already exists (business rule validation)
        var emailExists = await _userRepository.ExistsWithEmailAsync(
            command.Email,
            cancellationToken);

        if (emailExists)
        {
            return Result<UserDto>.Failure("A user with this email already exists");
        }

        // STEP 2: Hash the password (NEVER store plain text!)
        var passwordHash = _passwordHasher.HashPassword(command.Password);

        // STEP 3: Create User entity using factory method
        // Factory method encapsulates domain rules (email normalization, initial reputation, etc.)
        var user = User.Create(
            email: command.Email,
            passwordHash: passwordHash,
            firstName: command.FirstName,
            lastName: command.LastName,
            userType: command.UserType);

        // STEP 4: Create related entities
        // UserProfile: Extended user information (optional fields)
        var userProfile = UserProfile.Create(user.Id);

        // VerificationStatus: Tracks verification state
        // New users start unverified
        var verificationStatus = VerificationStatus.CreateUnverified(user.Id);

        // STEP 5: Associate related entities with user
        user.SetProfile(userProfile);
        user.SetVerificationStatus(verificationStatus);

        // STEP 6: Persist to database
        // EF Core will automatically insert related entities due to navigation properties
        await _userRepository.AddAsync(user, cancellationToken);

        // Note: In a real application, you'd call SaveChangesAsync here
        // We'll implement Unit of Work pattern later for transaction management
        // For now, assume repository handles persistence

        // STEP 7: Map entity to DTO for API response
        // Never return entities directly to API (security, serialization issues)
        var userDto = _mapper.Map<UserDto>(user);

        return Result<UserDto>.Success(userDto);
    }
}

/*
 * USAGE EXAMPLE (in a controller):
 * 
 * [HttpPost("register")]
 * public async Task<ActionResult<UserDto>> Register(RegisterUserCommand command)
 * {
 *     // Validation happens automatically (we'll set this up later)
 *     
 *     var result = await _handler.HandleAsync(command);
 *     
 *     if (result.IsSuccess)
 *         return Ok(result.Value);
 *     
 *     return BadRequest(result.Errors);
 * }
 * 
 * TESTING EXAMPLE:
 * 
 * public async Task RegisterUser_WithValidCommand_ReturnsSuccess()
 * {
 *     // Arrange
 *     var mockRepo = new Mock<IUserRepository>();
 *     mockRepo.Setup(x => x.ExistsWithEmailAsync(It.IsAny<string>(), default))
 *             .ReturnsAsync(false);
 *     
 *     var mockHasher = new Mock<IPasswordHasher>();
 *     mockHasher.Setup(x => x.HashPassword(It.IsAny<string>()))
 *               .Returns("hashed_password");
 *     
 *     var handler = new RegisterUserCommandHandler(mockRepo.Object, mockHasher.Object, mapper);
 *     
 *     var command = new RegisterUserCommand
 *     {
 *         Email = "test@test.com",
 *         Password = "SecurePass123!",
 *         ConfirmPassword = "SecurePass123!",
 *         FirstName = "John",
 *         LastName = "Doe",
 *         UserType = UserType.Both
 *     };
 *     
 *     // Act
 *     var result = await handler.HandleAsync(command);
 *     
 *     // Assert
 *     Assert.True(result.IsSuccess);
 *     Assert.NotNull(result.Value);
 *     Assert.Equal("test@test.com", result.Value.Email);
 * }
 */