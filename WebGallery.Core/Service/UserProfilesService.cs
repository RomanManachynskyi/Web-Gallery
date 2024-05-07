using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IUserProfilesService
{
    Task<UserProfileFull> GetUserProfileById(Guid userId);
    Task<UserProfileFull> CreateUserProfile(CreateUserProfile userProfileRequest);
    Task<UserProfileFull> UpdateUserProfile(Guid userId, UpdateUserProfile userProfileRequest);
    Task DeleteUserProfile(Guid userId);
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

    public async Task<UserProfileFull> GetUserProfileById(Guid userId)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User profile not found");

        var result = mapper.Map<UserProfileFull>(userProfile);

        return result;
    }

    public async Task<UserProfileFull> CreateUserProfile(CreateUserProfile userProfileRequest)
    {
        //TODO: Check if user already exists
        //if (await userProfileRepository.AnyAsync(new GetUserProfileByIdSpecification(userId)))
        //    throw new AlreadyExistsException("AUTH.SIGN_UP.PATIENT.PATIENT_ALREADY_EXISTS");

        var userProfile = mapper.Map<UserProfile>(userProfileRequest);
        await userProfileRepository.AddAsync(userProfile);

        var result = mapper.Map<UserProfileFull>(userProfile);
        
        return result;
    }

    public async Task<UserProfileFull> UpdateUserProfile(Guid userId, UpdateUserProfile userProfileRequest)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User profile not found");

        mapper.Map(userProfileRequest, userProfile);
        await userProfileRepository.UpdateAsync(userProfile);

        var result = mapper.Map<UserProfileFull>(userProfile);

        return result;
    }

    public async Task DeleteUserProfile(Guid userId)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User profile not found");

        await userProfileRepository.DeleteAsync(userProfile);
    }

    #endregion
}
