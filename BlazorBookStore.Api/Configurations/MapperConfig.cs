using AutoMapper;
using BlazorBookStore.Api.Data;
using BlazorBookStore.Api.Models.Author;

namespace BlazorBookStore.Api.Configurations
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<AuthorCreateDto, Author>().ReverseMap();
            CreateMap<AuthorUpdateDto, Author>().ReverseMap();
            CreateMap<AuthorReadOnlyDto, Author>().ReverseMap();
        }
    }
}