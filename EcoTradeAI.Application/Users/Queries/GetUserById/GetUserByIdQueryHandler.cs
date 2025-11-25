using AutoMapper;
using EcoTradeAI.Application.Common.Models;
using EcoTradeAI.Application.Users.DTOs;
using EcoTradeAI.Domain.Repositories;

namespace EcoTradeAI.Application.Users.Queries.GetUserById;

/// <summary>
/// Handles retrieval of a single user by ID.
/// </summary>
public class GetUserByIdQueryHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Executes the query to retrieve a user.
    /// </summary>
    /// <remarks>
    /// QUERY HANDLER PATTERN:
    /// - Simpler than command handlers (no state changes)
    /// - Just fetch and map
    /// - No validation needed (already done in query)
    /// 
    /// CONDITIONAL LOADING:
    /// - IncludeDetails = true → Load User + Profile + VerificationStatus
    /// - IncludeDetails = false → Load only User entity
    /// - Performance optimization: Don't load data you don't need
    /// </remarks>
    public async Task<Result<UserDto>> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        // Choose repository method based on whether details are needed
        var user = query.IncludeDetails
            ? await _userRepository.GetByIdWithDetailsAsync(query.UserId, cancellationToken)
            : await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        // Map to appropriate DTO based on details flag
        if (query.IncludeDetails)
        {
            var userDetailDto = _mapper.Map<UserDetailDto>(user);
            return Result<UserDto>.Success(userDetailDto);
        }
        else
        {
            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }
    }
}

/*
 * PERFORMANCE CONSIDERATION:
 * 
 * User list scenario (100 users):
 * - IncludeDetails = false: SELECT * FROM Users (1 query)
 * - IncludeDetails = true: SELECT * FROM Users LEFT JOIN UserProfiles... (1 query, more data)
 * 
 * Trade-off:
 * - false: Faster, less data, might need follow-up queries
 * - true: Slower, more data, no follow-up queries
 * 
 * Choose based on use case!
 */