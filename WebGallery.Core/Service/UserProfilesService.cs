using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IUserProfilesService
{
    Task<UserProfileFull> CreateUserProfile(CreateUserProfile userProfileRequest);
}

public sealed class UserProfilesService : IUserProfilesService
{
    private readonly IMapper mapper;
    private readonly IRepository<UserProfile> userProfileRepository;

    public UserProfilesService(
        IMapper mapper,
        IRepository<UserProfile> userProfileRepository)
    {
        this.mapper = mapper;
        this.userProfileRepository = userProfileRepository;
    }

    #region Implementation

    public async Task<UserProfileFull> CreateUserProfile(CreateUserProfile userProfileRequest)
    {
        var userProfile = mapper.Map<UserProfile>(userProfileRequest);
        await userProfileRepository.AddAsync(userProfile);

        var result = mapper.Map<UserProfileFull>(userProfile);
        
        return result;
    }

    #endregion
}
