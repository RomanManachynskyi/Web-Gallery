using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers;

[Authorize]
[ApiController]
[Tags("Hashtags")]
[Route("api/v1/hashtags")]
public class HashtagsController : Controller
{
    public readonly IHashtagService hashtagService;

    public HashtagsController(IHashtagService hashtagService)
    {
        this.hashtagService = hashtagService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<HashtagFull>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<HashtagFull>>> GetHashtags(
        [FromQuery][Required] HashtagRequest request)
    {
        var hashtags = await hashtagService.GetHashtags(request);
    
        return StatusCode(StatusCodes.Status200OK, hashtags);
    }
}
