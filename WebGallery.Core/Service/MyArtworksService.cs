using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IMyArtworksService
{
    Task<List<MyArtworkGeneral>> GetMyArtworks(GetMyArtworks request);
    Task<MyArtworkFull> GetMyArtwork(Guid artworkId);
}

public sealed class MyArtworksService : IMyArtworksService
{
    private readonly IMapper mapper;
    private readonly IUserData userData;

    private readonly IRepository<Like> likeRepository;
    private readonly IRepository<Bookmark> bookmarkRepository;
    private readonly IRepository<Artwork> artworkRepository;
    private readonly IRepository<UserProfile> userProfileRepository;

    public MyArtworksService(
        IMapper mapper,
        IUserData userData,
        IRepository<Like> likeRepository,
        IRepository<Bookmark> bookmarkRepository,
        IRepository<Artwork> artworkRepository,
        IRepository<UserProfile> userProfileRepository)
    {
        this.mapper = mapper;
        this.userData = userData;

        this.likeRepository = likeRepository;
        this.bookmarkRepository = bookmarkRepository;
        this.artworkRepository = artworkRepository;
        this.userProfileRepository = userProfileRepository;
    }

    #region Implementation

    public async Task<List<MyArtworkGeneral>> GetMyArtworks(GetMyArtworks request)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var artworks = await artworkRepository.ListAsync(new ListMyArtworksSpecification(userProfile.Id, request));
        var likes = await likeRepository.ListAsync(new ListAllLikesByUserProfileIdSpecification(userProfile.Id));
        var bookmarks = await bookmarkRepository.ListAsync(new ListAllBookmarksByUserProfileIdSpecification(userProfile.Id));

        var result = new List<MyArtworkGeneral>();

        foreach (var artwork in artworks)
        {
            var artworkResponse = mapper.Map<MyArtworkGeneral>(artwork);
            artworkResponse.IsLiked = likes.Any(like => like.Artwork == null || like.Artwork.Id == artworkResponse.Id);
            artworkResponse.IsBookmarked = bookmarks.Any(bookmark => bookmark.Artwork == null || bookmark.Artwork.Id == artworkResponse.Id);

            result.Add(artworkResponse);
        }

        return result;
    }

    public async Task<MyArtworkFull> GetMyArtwork(Guid artworkId)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var artwork = await artworkRepository.FirstOrDefaultAsync(new GetMyArtworkByIdWithDependenciesSpecification(userProfile.Id, artworkId))
                ?? throw new NotFoundException("Artwork not found");

        artwork.TotalViews++;
        await artworkRepository.SaveChangesAsync();

        var result = mapper.Map<MyArtworkFull>(artwork);

        var likes = await likeRepository.FirstOrDefaultAsync(new GetLikeByUserProfileAndArtworkIdSpecification(userProfile.Id, artwork.Id));
        result.IsLiked = likes is not null;

        var bookmarks = await bookmarkRepository.FirstOrDefaultAsync(new GetBookmarkByUserProfileAndArtworkIdSpecification(userProfile.Id, artwork.Id));
        result.IsBookmarked = bookmarks is not null;

        return result;
    }

    #endregion
}
