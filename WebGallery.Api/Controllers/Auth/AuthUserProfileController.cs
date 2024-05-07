using Microsoft.AspNetCore.Mvc;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers.Auth;

[ApiController]
[Tags("User Profiles")]
[Route("api/v1/auth/user-profile")]
public sealed class AuthUserProfileController : Controller
{
    public readonly IUserProfilesService userProfilesService;

    public AuthUserProfileController(IUserProfilesService userProfilesService)
    {
        this.userProfilesService = userProfilesService;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserProfileFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileFull>> GetUserProfileById([FromRoute] Guid userId)
    {
        var userProfile = await userProfilesService.GetUserProfileById(userId);

        return StatusCode(StatusCodes.Status200OK, userProfile);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserProfileFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileFull>> CreateUserProfile([FromBody] CreateUserProfile userProfileRequest)
    {
        var userProfile = await userProfilesService.CreateUserProfile(userProfileRequest);

        return StatusCode(StatusCodes.Status200OK, userProfile);
    }

    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(UserProfileFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileFull>> UpdateUserProfile(
        [FromRoute] Guid userId,
        [FromBody] UpdateUserProfile userProfileRequest)
    {
        var userProfile = await userProfilesService.UpdateUserProfile(userId, userProfileRequest);

        return StatusCode(StatusCodes.Status200OK, userProfile);
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteUserProfile([FromRoute] Guid userId)
    {
        await userProfilesService.DeleteUserProfile(userId);

        return StatusCode(StatusCodes.Status200OK);
    }
}
