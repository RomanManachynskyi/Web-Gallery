namespace WebGallery.Core.Dtos;

public interface IUserData
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}

public sealed class UserData : IUserData
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}
