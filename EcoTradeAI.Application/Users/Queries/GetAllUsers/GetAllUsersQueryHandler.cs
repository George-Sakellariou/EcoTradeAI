using AutoMapper;
using EcoTradeAI.Application.Common.Models;
using EcoTradeAI.Application.Users.DTOs;
using EcoTradeAI.Domain.Repositories;

namespace EcoTradeAI.Application.Users.Queries.GetAllUsers;

/// <summary>
/// Handles retrieval of all users with optional filtering.
/// </summary>
public class GetAllUsersQueryHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Executes the query to retrieve users.
    /// </summary>
    /// <remarks>
    /// FILTERING STRATEGY:
    /// - Start with all users
    /// - Apply filters in memory (for now)
    /// - Later: Push filters to database (WHERE clause)
    /// 
    /// IN-MEMORY vs DATABASE FILTERING:
    /// 
    /// In-memory (current):
    ///   var users = await _userRepository.GetAllAsync();
    ///   return users.Where(x => x.UserType == Buyer);
    ///   ❌ Loads ALL users into memory, then filters
    ///   ❌ Slow for large datasets
    ///   ✅ Simple to implement
    /// 
    /// Database (future):
    ///   var users = await _userRepository.GetByUserTypeAsync(Buyer);
    ///   ✅ Database filters (WHERE clause)
    ///   ✅ Fast, only loads needed data
    ///   ❌ Requires repository methods for each filter
    /// 
    /// For now: In-memory (simple, works for small datasets)
    /// Later: Implement repository methods for filters
    /// </remarks>
    public async Task<Result<IEnumerable<UserDto>>> HandleAsync(
        GetAllUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        // TEMPORARY: Load all users
        // TODO: Implement filtering in repository for better performance
        var users = await _userRepository.GetAllAsync(cancellationToken);

        // Apply filters if provided
        var filteredUsers = users.AsEnumerable();

        if (query.UserType.HasValue)
        {
            filteredUsers = filteredUsers.Where(x => x.UserType == query.UserType.Value);
        }

        if (query.IsVerified.HasValue)
        {
            filteredUsers = filteredUsers.Where(x => x.IsVerified == query.IsVerified.Value);
        }

        if (query.MinReputationScore.HasValue)
        {
            filteredUsers = filteredUsers.Where(x => x.ReputationScore >= query.MinReputationScore.Value);
        }

        // Map to DTOs
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(filteredUsers);

        return Result<IEnumerable<UserDto>>.Success(userDtos);
    }
}

/*
 * PERFORMANCE IMPROVEMENT (for later):
 * 
 * Instead of GetAllAsync() + in-memory filtering:
 * 
 * if (query.UserType.HasValue && query.MinReputationScore.HasValue)
 * {
 *     users = await _userRepository.GetByUserTypeAndMinReputationAsync(
 *         query.UserType.Value,
 *         query.MinReputationScore.Value);
 * }
 * 
 * This pushes filtering to database:
 * SELECT * FROM Users WHERE UserType = @UserType AND ReputationScore >= @MinScore
 * 
 * Much faster for large datasets!
 */