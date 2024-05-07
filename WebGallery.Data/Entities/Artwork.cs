using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace WebGallery.Data.Entities;

public sealed class Artwork
{
    [Key]
    public Guid Id { get; set; }

    public UserProfile UserProfile { get; set; }

    public string CompressedFrontPictureUrl { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public LocalDate PublishedAt { get; set; }

    public OpenTo OpenTo { get; set; }

    public bool AllowComments { get; set; }

    public bool IsOriginalWork { get; set; }

    public bool IsFeatured { get; set; }

    public bool TotalLikes { get; set; }

    public bool TotalViews { get; set; }

    public IList<Picture> Pictures { get; set; }

    public IList<ArtworkTag> Tags { get; set; }
}

public sealed class Picture
{
    [Key]
    public Guid Id { get; set; }

    public Artwork Artwork { get; set; }

    public string FullPictureUrl { get; set; }
}

public sealed class ArtworkTag
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int TotalUses { get; set; }

    public IList<Artwork> Artworks { get; set; }
}

public enum OpenTo
{
    Public,
    Private
}
