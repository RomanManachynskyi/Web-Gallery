using NodaTime;

namespace WebGallery.Core.Dtos;

public sealed class ArtworksResponse
{
    public UserProfileGeneral UserProfile { get; set; }

    public ArtworksGeneral Artwork { get; set; }
}

public sealed class ArtworksRequest
{
    public string Search { get; set; }

    public bool? IsFeatured { get; set; }

    public LocalDate? PublishedFrom { get; set; }

    public LocalDate? PublishedTo { get; set; }

    public SortParameters SortParameters { get; set; }

    public PaginationParameters PaginationParameters { get; set; }
}

#region Helper Dtos
public sealed class ArtworksGeneral
{
    public Guid Id { get; set; }

    public string Picture { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public IList<ArtworkTagsGeneral> Tags { get; set; }

    public LocalDate PublishedAt { get; set; }

    public bool IsLiked { get; set; }

    public bool IsBookmarked { get; set; }
}

public sealed class ArtworkTagsGeneral
{
    public Guid Id { get; set; }

    public string Name { get; set; }
}

#endregion
