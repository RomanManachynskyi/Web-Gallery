﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service;

namespace WebGallery.Api.Controllers;

[Authorize]
[ApiController]
[Tags("Artworks")]
[Route("api/v1/artworks/liked")]
public class LikesController : Controller
{
    public readonly ILikesService likesService;

    public LikesController(ILikesService likesService)
    {
        this.likesService = likesService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ArtworksResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ArtworksResponse>> GetLikes([FromQuery] LikesRequest likesRequest)
    {
        var likes = await likesService.GetLikes(likesRequest);

        return StatusCode(StatusCodes.Status200OK, likes);
    }
}
