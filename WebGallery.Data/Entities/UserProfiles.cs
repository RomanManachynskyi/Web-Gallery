using System.ComponentModel.DataAnnotations;

namespace WebGallery.Data.Entities;

public sealed class UserProfiles
{
    [Key]
    public Guid Id { get; set; }

    public Guid CognitoUserId { get; set; }

    public string Username { get; set; }

    public string ProfilePictureUrl { get; set; }

    public string ProfileCoverPictureUrl { get; set; }

    public Gender Gender { get; set; }

    public string Country { get; set; }

    public DateTime BirthDate { get; set; }

    public string Occupation { get; set; }

    public int TotalArtworks { get; set; }

    public IList<Artworks> Artworks { get; set; }

    public int TotalFollowers { get; set; }

    public int TotalFollowing { get; set; }
}

public enum Gender
{
    Male,
    Female,
    Other
}
