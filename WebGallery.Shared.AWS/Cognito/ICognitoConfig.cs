namespace WebGallery.Shared.AWS.Cognito;

public interface ICognitoConfig : IAWSConfig
{
    string Authority { get; set; }
    string UserPoolId { get; set; }
    string AppClientId { get; set; }
    string AppClientSecret { get; set; }
    string ConfirmationLink { get; set; }
}

public sealed class CognitoConfig : ICognitoConfig
{
    public string Authority { get; set; } = string.Empty;
    public string UserPoolId { get; set; } = string.Empty;
    public string AppClientId { get; set; } = string.Empty;
    public string AppClientSecret { get; set; } = string.Empty;
    public string ConfirmationLink { get; set; } = string.Empty;
    public string RegionEndpoint { get; set; } = string.Empty;
}
