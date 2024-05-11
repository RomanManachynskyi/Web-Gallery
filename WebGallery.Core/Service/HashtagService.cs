using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Core.Service.Specification;
using WebGallery.Data.Entities;
using WebGallery.Data.Repositories;

namespace WebGallery.Core.Service;

public interface IHashtagService
{
    Task<List<HashtagFull>> GetHashtags(HashtagRequest hashtagsRequest);
}

public sealed class HashtagService : IHashtagService
{
    private readonly IMapper mapper;
    private readonly IRepository<Hashtag> hashtagRepository;

    public HashtagService(
        IMapper mapper,
        IRepository<Hashtag> artworkRepository)
    {
        this.mapper = mapper;
        this.hashtagRepository = artworkRepository;
    }

    #region Implementation

    public async Task<List<HashtagFull>> GetHashtags(HashtagRequest hashtagsRequest)
    {
        var hashtags = await hashtagRepository.ListAsync(new ListHashtagSpecification(hashtagsRequest));

        var result = mapper.Map<List<HashtagFull>>(hashtags);

        return result;
    }

    #endregion
}
