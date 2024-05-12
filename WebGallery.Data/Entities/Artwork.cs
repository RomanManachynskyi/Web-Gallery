using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace WebGallery.Data.Entities;

public sealed class Artwork
{
    [Key]
    public Guid Id { get; set; }

    public UserProfile UserProfile { get; set; }

    public string? FrontPictureUrl { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public LocalDate PublishedAt { get; set; }

    public OpenTo OpenTo { get; set; }

    public bool AllowComments { get; set; }

    public bool IsOriginalWork { get; set; }

    public bool IsFeatured { get; set; }

    public uint TotalLikes { get; set; }

    public uint TotalViews { get; set; }

    public IList<Picture> Pictures { get; set; }

    public IList<Hashtag> Hashtags { get; set; }
}

public sealed class Picture
{
    [Key]
    public Guid Id { get; set; }

    public Artwork Artwork { get; set; }

    public string FullPictureUrl { get; set; }
}

public sealed class Hashtag
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public uint TotalUses { get; set; }

    public IList<Artwork> Artworks { get; set; }
}

public enum OpenTo
{
    Public,
    Private
}
