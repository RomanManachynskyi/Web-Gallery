using Ardalis.Specification;
using WebGallery.Core.Dtos;
using WebGallery.Core.Extentions;
using WebGallery.Data.Entities;

namespace WebGallery.Core.Service.Specification;

public sealed class ListBookmarksSpecification : Specification<Bookmark>
{
    public ListBookmarksSpecification(Guid userId, BookmarksRequest request)
    {
        Query
            .Include(likes => likes.Artwork)
            .Include(likes => likes.Artwork.UserProfile)
            .Include(likes => likes.Artwork.Hashtags)
            .Where(likes => likes.UserProfile.Id == userId)
            .Where(likes => string.IsNullOrEmpty(request.Search)
                            || likes.Artwork.Title.Contains(request.Search))
            .Where(likes => string.IsNullOrEmpty(request.Search)
                            || likes.Artwork.Description.Contains(request.Search))
            .Where(likes => string.IsNullOrEmpty(request.Search)
                            || likes.Artwork.UserProfile.Username.Contains(request.Search))
            .Where(likes => request.PublishedFrom == null || likes.Artwork.PublishedAt >= request.PublishedFrom)
            .Where(likes => request.PublishedTo == null || likes.Artwork.PublishedAt <= request.PublishedTo);

        Query.ApplyListRequest(request.SortParameters, request.PaginationParameters);
    }
}

public sealed class ListAllBookmarksByUserProfileIdSpecification : Specification<Bookmark>
{
    public ListAllBookmarksByUserProfileIdSpecification(Guid userId)
    {
        Query
            .Where(likes => likes.UserProfile.Id == userId);
    }
}

public sealed class GetBookmarkByUserProfileAndArtworkIdSpecification : Specification<Bookmark>
{
    public GetBookmarkByUserProfileAndArtworkIdSpecification(Guid userId, Guid artworkId)
    {
        Query
            .Where(likes => likes.UserProfile.Id == userId)
            .Where(likes => likes.Artwork.Id == artworkId);
    }
}
