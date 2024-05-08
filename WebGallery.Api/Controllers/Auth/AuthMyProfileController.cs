using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers.Auth;

[Authorize]
[ApiController]
[Tags("User Profiles")]
[Route("api/v1/auth/user-profile")]
public sealed class AuthMyProfileController : Controller
{
    public readonly IMyProfileService userProfilesService;

    public AuthMyProfileController(IMyProfileService userProfilesService)
    {
        this.userProfilesService = userProfilesService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(UserProfileFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileFull>> GetUserProfileById()
    {
        var userProfile = await userProfilesService.GetMyProfile();

        return StatusCode(StatusCodes.Status200OK, userProfile);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserProfileFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileFull>> CreateUserProfile([FromBody] CreateMyProfile userProfileRequest)
    {
        var userProfile = await userProfilesService.CreateMyProfile(userProfileRequest);

        return StatusCode(StatusCodes.Status200OK, userProfile);
    }

    [HttpPut]
    [ProducesResponseType(typeof(UserProfileFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileFull>> UpdateUserProfile([FromBody] UpdateMyProfile userProfileRequest)
    {
        var userProfile = await userProfilesService.UpdateMyProfile(userProfileRequest);

        return StatusCode(StatusCodes.Status200OK, userProfile);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteUserProfile()
    {
        await userProfilesService.DeleteMyProfile();

        return StatusCode(StatusCodes.Status200OK);
    }
}
