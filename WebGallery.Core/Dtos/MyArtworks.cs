using Microsoft.AspNetCore.Http;
using NodaTime;
using WebGallery.Data.Entities;

namespace WebGallery.Core.Dtos;

public sealed class MyArtworkFull
{
    public Guid Id { get; set; }
    public IList<PictureResponse> Pictures { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IList<HashtagsGeneral> Hashtags { get; set; }
    public LocalDate PublishedAt { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsOriginalWork { get; set; }
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
    public bool AllowComments { get; set; }
    public OpenTo OpenTo { get; set; }
    public uint TotalViews { get; set; }
    public uint TotalLikes { get; set; }
}

public sealed class MyArtworkGeneral
{
    public Guid Id { get; set; }
    public string Picture { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IList<HashtagsGeneral> Hashtags { get; set; }
    public LocalDate PublishedAt { get; set; }
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
}

public sealed class GetMyArtworks
{
    public string Search { get; set; }
    public bool? IsFeatured { get; set; }
    public LocalDate? PublishedFrom { get; set; }
    public LocalDate? PublishedTo { get; set; }
    public SortParameters SortParameters { get; set; }
    public PaginationParameters PaginationParameters { get; set; }
}

public sealed class CreateArtwork
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IList<IFormFile> Pictures { get; set; }
    public IList<string> Hashtags { get; set; }
    public OpenTo OpenTo { get; set; }
    public bool AllowComments { get; set; }
    public bool IsOriginalWork { get; set; }
    public bool IsFeatured { get; set; }
}
