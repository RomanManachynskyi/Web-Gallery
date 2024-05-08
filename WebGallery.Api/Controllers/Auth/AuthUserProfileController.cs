using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers.Auth;

[Authorize]
[ApiController]
[Tags("User Profiles")]
[Route("api/v1/auth/sign-up/user-profile")]
public sealed class AuthUserProfileController : Controller
{
    public readonly IMyProfileService userProfilesService;

    public AuthUserProfileController(IMyProfileService userProfilesService)
    {
        this.userProfilesService = userProfilesService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserProfileFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileFull>> CreateUserProfile([FromBody] CreateMyProfile userProfileRequest)
    {
        var userProfile = await userProfilesService.CreateMyProfile(userProfileRequest);

        return StatusCode(StatusCodes.Status200OK, userProfile);
    }
}
