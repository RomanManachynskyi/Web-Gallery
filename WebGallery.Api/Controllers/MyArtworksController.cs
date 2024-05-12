using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers;

[Authorize]
[ApiController]
[Tags("My Artworks")]
[Route("api/v1/artworks/my-artworks")]
public class MyArtworksController : Controller
{
    private readonly IMyArtworksService myArtworksService;

    public MyArtworksController(IMyArtworksService myArtworksService)
    {
        this.myArtworksService = myArtworksService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MyArtworkGeneral>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<MyArtworkGeneral>>> GetMyArtworks(
        [FromQuery][Required] GetMyArtworks request)
    {
        var myArtworks = await myArtworksService.GetMyArtworks(request);

        return StatusCode(StatusCodes.Status200OK, myArtworks);
    }

    [HttpGet("{artworkId}")]
    [ProducesResponseType(typeof(MyArtworkFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MyArtworkFull>> GetMyArtwork(
        [FromRoute][Required] Guid artworkId)
    {
        var myArtwork = await myArtworksService.GetMyArtwork(artworkId);

        return StatusCode(StatusCodes.Status200OK, myArtwork);
    }
}
