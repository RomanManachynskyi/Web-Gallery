using System.ComponentModel.DataAnnotations;

namespace WebGallery.Data.Entities;

public sealed class Artworks
{
    [Key]
    public Guid Id { get; set; }

    public UserProfiles UserProfile { get; set; }

    public string CompressedFrontPictureUrl { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime PublishedAt { get; set; }

    public OpenTo OpenTo { get; set; }

    public bool AllowComments { get; set; }

    public bool IsOriginalWork { get; set; }

    public bool IsFeatured { get; set; }

    public bool TotalLikes { get; set; }

    public bool TotalViews { get; set; }

    public IList<Pictures> Pictures { get; set; }

    public IList<ArtworkTags> Tags { get; set; }
}

public sealed class Pictures
{
    [Key]
    public Guid Id { get; set; }

    public Artworks Artwork { get; set; }

    public string FullPictureUrl { get; set; }
}

public sealed class ArtworkTags
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int TotalUses { get; set; }

    public IList<Artworks> Artworks { get; set; }
}

public enum OpenTo
{
    Public,
    Private
}
