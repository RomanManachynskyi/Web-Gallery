using AutoMapper;
using Microsoft.AspNetCore.Http;
using NodaTime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;
using WebGallery.Shared.AWS.S3;

namespace WebGallery.Core.Service;

public interface IMyArtworksService
{
    Task<List<MyArtworkGeneral>> GetMyArtworks(GetMyArtworks request);
    Task<MyArtworkFull> GetMyArtwork(Guid artworkId);
    Task<MyArtworkFull> CreateMyArtwork(CreateArtwork request);
    Task<MyArtworkFull> UpdateMyArtwork(Guid artworkId, UpdateArtwork request);
    Task DeleteMyArtwork(Guid artworkId);
}

public sealed class MyArtworksService : IMyArtworksService
{
    private const string PictureBucket = "https://s3.eu-north-1.amazonaws.com/web-gallery.bucket";
    private readonly IMapper mapper;
    private readonly IUserData userData;

    private readonly IS3Service s3Service;
    private readonly IRepository<Like> likeRepository;
    private readonly IRepository<Bookmark> bookmarkRepository;
    private readonly IRepository<Hashtag> hashtagRepository;
    private readonly IRepository<Artwork> artworkRepository;
    private readonly IRepository<UserProfile> userProfileRepository;

    public MyArtworksService(
        IMapper mapper,
        IUserData userData,
        IS3Service s3Service,
        IRepository<Like> likeRepository,
        IRepository<Bookmark> bookmarkRepository,
        IRepository<Hashtag> hashtagRepository,
        IRepository<Artwork> artworkRepository,
        IRepository<UserProfile> userProfileRepository)
    {
        this.mapper = mapper;
        this.userData = userData;

        this.s3Service = s3Service;
        this.likeRepository = likeRepository;
        this.bookmarkRepository = bookmarkRepository;
        this.hashtagRepository = hashtagRepository;
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

    public async Task<MyArtworkFull> CreateMyArtwork(CreateArtwork request)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        if (request.Pictures is null || request.Pictures.Count == 0)
            throw new Exception("Pictures are required");

        var artwork = mapper.Map<Artwork>(request);

        artwork.PublishedAt = new LocalDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        artwork.UserProfile = userProfile;

        await artworkRepository.AddAsync(artwork);

        artwork.Hashtags = await AddHashtags(request.Hashtags, artwork);
        artwork.Pictures = await UploadPictures(userProfile.Id, request.Pictures, artwork);
        artwork.FrontPictureUrl = await CompressPicture(request.Pictures.First(), artwork.Id, userProfile.Id);

        await artworkRepository.UpdateAsync(artwork);
        await artworkRepository.SaveChangesAsync();

        var result = mapper.Map<MyArtworkFull>(artwork);

        return result;
    }

    public async Task<MyArtworkFull> UpdateMyArtwork(Guid artworkId, UpdateArtwork request)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");
        var artwork = await artworkRepository.FirstOrDefaultAsync(new GetMyArtworkByIdWithDependenciesSpecification(userProfile.Id, artworkId))
            ?? throw new NotFoundException("Artwork not found");

        artwork.Title = request.Title;
        artwork.Description = request.Description;
        artwork.OpenTo = request.OpenTo;
        artwork.AllowComments = request.AllowComments;
        artwork.IsOriginalWork = request.IsOriginalWork;
        artwork.IsFeatured = request.IsFeatured;

        if (request.Pictures is not null && request.Pictures.Count > 0)
        {
            await DeletePictures(artwork.FrontPictureUrl, artwork.Pictures);

            artwork.Pictures = await UploadPictures(userProfile.Id, request.Pictures, artwork);
            artwork.FrontPictureUrl = await CompressPicture(request.Pictures.First(), artwork.Id, userProfile.Id);
        }

        artwork.Hashtags = await AddHashtags(request.Hashtags, artwork);

        await artworkRepository.UpdateAsync(artwork);
        await artworkRepository.SaveChangesAsync();

        var result = mapper.Map<MyArtworkFull>(artwork);

        return result;
    }

    public async Task DeleteMyArtwork(Guid artworkId)
    {
        var userProfile = await userProfileRepository.FirstOrDefaultAsync(new GetUserProfileByCognitoUserIdSpecification(userData.Id))
            ?? throw new NotFoundException("User profile not found");

        var artwork = await artworkRepository.FirstOrDefaultAsync(new GetMyArtworkByIdWithDependenciesSpecification(userProfile.Id, artworkId))
            ?? throw new NotFoundException("Artwork not found");

        await RemoveHashtags(artwork.Hashtags);
        await DeletePictures(artwork.FrontPictureUrl, artwork.Pictures);

        await artworkRepository.DeleteAsync(artwork);
        await artworkRepository.SaveChangesAsync();
    }

    #endregion


    #region Helper Methods

    private async Task<List<Hashtag>> AddHashtags(IList<string> hashtags, Artwork artwork)
    {
        var result = new List<Hashtag>();

        foreach (var hashtag in hashtags)
        {
            var existingHashtag = await hashtagRepository.FirstOrDefaultAsync(new GetHashtagByNameSpecification(hashtag));

            if (existingHashtag is null)
                existingHashtag = new Hashtag { Name = hashtag, Artworks = new List<Artwork> { artwork } };
            else
                existingHashtag.Artworks.Add(artwork);

            existingHashtag.TotalUses++;

            result.Add(existingHashtag);
        }

        return result;
    }

    private async Task RemoveHashtags(IList<Hashtag> hashtags)
    {
        foreach (var hashtag in hashtags)
        {
            hashtag.TotalUses--;

            if (hashtag.TotalUses == 0)
                await hashtagRepository.DeleteAsync(hashtag);
        }
    }

    private async Task<string> CompressPicture(IFormFile picture, Guid artworkId, Guid userProfileId)
    {
        using var image = Image.Load(picture.OpenReadStream());

        image.Mutate(x => x
            .Resize(image.Width / 4, image.Height / 4));

        using var memoryStream = new MemoryStream();
        var encoder = new JpegEncoder { Quality = 50 };

        image.Save(memoryStream, encoder);
        memoryStream.Position = 0;

        var fileName = $"{artworkId}_p0_compressed.jpg";
        var filePath = $"artworks/{userProfileId}";
        await s3Service.PublishFile(fileName, filePath, memoryStream, "image/jpeg");

        var result = $"{PictureBucket}/{filePath}/{fileName}";

        return result;
    }

    private async Task<IList<Picture>> UploadPictures(Guid userProfileId, IList<IFormFile> pictures, Artwork artwork)
    {
        var currentPictureCount = 0;
        var filePath = $"artworks/{userProfileId}";
        var result = new List<Picture>();

        foreach (var picture in pictures)
        {
            var fileName = $"{artwork.Id}_p{currentPictureCount}.jpeg";
            await s3Service.PublishFile(fileName, filePath, picture);
            result.Add(new Picture { FullPictureUrl = $"{PictureBucket}/{filePath}/{fileName}", Artwork = artwork });
            currentPictureCount++;
        }

        return result;
    }

    private async Task DeletePictures(string frontpicture, IList<Picture> pictures)
    {
        var pictureUrls = pictures.Select(picture => picture.FullPictureUrl).ToList();
        pictureUrls.Add(frontpicture);

        foreach (var pictureUrl in pictureUrls)
        {
            var fileName = pictureUrl.Split('/').Last();
            var filePath = pictureUrl.Replace(PictureBucket, string.Empty).Replace(fileName, string.Empty).Trim('/');

            await s3Service.DeleteFile(filePath, fileName);
        }
    }

    #endregion
}
