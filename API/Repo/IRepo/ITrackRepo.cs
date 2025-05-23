using API.DTOs;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repo.IRepo
{
    public interface ITrackRepo : IBaseRepo<Track>
    {
        Task<List<TrackDto>> GetAllTracksAsync();
        Task<TrackDto> GetTrackByIdAsync(int id);
    }
}
