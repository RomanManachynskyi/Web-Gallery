namespace WebGallery.Shared.AWS.IAM;

public interface IIamConfig : IAWSConfig
{
    string AccessKeyId { get; set; }
    string SecretAccessKey { get; set; }
}

public sealed class IamConfig : IIamConfig
{
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string RegionEndpoint { get; set; } = string.Empty;
}
