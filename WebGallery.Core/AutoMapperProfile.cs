﻿using AutoMapper;
using WebGallery.Core.Dtos;
using WebGallery.Data.Entities;

namespace WebGallery.Core;

public sealed class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserProfile, UserProfileGeneral>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl));
        CreateMap<CreateUserProfile, UserProfile>(MemberList.Destination);
        CreateMap<UserProfile,   UserProfileFull>(MemberList.Destination);

        CreateMap<Artwork, ArtworksGeneral>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.CompressedFrontPictureUrl))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.PublishedAt));

        CreateMap<ArtworkTag, ArtworkTagsGeneral>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<Artwork, ArtworksResponse>()
            .ForMember(dest => dest.UserProfile, opt => opt.MapFrom(src => src.UserProfile))
            .ForMember(dest => dest.Artwork, opt => opt.MapFrom(src => src));
    }
}
