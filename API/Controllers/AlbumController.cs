using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly ApplicationDb _db;

        public AlbumController(ApplicationDb db)
        {
            _db = db;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<List<AlbumDto>>> GetAll()
        {
            var albums = await _db.Albums
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

            return albums;
        }

        [HttpGet("get-one/{id}")]
        public async Task<ActionResult<AlbumDto>> GetOne(int id)
        {
            var album = await _db.Albums
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
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (album == null) return NotFound();

            return album;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(AlbumAddEditDto model)
        {
            if (AlbumNameExistsAsyn(model.Name).GetAwaiter().GetResult())
            {
                return BadRequest("Album name should be unique");
            }

            if (model.ArtistIds.Count == 0)
            {
                return BadRequest("At least one artist id should be seleceted");
            }

            var albumToAdd = new Album
            {
                Name = model.Name,
                PhotoUrl = model.PhotoUrl
            };
            _db.Albums.Add(albumToAdd);
            await _db.SaveChangesAsync();

            await AssignArtistsToAlbumAsync(albumToAdd.Id, model.ArtistIds);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(AlbumAddEditDto model)
        {
            var fetchedAlbum = await _db.Albums.Include(x => x.Artists).FirstOrDefaultAsync(x => x.Id == model.Id);
            if (fetchedAlbum == null) return NotFound();

            if (fetchedAlbum.Name != model.Name.ToLower() && await AlbumNameExistsAsyn(model.Name))
            {
                return BadRequest("Album name should be unique");
            }

            // clear all existing Artists
            foreach(var artist in fetchedAlbum.Artists)
            {
                var fetchedArtistAlbumBridge = await _db.ArtistAlbumBridge
                    .SingleOrDefaultAsync(x => x.ArtistId == artist.ArtistId && x.AlbumId == fetchedAlbum.Id);
                _db.ArtistAlbumBridge.Remove(fetchedArtistAlbumBridge);
            }

            await _db.SaveChangesAsync();

            fetchedAlbum.Name = model.Name.ToLower();
            fetchedAlbum.PhotoUrl = model.PhotoUrl;

            await AssignArtistsToAlbumAsync(fetchedAlbum.Id, model.ArtistIds);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fetchedAlbum = await _db.Albums.Include(x => x.Artists).FirstOrDefaultAsync(x => x.Id == id);
            if (fetchedAlbum == null) return NotFound();

            // clear all existing Artists
            foreach (var artist in fetchedAlbum.Artists)
            {
                var fetchedArtistAlbumBridge = await _db.ArtistAlbumBridge
                    .SingleOrDefaultAsync(x => x.ArtistId == artist.ArtistId && x.AlbumId == fetchedAlbum.Id);
                _db.ArtistAlbumBridge.Remove(fetchedArtistAlbumBridge);
            }

            await _db.SaveChangesAsync();

            _db.Albums.Remove(fetchedAlbum);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> AlbumNameExistsAsyn(string albumName)
        {
            return await _db.Albums.AnyAsync(x => x.Name == albumName.ToLower());
        }

        private async Task AssignArtistsToAlbumAsync(int albumId, List<int> artistIds)
        {
            // removing any duplicate artistsIds
            artistIds = artistIds.Distinct().ToList();

            foreach (var artistId in artistIds)
            {
                var artist = await _db.Artists.FindAsync(artistId);
                if (artist != null)
                {
                    var artistAlbumBridgeToAdd = new ArtistAlbumBridge
                    {
                        AlbumId = albumId,
                        ArtistId = artistId
                    };

                    _db.ArtistAlbumBridge.Add(artistAlbumBridgeToAdd);
                }
            }
        }
    }
}
