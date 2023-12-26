using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly ApplicationDb _db;

        public ArtistController(ApplicationDb db)
        {
            _db = db;
        }

        [HttpGet("get-all")]
        public ActionResult<List<ArtistDto>> GetAll()
        {
            var artists = _db.Artists
                .Select(x => new ArtistDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhotoUrl = x.PhotoUrl,
                    Genre = x.Genre.Name
                })
                .ToList();

            return artists;
        }

        [HttpGet("get-one/{id}")]
        public ActionResult<ArtistDto> GetOne(int id)
        {
            var artist = _db.Artists
                .Where(x => x.Id == id)
                .Select(x => new ArtistDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhotoUrl = x.PhotoUrl,
                    Genre = x.Genre.Name
                }).FirstOrDefault();

            if (artist == null)
            {
                return NotFound();
            }

            return artist;
        }

        [HttpPost("create")]
        public IActionResult Create(ArtistAddEditDto model)
        {
            if (ArtisNameExists(model.Name))
            {
                return BadRequest("Artist name should be unique");
            }

            var fetchedGenre = GetGenreByName(model.Genre);
            if (fetchedGenre == null)
            {
                return BadRequest("Invalid genre name");
            }

            var artistToAdd = new Artist
            {
                Name = model.Name.ToLower(),
                //Genre = fetchedGenre,
                GenreId = fetchedGenre.Id,
                PhotoUrl = model.PhotoUrl,
            };

            _db.Artists.Add(artistToAdd);
            _db.SaveChanges();

            //return NoContent();
            return CreatedAtAction(nameof(GetOne), new { id = artistToAdd.Id }, null);
        }

        [HttpPut("update")]
        public IActionResult Update(ArtistAddEditDto model)
        {
            var fetchedArtist = _db.Artists.Find(model.Id);
            if (fetchedArtist == null)
            {
                return NotFound();
            }

            if (fetchedArtist.Name != model.Name.ToLower() && ArtisNameExists(model.Name))
            {
                return BadRequest("Artist name should be unique");
            }

            var fetchedGenre = GetGenreByName(model.Genre);
            if (fetchedGenre == null)
            {
                return BadRequest("Invalid genre name");
            }

            // updeting the record here
            fetchedArtist.Name = model.Name.ToLower();
            fetchedArtist.Genre = fetchedGenre;
            fetchedArtist.PhotoUrl = model.PhotoUrl;
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var fetchedArtist = _db.Artists.Find(id);
            if (fetchedArtist == null)
            {
                return NotFound();
            }

            _db.Artists.Remove(fetchedArtist);
            _db.SaveChanges();

            return NoContent();
        }

        private bool ArtisNameExists(string name)
        {
            return _db.Artists.Any(x => x.Name.ToLower() == name.ToLower());
        }

        private Genre GetGenreByName(string name)
        {
            return _db.Genres.SingleOrDefault(x => x.Name.ToLower() == name.ToLower());
        }
    }
}
