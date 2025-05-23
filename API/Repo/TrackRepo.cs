using API.Data;
using API.DTOs;
using API.Models;
using API.Repo.IRepo;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repo
{
    public class TrackRepo : BaseRepo<Track>, ITrackRepo
    {
        private readonly Context _context;

        public TrackRepo(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<List<TrackDto>> GetAllTracksAsync()
        {
            return await _context.Tracks
                .Select(x => new TrackDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    AlbumName = x.Album.Name,
                    ArtistNames = x.Album.Artists.Select(a => a.Artist.Name).ToList(),
                    ContentType = x.ContentType,
                    Contents = x.Contents
                }).ToListAsync();
        }

        public async Task<TrackDto> GetTrackByIdAsync(int id)
        {
            return await _context.Tracks
                .Where(x => x.Id == id)
                .Select(x => new TrackDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    AlbumName = x.Album.Name,
                    ArtistNames = x.Album.Artists.Select(a => a.Artist.Name).ToList(),
                    ContentType = x.ContentType,
                    Contents = x.Contents
                }).FirstOrDefaultAsync();
        }
    }
}
