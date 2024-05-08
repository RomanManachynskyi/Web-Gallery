using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers;

[Authorize]
[ApiController]
[Tags("Bookmarks")]
[Route("api/v1/user-profiles/my-profile/bookmarks")]
public class BookmarksController : Controller
{
    public readonly IBookmarksService bookmarksService;

    public BookmarksController(IBookmarksService bookmarksService)
    {
        this.bookmarksService = bookmarksService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ArtworksResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ArtworksResponse>> GetBookmarks([FromQuery] BookmarksRequest bookmarksRequest)
    {
        var bookmarks = await bookmarksService.GetBookmarks(bookmarksRequest);

        return StatusCode(StatusCodes.Status200OK, bookmarks);
    }

    [HttpPost("{artworkId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> BookmarkArtwork(Guid artworkId)
    {
        await bookmarksService.BookmarkArtwork(artworkId);

        return StatusCode(StatusCodes.Status201Created);
    }
}
