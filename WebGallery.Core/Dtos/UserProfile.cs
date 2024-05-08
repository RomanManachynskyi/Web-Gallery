using NodaTime;
using WebGallery.Data.Entities;

namespace WebGallery.Core.Dtos;

public sealed class UserProfileGeneral
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string ProfilePictureUrl { get; set; }
}

public sealed class UserProfileFull
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string ProfileCoverPictureUrl { get; set; }
    public Gender Gender { get; set; }
    public string Country { get; set; }
    public LocalDate BirthDate { get; set; }
    public string Occupation { get; set; }
    public uint TotalWorks { get; set; }
    public uint Followers { get; set; }
    public uint Following { get; set; }
}

public sealed class UserProfilesRequest
{
    public string Search { get; set; }
    public SortParameters SortParameters { get; set; }
    public PaginationParameters PaginationParameters { get; set; }
}

public sealed class CreateMyProfile
{
    public string Username { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string ProfileCoverPictureUrl { get; set; }
    public Gender Gender { get; set; }
    public string Country { get; set; }
    public LocalDate BirthDate { get; set; }
    public string Occupation { get; set; }
}

public sealed class UpdateMyProfile
{
    public string Username { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string ProfileCoverPictureUrl { get; set; }
    public Gender? Gender { get; set; }
    public string Country { get; set; }
    public LocalDate? BirthDate { get; set; }
    public string Occupation { get; set; }
}
