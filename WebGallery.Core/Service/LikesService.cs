using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface ILikesService
{
    Task<List<ArtworksResponse>> GetLikes(LikesRequest likesRequest);
}

public sealed class LikesService : ILikesService
{
    private readonly IMapper mapper;
    private readonly IUserData userData;
    private readonly IRepository<Like> likesRepository;
    private readonly IRepository<UserProfile> userProfileRepository;

    public LikesService(
        IMapper mapper,
        IUserData userData,
        IRepository<Like> likesRepository,
        IRepository<UserProfile> userProfileRepository)
    {
        this.mapper = mapper;
        this.userData = userData;
        this.likesRepository = likesRepository;
        this.userProfileRepository = userProfileRepository;
    }

    #region Implementation

    public async Task<List<ArtworksResponse>> GetLikes(LikesRequest likesRequest)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var likes = await likesRepository.ListAsync(new ListLikesSpecification(userProfile.Id, likesRequest));

        var result = mapper.Map<List<ArtworksResponse>>(likes);

        return result;
    }

    #endregion
}
