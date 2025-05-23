using API.DTOs;
using API.Models;
using AutoMapper;

namespace API.Helpers
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            // d -> dest, o -> option, s -> source
            // m.CreateMap<Source, Destination>();

            var mappingConfig = new MapperConfiguration(m =>
            {
                m.CreateMap<Genre, GenreDto>();
                m.CreateMap<Artist, ArtistDto>().ForMember(d => d.Genre, o => o.MapFrom(s => s.Genre.Name));
            });

            return mappingConfig;
        }
    }
}
