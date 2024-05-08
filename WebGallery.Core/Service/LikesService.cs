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
    Task LikeArtwork(Guid artworkId);
}

public sealed class LikesService : ILikesService
{
    private readonly IMapper mapper;
    private readonly IUserData userData;

    private readonly IRepository<Like> likeRepository;
    private readonly IRepository<Bookmark> bookmarkRepository;
    private readonly IRepository<Artwork> artworkRepository;
    private readonly IRepository<UserProfile> userProfileRepository;

    public LikesService(
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

    public async Task<List<ArtworksResponse>> GetLikes(LikesRequest likesRequest)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var likes = await likeRepository.ListAsync(new ListLikesSpecification(userProfile.Id, likesRequest));
        var bookmarks = await bookmarkRepository.ListAsync(new ListAllBookmarksByUserProfileIdSpecification(userProfile.Id));

        var result = new List<ArtworksResponse>();

        foreach (var like in likes)
        {
            var artworkResponse = mapper.Map<ArtworksResponse>(like.Artwork);
            artworkResponse.Artwork.IsLiked = true;
            artworkResponse.Artwork.IsBookmarked = bookmarks.Any(bookmark => bookmark.Artwork.Id == artworkResponse.Artwork.Id);

            result.Add(artworkResponse);
        }

        return result;
    }

    public async Task LikeArtwork(Guid artworkId)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var like = await likeRepository.FirstOrDefaultAsync(new GetLikeByUserProfileAndArtworkIdSpecification(userProfile.Id, artworkId));
        var artwork = await artworkRepository.GetByIdAsync(artworkId);

        if (like is not null)
        {
            await likeRepository.DeleteAsync(like);
            artwork.TotalLikes--;
        }
        else
        {
            like = new Like
            {
                UserProfile = userProfile,
                Artwork = artwork
            };

            await likeRepository.AddAsync(like);
            artwork.TotalLikes++;
        }

        await likeRepository.SaveChangesAsync();
    }

    #endregion
}
