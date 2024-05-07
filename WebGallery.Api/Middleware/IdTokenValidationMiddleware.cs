using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Security.Authentication;
using WebGallery.Api.Helpers;

namespace WebGallery.Api.Middleware;

public class IdTokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public IdTokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint?.Metadata.GetMetadata<IAuthorizeData>() is null
            || endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            await _next(context);
            return;
        }

        var region = _configuration["AWS:Cognito:RegionEndpoint"];
        var userPoolId = _configuration["AWS:Cognito:UserPoolId"];

        var idTokenHeader = context.Request.Headers.GetHeaderByName(AuthConsts.IdTokenHeaderName);
        var idTokenHeaderValue = !string.IsNullOrEmpty(idTokenHeader)
            ? AuthenticationHeaderValue.Parse(idTokenHeader)
            : null;

        if (idTokenHeaderValue is not { Parameter: not null })
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Id token is missing");
            return;
        }

        var validationParameters = TokenHelper.GetCognitoTokenValidationParams(userPoolId, region);
        var idToken = TokenHelper.GetValidatedToken(idTokenHeaderValue.Parameter, validationParameters);

        var authorizationHeader = context.Request.Headers.GetHeaderByName(AuthConsts.AuthorizationHeaderName);
        var authorizationHeaderValue = AuthenticationHeaderValue.Parse(authorizationHeader);
        if (authorizationHeaderValue.Parameter is not null)
        {
            var authorizationToken = TokenHelper.GetValidatedToken(authorizationHeaderValue.Parameter, validationParameters);
            var idTokenClaim = idToken.GetClaimByType(AuthConsts.ClaimTypeSub)?.Value;
            var authTokenClaim = authorizationToken.GetClaimByType(AuthConsts.ClaimTypeUserName)?.Value;

            if (idTokenClaim != authTokenClaim)
                throw new AuthenticationException("ID Token not sync with Authorization Token.");
        }

        await _next(context);
    }
}
