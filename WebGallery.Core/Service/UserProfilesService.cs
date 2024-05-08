using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IUserProfilesService
{
    Task<List<UserProfileGeneral>> GetUserProfiles(UserProfilesRequest profilesRequest);
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

    public async Task<List<UserProfileGeneral>> GetUserProfiles(UserProfilesRequest profilesRequest)
    {
        var profiles = await userProfileRepository.ListAsync(new ListUserProfilesSpecification(profilesRequest));

        var result = mapper.Map<List<UserProfileGeneral>>(profiles);

        return result;
    }
}
