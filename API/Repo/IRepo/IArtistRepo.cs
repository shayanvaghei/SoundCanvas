using API.DTOs;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repo.IRepo
{
    public interface IArtistRepo : IBaseRepo<Artist>
    {
        Task<List<ArtistDto>> GetAllArtistsAsync();
        Task<ArtistDto> GetArtistByIdAsync(int id);
    }
}
