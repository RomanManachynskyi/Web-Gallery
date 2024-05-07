using WebGallery.Core.Attributes;

namespace WebGallery.Core.Dtos;

public sealed class SortParameters
{
    public string Sort { get; set; }

    public bool OrderByAscending { get; set; }
}

public sealed class PaginationParameters
{
    [ListRequestOffset]
    public int Offset { get; set; }

    [ListRequestLimit]
    public int Limit { get; set; }
}
