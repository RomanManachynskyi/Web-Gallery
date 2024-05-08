using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace WebGallery.Data.Entities;

public sealed class UserProfile
{
    [Key]
    public Guid Id { get; set; }
    public Guid CognitoUserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string ProfileCoverPictureUrl { get; set; }
    public Gender Gender { get; set; }
    public string Country { get; set; }
    public LocalDate BirthDate { get; set; }
    public string Occupation { get; set; }
    public int TotalArtworks { get; set; }
    public IList<Artwork> Artworks { get; set; }
    public int TotalFollowers { get; set; }
    public int TotalFollowing { get; set; }
}

public sealed class Bookmark
{
    [Key]
    public Guid Id { get; set; }
    public UserProfile UserProfile { get; set; }
    public Artwork Artwork { get; set; }
}

public sealed class Like
{
    [Key]
    public Guid Id { get; set; }
    public UserProfile UserProfile { get; set; }
    public Artwork Artwork { get; set; }
}

public enum Gender
{
    Male,
    Female,
    Other
}
