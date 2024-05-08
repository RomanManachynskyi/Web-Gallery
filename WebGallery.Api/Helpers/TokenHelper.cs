using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;

namespace WebGallery.Api.Helpers;

public static class TokenHelper
{
    public static JwtSecurityToken GetValidatedToken(string jwt, TokenValidationParameters validationParameters)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(jwt, validationParameters, out var rawValidatedToken);
            return rawValidatedToken as JwtSecurityToken;
        }
        catch (SecurityTokenValidationException stvEx)
        {
            throw new AuthenticationException($"Token failed validation: {stvEx.Message}");
        }
        catch (ArgumentException argEx)
        {
            throw new AuthenticationException($"Token was invalid: {argEx.Message}");
        }
    }

    public static JwtSecurityToken GetJwtSecurityToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken ?? throw new Exception("Could not read token.");

        return jwtToken;
    }

    public static TokenValidationParameters GetCognitoTokenValidationParams(string userPoolId, string region)
    {
        var issuer = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}"; // todo: move to config and format with string interpolation
        var jwtKeySetUrl = $"{issuer}/.well-known/jwks.json"; // todo same

        return new TokenValidationParameters
        {
            IssuerSigningKeyResolver = (_, _, _, _) =>
            {
                // get JsonWebKeySet from AWS
                var json = new WebClient().DownloadString(jwtKeySetUrl);
                var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json)!.Keys;
                return keys;
            },
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidAudience = userPoolId
        };
    }

    public static Claim GetClaimByType(this JwtSecurityToken token, string claimType) =>
        token.Claims.FirstOrDefault(claim => claim.Type == claimType);

    public static string GetHeaderByName(this IHeaderDictionary headers, string headerName) =>
        headers.FirstOrDefault(x =>
            string.Equals(x.Key, headerName, StringComparison.OrdinalIgnoreCase)).Value.FirstOrDefault();
}
