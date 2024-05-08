using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IArtworksService
{
    Task<List<ArtworksResponse>> GetArtworks(ArtworksRequest artworksRequest);
    Task<ArtworkResponse> GetArtwork(Guid artworkId);
}

public sealed class ArtworksService : IArtworksService
{
    private readonly IMapper mapper;
    private readonly IUserData userData;

    private readonly IRepository<Like> repository;
    private readonly IRepository<Artwork> artworkRepository;
    private readonly IRepository<UserProfile> userProfileRepository;

    public ArtworksService(
        IMapper mapper,
        IUserData userData,
        IRepository<Like> repository,
        IRepository<Artwork> artworkRepository,
        IRepository<UserProfile> userProfileRepository)
    {
        this.mapper = mapper;
        this.userData = userData;
        this.repository = repository;
        this.artworkRepository = artworkRepository;
        this.userProfileRepository = userProfileRepository;
    }

    #region Implementation

    public async Task<List<ArtworksResponse>> GetArtworks(ArtworksRequest artworksRequest)
    {
        var artworks = await artworkRepository.ListAsync(new ListArtworksSpecification(artworksRequest));

        var result = mapper.Map<List<ArtworksResponse>>(artworks);

        return result;
    }

    public async Task<ArtworkResponse> GetArtwork(Guid artworkId)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var artwork = await artworkRepository.FirstOrDefaultAsync(new GetArtworkByIdWithDependenciesSpecification(artworkId))
                ?? throw new NotFoundException("Artwork not found");

        artwork.TotalViews++;
        await artworkRepository.SaveChangesAsync();

        var result = mapper.Map<ArtworkResponse>(artwork);

        var likes = await repository.FirstOrDefaultAsync(new GetLikeByUserProfileAndArtworkIdSpecification(userProfile.Id, artwork.Id));
        result.IsLiked = likes is not null;

        return result;
    }

    #endregion
}
