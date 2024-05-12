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

    [HttpPost]
    [ProducesResponseType(typeof(MyArtworkFull), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MyArtworkFull>> CreateMyArtwork(
               [FromForm][Required] CreateArtwork request)
    {
        var myArtwork = await myArtworksService.CreateMyArtwork(request);

        return StatusCode(StatusCodes.Status201Created, myArtwork);
    }

    [HttpPut("{artworkId}")]
    [ProducesResponseType(typeof(MyArtworkFull), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MyArtworkFull>> UpdateMyArtwork(
        [FromRoute][Required] Guid artworkId,
        [FromForm][Required] UpdateArtwork request)
    {
        var myArtwork = await myArtworksService.UpdateMyArtwork(artworkId, request);

        return StatusCode(StatusCodes.Status200OK, myArtwork);
    }

    [HttpDelete("{artworkId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteMyArtwork(
               [FromRoute][Required] Guid artworkId)
    {
        await myArtworksService.DeleteMyArtwork(artworkId);

        return StatusCode(StatusCodes.Status204NoContent);
    }
}
