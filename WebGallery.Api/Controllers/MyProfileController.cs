using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers;

[Authorize]
[ApiController]
[Tags("User Profiles")]
[Route("api/v1/user-profiles/my-profile")]
public class MyProfileController : Controller
{
    public readonly IMyProfileService userProfilesService;

    public MyProfileController(IMyProfileService userProfilesService)
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
