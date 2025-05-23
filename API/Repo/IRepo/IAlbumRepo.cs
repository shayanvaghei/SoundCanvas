using API.DTOs;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repo.IRepo
{
    public interface IAlbumRepo : IBaseRepo<Album>
    {
        Task<List<AlbumDto>> GetAlbumsAsync();
        Task<AlbumDto> GetAlbymByIdAsync(int id); 
    }
}
