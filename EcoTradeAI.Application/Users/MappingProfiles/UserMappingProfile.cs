using AutoMapper;
using EcoTradeAI.Domain.Entities;
using EcoTradeAI.Application.Users.DTOs;

namespace EcoTradeAI.Application.Users.MappingProfiles;

/// <summary>
/// AutoMapper profile for User entity mappings.
/// Defines how to convert between domain entities and DTOs.
/// </summary>
/// <remarks>
/// AutoMapper conventions:
/// - Properties with same name are mapped automatically (User.Email → UserDto.Email)
/// - Custom mappings needed when names differ or logic is required
/// 
/// Why AutoMapper?
/// - Reduces boilerplate mapping code
/// - Centralized mapping configuration
/// - Easy to maintain when DTOs change
/// </remarks>
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // User entity → UserDto
        CreateMap<User, UserDto>()
            // UserType enum → string (e.g., "Buyer", "Seller", "Both")
            .ForMember(dest => dest.UserType, 
                opt => opt.MapFrom(src => src.UserType.ToString()))
            // FullName is computed property - AutoMapper handles it automatically
            .ForMember(dest => dest.FullName, 
                opt => opt.MapFrom(src => src.FullName))
            // IsVerified comes from navigation property
            .ForMember(dest => dest.IsVerified, 
                opt => opt.MapFrom(src => src.IsVerified));

        // User entity → UserDetailDto (includes related entities)
        CreateMap<User, UserDetailDto>()
            .IncludeBase<User, UserDto>() // Inherit base mapping
            .ForMember(dest => dest.Profile, 
                opt => opt.MapFrom(src => src.UserProfile))
            .ForMember(dest => dest.VerificationStatus, 
                opt => opt.MapFrom(src => src.VerificationStatus));

        // UserProfile entity → UserProfileDto
        CreateMap<UserProfile, UserProfileDto>();

        // VerificationStatus entity → VerificationStatusDto
        CreateMap<VerificationStatus, VerificationStatusDto>()
            .ForMember(dest => dest.VerificationType, 
                opt => opt.MapFrom(src => src.VerificationType.ToString()));
    }
}