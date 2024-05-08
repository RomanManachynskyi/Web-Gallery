using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Exceptions;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IArtworksService
{
    Task<List<ArtworksResponse>> GetArtworks(ArtworksRequest artworksRequest);
    Task<ArtworkResponse> GetArtwork(Guid artworkId);
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

    public async Task<ArtworkResponse> GetArtwork(Guid artworkId)
    {
        var artwork = await artworkRepository.FirstOrDefaultAsync(new GetArtworkByIdWithDependenciesSpecification(artworkId))
                ?? throw new NotFoundException("Artwork not found");

        artwork.TotalViews++;
        await artworkRepository.SaveChangesAsync();

        var result = mapper.Map<ArtworkResponse>(artwork);

        return result;
    }

    #endregion
}
