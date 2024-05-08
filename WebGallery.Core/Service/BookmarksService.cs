using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IBookmarksService
{
    Task<List<ArtworksResponse>> GetBookmarks(BookmarksRequest bookmarksRequest);
    Task BookmarkArtwork(Guid artworkId);
}

public sealed class BookmarksService : IBookmarksService
{
    private readonly IMapper mapper;
    private readonly IUserData userData;

    private readonly IRepository<Like> likeRepository;
    private readonly IRepository<Bookmark> bookmarkRepository;
    private readonly IRepository<Artwork> artworkRepository;
    private readonly IRepository<UserProfile> userProfileRepository;

    public BookmarksService(
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

    public async Task<List<ArtworksResponse>> GetBookmarks(BookmarksRequest bookmarksRequest)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var likes = await likeRepository.ListAsync(new ListAllLikesByUserProfileIdSpecification(userProfile.Id));
        var bookmarks = await bookmarkRepository.ListAsync(new ListBookmarksSpecification(userProfile.Id, bookmarksRequest));

        var result = new List<ArtworksResponse>();

        foreach (var bookmark in bookmarks)
        {
            var artworkResponse = mapper.Map<ArtworksResponse>(bookmark.Artwork);
            artworkResponse.Artwork.IsLiked = likes.Any(like => like.Artwork.Id == artworkResponse.Artwork.Id);
            artworkResponse.Artwork.IsBookmarked = true;

            result.Add(artworkResponse);
        }

        return result;
    }

    public async Task BookmarkArtwork(Guid artworkId)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var bookmark = await bookmarkRepository.FirstOrDefaultAsync(new GetBookmarkByUserProfileAndArtworkIdSpecification(userProfile.Id, artworkId));
        var artwork = await artworkRepository.GetByIdAsync(artworkId);

        if (bookmark is not null)
            await bookmarkRepository.DeleteAsync(bookmark);
        else
        {
            bookmark = new Bookmark
            {
                UserProfile = userProfile,
                Artwork = artwork
            };

            await bookmarkRepository.AddAsync(bookmark);
        }

        await bookmarkRepository.SaveChangesAsync();
    }

    #endregion
}
