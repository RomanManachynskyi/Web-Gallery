using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using WebGallery.Api.Helpers;
using WebGallery.Core.Dtos;

namespace WebGallery.Api.Middleware;

public class UserDataMiddleware
{
    private readonly RequestDelegate _next;

    public UserDataMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, IUserData userData)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint?.Metadata.GetMetadata<IAuthorizeData>() is null
            || endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            await _next(context);
            return;
        }

        var idToken = ParseToken(context);
        var userDetails = GetUserDetails(idToken);
        userData.Id = userDetails.Id;
        userData.Email = userDetails.Email;

        await _next(context);
    }

    private static JwtSecurityToken ParseToken(HttpContext httpContext)
    {
        var idTokenHeader = httpContext.Request.Headers.GetHeaderByName(AuthConsts.IdTokenHeaderName);
        if (idTokenHeader is null)
            throw new ArgumentNullException(nameof(idTokenHeader));

        var idTokenHeaderValue = AuthenticationHeaderValue.Parse(idTokenHeader);
        if (idTokenHeaderValue is not { Parameter: not null })
            throw new ArgumentNullException(nameof(idTokenHeaderValue.Parameter));

        var token = TokenHelper.GetJwtSecurityToken(idTokenHeaderValue.Parameter);

        return token;
    }

    private static UserData GetUserDetails(JwtSecurityToken token) => new UserData
    {
        Id = Guid.Parse(token.GetClaimByType(AuthConsts.ClaimTypeSub)?.Value
                   ?? throw new ArgumentNullException(nameof(token), "The token does not contain a sub claim")),
        Email = token.GetClaimByType(AuthConsts.ClaimTypeEmail)?.Value ?? string.Empty
    };
}