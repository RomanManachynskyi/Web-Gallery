using Ardalis.Specification;
using WebGallery.Core.Dtos;
using WebGallery.Core.Extentions;
using WebGallery.Data.Entities;

namespace WebGallery.Core.Service.Specification;

public sealed class ListHashtagSpecification : Specification<Hashtag>
{
    public ListHashtagSpecification(HashtagRequest request)
    {
        Query
            .Where(hashtag => string.IsNullOrEmpty(request.Search)
                            || hashtag.Name.Contains(request.Search))
            .Where(hashtag => request.TotalUsesFrom == null
                            || hashtag.TotalUses >= request.TotalUsesFrom)
            .Where(hashtag => request.TotalUsesTo == null
                            || hashtag.TotalUses <= request.TotalUsesTo);

        Query.ApplyListRequest(request.SortParameters, request.PaginationParameters);
    }
}
