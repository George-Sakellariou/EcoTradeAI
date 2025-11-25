using EcoTradeAI.Application.Common.Models;
using EcoTradeAI.Application.Users.Commands.RegisterUser;
using EcoTradeAI.Application.Users.Commands.UpdateUser;
using EcoTradeAI.Application.Users.DTOs;
using EcoTradeAI.Application.Users.Queries.GetAllUsers;
using EcoTradeAI.Application.Users.Queries.GetUserById;
using FluentValidation;

namespace EcoTradeAI.Application.Users.Services;

/// <summary>
/// Application service for user-related operations.
/// Acts as a facade to simplify controller code.
/// </summary>
/// <remarks>
/// WHY APPLICATION SERVICE?
/// 
/// WITHOUT SERVICE (Controller directly uses handlers):
/// [HttpPost("register")]
/// public async Task<ActionResult> Register(RegisterUserCommand command)
/// {
///     var validator = new RegisterUserCommandValidator();
///     var validationResult = await validator.ValidateAsync(command);
///     if (!validationResult.IsValid)
///         return BadRequest(validationResult.Errors);
///     
///     var result = await _registerHandler.HandleAsync(command);
///     if (result.IsSuccess)
///         return Ok(result.Value);
///     
///     return BadRequest(result.Errors);
/// }
/// 
/// WITH SERVICE (Controller uses service):
/// [HttpPost("register")]
/// public async Task<ActionResult> Register(RegisterUserCommand command)
/// {
///     var result = await _userService.RegisterUserAsync(command);
///     return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
/// }
/// 
/// BENEFITS:
/// ✅ Less duplication: Validation logic in one place
/// ✅ Simpler controllers: One line per action
/// ✅ Easy testing: Mock one service, not many handlers
/// 
/// TRADE-OFFS:
/// ❌ Extra layer: More files, slightly more complex
/// ❌ Less granular: Can't easily swap handlers
/// 
/// WHEN TO USE:
/// - Controllers are getting complex
/// - Multiple handlers often used together
/// - Validation logic duplicated
/// 
/// WHEN NOT TO USE:
/// - Simple CRUD operations
/// - Controllers are already simple
/// - Following strict CQRS (handlers only)
/// </remarks>
public class UserService
{
    private readonly RegisterUserCommandHandler _registerUserHandler;
    private readonly UpdateUserCommandHandler _updateUserHandler;
    private readonly GetUserByIdQueryHandler _getUserByIdHandler;
    private readonly GetAllUsersQueryHandler _getAllUsersHandler;

    // Validators injected for reuse
    private readonly IValidator<RegisterUserCommand> _registerUserValidator;
    private readonly IValidator<UpdateUserCommand> _updateUserValidator;

    public UserService(
        RegisterUserCommandHandler registerUserHandler,
        UpdateUserCommandHandler updateUserHandler,
        GetUserByIdQueryHandler getUserByIdHandler,
        GetAllUsersQueryHandler getAllUsersHandler,
        IValidator<RegisterUserCommand> registerUserValidator,
        IValidator<UpdateUserCommand> updateUserValidator)
    {
        _registerUserHandler = registerUserHandler;
        _updateUserHandler = updateUserHandler;
        _getUserByIdHandler = getUserByIdHandler;
        _getAllUsersHandler = getAllUsersHandler;
        _registerUserValidator = registerUserValidator;
        _updateUserValidator = updateUserValidator;
    }

    /// <summary>
    /// Registers a new user with validation.
    /// </summary>
    public async Task<Result<UserDto>> RegisterUserAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validate command
        var validationResult = await _registerUserValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<UserDto>.Failure(errors);
        }

        // Execute command
        return await _registerUserHandler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Updates an existing user with validation.
    /// </summary>
    public async Task<Result<UserDto>> UpdateUserAsync(
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validate command
        var validationResult = await _updateUserValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<UserDto>.Failure(errors);
        }

        // Execute command
        return await _updateUserHandler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by ID.
    /// </summary>
    public async Task<Result<UserDto>> GetUserByIdAsync(
        Guid userId,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery
        {
            UserId = userId,
            IncludeDetails = includeDetails
        };

        return await _getUserByIdHandler.HandleAsync(query, cancellationToken);
    }

    /// <summary>
    /// Retrieves all users with optional filtering.
    /// </summary>
    public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(
        GetAllUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _getAllUsersHandler.HandleAsync(query, cancellationToken);
    }
}

/*
 * USAGE IN CONTROLLER:
 * 
 * [ApiController]
 * [Route("api/[controller]")]
 * public class UsersController : ControllerBase
 * {
 *     private readonly UserService _userService;
 *     
 *     public UsersController(UserService userService)
 *     {
 *         _userService = userService;
 *     }
 *     
 *     [HttpPost("register")]
 *     public async Task<ActionResult<UserDto>> Register(RegisterUserCommand command)
 *     {
 *         var result = await _userService.RegisterUserAsync(command);
 *         
 *         if (result.IsSuccess)
 *             return Ok(result.Value);
 *         
 *         return BadRequest(result.Errors);
 *     }
 *     
 *     [HttpGet("{id}")]
 *     public async Task<ActionResult<UserDto>> GetUser(Guid id)
 *     {
 *         var result = await _userService.GetUserByIdAsync(id, includeDetails: true);
 *         
 *         if (result.IsSuccess)
 *             return Ok(result.Value);
 *         
 *         return NotFound(result.Errors);
 *     }
 * }
 */