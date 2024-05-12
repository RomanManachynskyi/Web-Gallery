using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Data.Entities;

namespace WebGallery.Core;

public sealed class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserProfile, UserProfileGeneral>(MemberList.Destination);
        CreateMap<UserProfile, UserProfileFull>(MemberList.Destination);
        CreateMap<CreateMyProfile, UserProfile>(MemberList.Destination);
        CreateMap<UpdateMyProfile, UserProfile>(MemberList.Destination);

        CreateMap<Artwork, ArtworksGeneral>(MemberList.Destination)
            .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.CompressedFrontPictureUrl));
        CreateMap<Artwork, MyArtworkGeneral>(MemberList.Destination)
            .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.CompressedFrontPictureUrl));
        CreateMap<Artwork, MyArtworkFull>(MemberList.Destination);

        CreateMap<Picture, PictureResponse>(MemberList.Destination);
        CreateMap<Hashtag, HashtagsGeneral>(MemberList.Destination);
        CreateMap<Hashtag, HashtagFull>(MemberList.Destination);

        CreateMap<Artwork, ArtworksResponse>(MemberList.Destination)
            .ForMember(dest => dest.Artwork, opt => opt.MapFrom(src => src));
        CreateMap<Artwork, ArtworkResponse>(MemberList.Destination);
        CreateMap<Like, ArtworksResponse>(MemberList.Destination)
            .ForMember(dest => dest.UserProfile, opt => opt.MapFrom(src => src.Artwork.UserProfile));
        CreateMap<Bookmark, ArtworksResponse>(MemberList.Destination)
            .ForMember(dest => dest.UserProfile, opt => opt.MapFrom(src => src.Artwork.UserProfile));
    }
}
