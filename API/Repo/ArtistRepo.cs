using API.Data;
using API.DTOs;
using API.Models;
using API.Repo.IRepo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repo
{
    public class ArtistRepo : BaseRepo<Artist>, IArtistRepo
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public ArtistRepo(Context context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ArtistDto>> GetAllArtistsAsync()
        {
            var artists = await GetAllAsync(includeProperties: "Genre");
            return _mapper.Map<List<ArtistDto>>(artists);
        }

        public async Task<ArtistDto> GetArtistByIdAsync(int id)
        {
            return await _context.Artists
               .Where(x => x.Id == id)
               .Select(x => new ArtistDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   PhotoUrl = x.PhotoUrl,
                   Genre = x.Genre.Name,
                   AlbumNames = x.Albums.Select(a => a.Album.Name).ToList()
               }).FirstOrDefaultAsync();
        }
    }
}
