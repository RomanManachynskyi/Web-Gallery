using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers;

[Authorize]
[ApiController]
[Tags("Artworks")]
[Route("api/v1/artworks")]
public sealed class ArtworksController : Controller
{
    private readonly IArtworksService artworksService;

    public ArtworksController(IArtworksService artworksService)
    {
        this.artworksService = artworksService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<List<ArtworksResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<List<ArtworksResponse>>>> GetArtworks(
        [FromQuery][Required] ArtworksRequest request)
    {
        var artworks = await artworksService.GetArtworks(request);

        return StatusCode(StatusCodes.Status200OK, artworks);
    }
}
