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
    private readonly ICognitoConfig cognitoConfig;
    private readonly IAmazonCognitoIdentityProvider cognitoClient;

    public CognitoService(ICognitoConfig cognitoConfig)
    {
        this.cognitoConfig = cognitoConfig;
        cognitoClient = new AmazonCognitoIdentityProviderClient(cognitoConfig.GetRegionEndpoint());
    }

    #region Implementation

    public async Task<SignUpResponse> SignUp(SignUp signUp)
    {
        var signUpRequest = CreateSignUpRequest(signUp);

        signUpRequest.SecretHash = CalculateSecretHash(signUp.Email);

        return await cognitoClient.SignUpAsync(signUpRequest);
    }

    public async Task<InitiateAuthResponse> SignIn(SignIn signIn)
    {
        var (email, password) = signIn;
        var authRequest = CreateSignInRequest(email, password);

        var response = await cognitoClient.InitiateAuthAsync(authRequest);

        return response;
    }

    public async Task<CodeDeliveryDetailsType> ResendConfirmationCode(ResendConfirmationCode resendConfirmation)
    {
        var codeRequest = CreateResendConfirmationRequest(resendConfirmation.Email);
        var response = await cognitoClient.ResendConfirmationCodeAsync(codeRequest);

        return response.CodeDeliveryDetails;
    }

    public string GetConfirmationLink(ConfirmSignUp confirmSignUp)
    {
        var (email, confirmationCode) = confirmSignUp;
        var confirmationLink = cognitoConfig.ConfirmationLink;

        if (string.IsNullOrEmpty(confirmationLink))
            throw new InvalidOperationException("no confirmation link is available");

        return confirmationLink
            .Replace("{user_pool_domain}", cognitoConfig.UserPoolDomain)
            .Replace("{region}", cognitoConfig.RegionEndpoint)
            .Replace("{client_id}", cognitoConfig.AppClientId)
            .Replace("{email}", email)
            .Replace("{cognito_confirmation_code}", confirmationCode);
    }

    #endregion

    #region Helper Methods

    private SignUpRequest CreateSignUpRequest(SignUp userSignUp)
    {
        return new SignUpRequest
        {
            ClientId = cognitoConfig.AppClientId,
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
        ClientId = cognitoConfig.AppClientId,
        Username = email,
        SecretHash = CalculateSecretHash(email)
    };

    private InitiateAuthRequest CreateSignInRequest(string email, string password) => new()
    {
        ClientId = cognitoConfig.AppClientId,
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
        var data = username + cognitoConfig.AppClientId;

        using var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(cognitoConfig.AppClientSecret));
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hashBytes);
    }

    #endregion
}
