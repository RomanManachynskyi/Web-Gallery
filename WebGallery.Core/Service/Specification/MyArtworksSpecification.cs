using Ardalis.Specification;
using WebGallery.Core.Dtos;
using WebGallery.Core.Extentions;
using WebGallery.Data.Entities;

namespace WebGallery.Core.Service.Specification;

public sealed class GetMyArtworkByIdWithDependenciesSpecification : Specification<Artwork>
{
    public GetMyArtworkByIdWithDependenciesSpecification(Guid userProfileId, Guid artworkId)
    {
        Query
            .Include(artwork => artwork.Pictures)
            .Include(artwork => artwork.Hashtags)
            .Where(artwork => artwork.UserProfile.Id == userProfileId)
            .Where(artwork => artwork.Id == artworkId);
    }
}

public sealed class ListMyArtworksSpecification : Specification<Artwork>
{
    public ListMyArtworksSpecification(Guid currentUserId, GetMyArtworks request)
    {
        Query
            .Include(artwork => artwork.Hashtags)
            .Where(artwork => artwork.UserProfile.Id == currentUserId)
            .Where(artwork => string.IsNullOrEmpty(request.Search)
                            || artwork.Title.Contains(request.Search))
            .Where(artwork => string.IsNullOrEmpty(request.Search)
                            || artwork.Description.Contains(request.Search))
            .Where(artwork => request.IsFeatured == null || artwork.IsFeatured == request.IsFeatured)
            .Where(artwork => request.PublishedFrom == null || artwork.PublishedAt >= request.PublishedFrom)
            .Where(artwork => request.PublishedTo == null || artwork.PublishedAt <= request.PublishedTo);

        Query.ApplyListRequest(request.SortParameters, request.PaginationParameters);
    }
}
