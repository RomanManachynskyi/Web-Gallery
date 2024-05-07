using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IMyProfileService
{
    Task<UserProfileFull> GetMyProfile();
    Task<UserProfileFull> CreateMyProfile(CreateMyProfile userProfileRequest);
    Task<UserProfileFull> UpdateMyProfile(UpdateMyProfile userProfileRequest);
    Task DeleteMyProfile();
}

public sealed class MyProfileService : IMyProfileService
{
    private readonly IMapper mapper;
    private readonly IUserData userData;
    private readonly IRepository<UserProfile> userProfileRepository;

    public MyProfileService(
        IMapper mapper,
        IUserData userData,
        IRepository<UserProfile> userProfileRepository)
    {
        this.mapper = mapper;
        this.userData = userData;
        this.userProfileRepository = userProfileRepository;
    }

    #region Implementation

    public async Task<UserProfileFull> GetMyProfile()
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var result = mapper.Map<UserProfileFull>(userProfile);

        return result;
    }

    public async Task<UserProfileFull> CreateMyProfile(CreateMyProfile profileRequest)
    {
        if (await userProfileRepository.AnyAsync(new GetUserProfileByEmailSpecification(userData.Email)))
            throw new AlreadyExistsException("Email is already used");

        var userProfile = mapper.Map<UserProfile>(profileRequest);
        userProfile.CognitoUserId = userData.Id;
        userProfile.Email = userData.Email;
        await userProfileRepository.AddAsync(userProfile);

        var result = mapper.Map<UserProfileFull>(userProfile);
        
        return result;
    }

    public async Task<UserProfileFull> UpdateMyProfile(UpdateMyProfile profileRequest)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        mapper.Map(profileRequest, userProfile);
        await userProfileRepository.UpdateAsync(userProfile);

        var result = mapper.Map<UserProfileFull>(userProfile);

        return result;
    }

    public async Task DeleteMyProfile()
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        await userProfileRepository.DeleteAsync(userProfile);
    }

    #endregion
}
