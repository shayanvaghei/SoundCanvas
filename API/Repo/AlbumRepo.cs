using API.Data;
using API.DTOs;
using API.Models;
using API.Repo.IRepo;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repo
{
    public class AlbumRepo : BaseRepo<Album>, IAlbumRepo
    {
        private readonly Context _context;

        public AlbumRepo(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AlbumDto>> GetAlbumsAsync()
        {
            return await _context.Albums
                .Select(x => new AlbumDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhotoUrl = x.PhotoUrl,
                    Artists = x.Artists.Select(a => new ArtistDto
                    {
                        Id = a.Artist.Id,
                        Name = a.Artist.Name,
                        PhotoUrl = a.Artist.PhotoUrl,
                        Genre = a.Artist.Genre.Name
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<AlbumDto> GetAlbymByIdAsync(int id)
        {
            return await _context.Albums
                .Where(x => x.Id == id)
                .Select(x => new AlbumDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhotoUrl = x.PhotoUrl,
                    Artists = x.Artists.Select(a => new ArtistDto
                    {
                        Id = a.Artist.Id,
                        Name = a.Artist.Name,
                        PhotoUrl = a.Artist.PhotoUrl,
                        Genre = a.Artist.Genre.Name
                    }).ToList(),
                    TrackNames = x.Tracks.Select(t => t.Name).ToList()
                }).FirstOrDefaultAsync();
        }
    }
}
