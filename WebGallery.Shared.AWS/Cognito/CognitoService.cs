using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System.Security.Cryptography;
using System.Text;

namespace WebGallery.Shared.AWS.Cognito;

public interface ICognitoService
{
    Task<SignUpResponse> SignUp(SignUp signUp);
    Task<InitiateAuthResponse> SignIn(SignIn signIn);
    Task<CodeDeliveryDetailsType> ResendConfirmationCode(ResendConfirmationCode resendConfirmation);
    string GetConfirmationLink(ConfirmSignUp confirmSignUp);
}

public class CognitoService : ICognitoService
{
    private readonly ICognitoConfig _cognitoConfig;
    private readonly IAmazonCognitoIdentityProvider _cognitoClient;

    public CognitoService(ICognitoConfig cognitoConfig)
    {
        _cognitoConfig = cognitoConfig;
        _cognitoClient = new AmazonCognitoIdentityProviderClient(cognitoConfig.GetRegionEndpoint());
    }

    #region Implementation

    public async Task<SignUpResponse> SignUp(SignUp signUp)
    {
        var signUpRequest = CreateSignUpRequest(signUp);

        signUpRequest.SecretHash = CalculateSecretHash(signUp.Email);

        return await _cognitoClient.SignUpAsync(signUpRequest);
    }

    public async Task<InitiateAuthResponse> SignIn(SignIn signIn)
    {
        var (email, password) = signIn;
        var authRequest = CreateSignInRequest(email, password);

        var response = await _cognitoClient.InitiateAuthAsync(authRequest);

        return response;
    }

    public async Task<CodeDeliveryDetailsType> ResendConfirmationCode(ResendConfirmationCode resendConfirmation)
    {
        var codeRequest = CreateResendConfirmationRequest(resendConfirmation.Email);
        var response = await _cognitoClient.ResendConfirmationCodeAsync(codeRequest);

        return response.CodeDeliveryDetails;
    }

    public string GetConfirmationLink(ConfirmSignUp confirmSignUp)
    {
        var (email, confirmationCode) = confirmSignUp;
        var confirmationLink = _cognitoConfig.ConfirmationLink;

        if (string.IsNullOrEmpty(confirmationLink))
            throw new InvalidOperationException("no confirmation link is available");

        return confirmationLink
            .Replace("{user_pool_domain}", _cognitoConfig.UserPoolDomain)
            .Replace("{region}", _cognitoConfig.RegionEndpoint)
            .Replace("{client_id}", _cognitoConfig.AppClientId)
            .Replace("{email}", email)
            .Replace("{cognito_confirmation_code}", confirmationCode);
    }

    #endregion

    #region Helper Methods

    private SignUpRequest CreateSignUpRequest(SignUp userSignUp)
    {
        return new SignUpRequest
        {
            ClientId = _cognitoConfig.AppClientId,
            Username = userSignUp.Email,
            Password = userSignUp.Password,
            UserAttributes = new List<AttributeType>
            {
                new() { Name = "email", Value = userSignUp.Email },
            }
        };
    }

    private ResendConfirmationCodeRequest CreateResendConfirmationRequest(string email) => new()
    {
        ClientId = _cognitoConfig.AppClientId,
        Username = email,
        SecretHash = CalculateSecretHash(email)
    };

    private InitiateAuthRequest CreateSignInRequest(string email, string password) => new()
    {
        ClientId = _cognitoConfig.AppClientId,
        AuthParameters = CreateAuthParameters(email, password),
        AuthFlow = AuthFlowType.USER_PASSWORD_AUTH
    };

    private Dictionary<string, string> CreateAuthParameters(string email, string password)
    {
        var secretHash = CalculateSecretHash(email);

        return new Dictionary<string, string>
        {
            { "USERNAME", email },
            { "PASSWORD", password },
            { "SECRET_HASH", secretHash }
        };
    }

    private string CalculateSecretHash(string username)
    {
        var data = username + _cognitoConfig.AppClientId;

        using var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(_cognitoConfig.AppClientSecret));
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hashBytes);
    }

    #endregion
}
