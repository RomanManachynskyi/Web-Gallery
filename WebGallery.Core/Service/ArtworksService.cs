using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IArtworksService
{
    Task<List<ArtworksResponse>> GetArtworks(ArtworksRequest artworksRequest);
}

public sealed class ArtworksService : IArtworksService
{
    private readonly IMapper mapper;
    private readonly IRepository<Artwork> artworkRepository;

    public ArtworksService(
        IMapper mapper,
        IRepository<Artwork> artworkRepository)
    {
        this.mapper = mapper;
        this.artworkRepository = artworkRepository;
    }

    #region Implementation

    public async Task<List<ArtworksResponse>> GetArtworks(ArtworksRequest artworksRequest)
    {
        var artworks = await artworkRepository.ListAsync(new ListArtworksSpecification(artworksRequest));

        var result = mapper.Map<List<ArtworksResponse>>(artworks);

        return result;
    }

    #endregion
}
