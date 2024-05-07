using Ardalis.Specification;
using WebGallery.Core.Dtos;
using WebGallery.Core.Extentions;
using WebGallery.Data.Entities;

namespace WebGallery.Core.Service.Specification;

public sealed class ListArtworksSpecification : Specification<Artwork>
{
    public ListArtworksSpecification(ArtworksRequest request)
    {
        Query
            .Include(artwork => artwork.UserProfile)
            .Include(artwork => artwork.Tags)
            .Where(artwork => string.IsNullOrEmpty(request.Search)
                            || artwork.Title.Contains(request.Search))
            .Where(artwork => string.IsNullOrEmpty(request.Search)
                            || artwork.Description.Contains(request.Search))
            .Where(artwork => string.IsNullOrEmpty(request.Search)
                            || artwork.UserProfile.Username.Contains(request.Search))
            .Where(artwork => request.PublishedFrom == null || artwork.PublishedAt >= request.PublishedFrom)
            .Where(artwork => request.PublishedTo == null || artwork.PublishedAt <= request.PublishedTo);

        Query.ApplyListRequest(request.SortParameters, request.PaginationParameters);
    }
}
