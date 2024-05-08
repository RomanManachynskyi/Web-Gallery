﻿using AutoMapper;
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

    private readonly IRepository<Like> likeRepository;
    private readonly IRepository<Bookmark> bookmarkRepository;
    private readonly IRepository<Artwork> artworkRepository;
    private readonly IRepository<UserProfile> userProfileRepository;

    public ArtworksService(
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

    public async Task<List<ArtworksResponse>> GetArtworks(ArtworksRequest artworksRequest)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var artworks = await artworkRepository.ListAsync(new ListArtworksSpecification(artworksRequest));
        var likes = await likeRepository.ListAsync(new ListAllLikesByUserProfileIdSpecification(userProfile.Id));
        var bookmarks = await bookmarkRepository.ListAsync(new ListAllBookmarksByUserProfileIdSpecification(userProfile.Id));

        var result = new List<ArtworksResponse>();

        foreach (var artwork in artworks)
        {
            var artworkResponse = mapper.Map<ArtworksResponse>(artwork);
            artworkResponse.Artwork.IsLiked = likes.Any(like => like.Artwork.Id == artworkResponse.Artwork.Id);
            artworkResponse.Artwork.IsBookmarked = bookmarks.Any(bookmark => bookmark.Artwork.Id == artworkResponse.Artwork.Id);

            result.Add(artworkResponse);
        }

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

        var likes = await likeRepository.FirstOrDefaultAsync(new GetLikeByUserProfileAndArtworkIdSpecification(userProfile.Id, artwork.Id));
        result.IsLiked = likes is not null;

        return result;
    }

    #endregion
}
