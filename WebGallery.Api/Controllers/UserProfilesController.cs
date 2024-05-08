using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers;

[Authorize]
[ApiController]
[Tags("User Profiles")]
[Route("api/v1/user-profiles")]
public class UserProfilesController : Controller
{
    public readonly IUserProfilesService userProfilesService;

    public UserProfilesController(IUserProfilesService userProfilesService)
    {
        this.userProfilesService = userProfilesService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(UserProfileGeneral), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileGeneral>> GetUserProfileById(
        [FromQuery] UserProfilesRequest profilesRequest)
    {
        var userProfile = await userProfilesService.GetUserProfiles(profilesRequest);

        return StatusCode(StatusCodes.Status200OK, userProfile);
    }

    [HttpGet("{userProfileId}")]
    [ProducesResponseType(typeof(UserProfileFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileFull>> GetUserProfileById([FromRoute] Guid userProfileId)
    {
        var userProfile = await userProfilesService.GetUserProfile(userProfileId);

        return StatusCode(StatusCodes.Status200OK, userProfile);
    }
}
