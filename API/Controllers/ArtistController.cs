using API.Data;
using API.DTOs;
using API.Models;
using API.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class ArtistController : ApiCoreController
    {
        [HttpGet("get-all")]
        public async Task<ActionResult<List<ArtistDto>>> GetAll()
        {
            return Ok(await UnitOfWork.ArtistRepo.GetAllArtistsAsync());
        }

        [HttpGet("get-one/{id}")]
        public async Task<ActionResult<ArtistDto>> GetOne(int id)
        {
            var artist = await UnitOfWork.ArtistRepo.GetArtistByIdAsync(id);

            if (artist == null)
            {
                return NotFound();
            }

            return artist;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(ArtistAddEditDto model)
        {
            if (await ArtisNameExistsAsync(model.Name))
            {
                return BadRequest("Artist name should be unique");
            }

            var fetchedGenre = await GetGenreByNameAsync(model.Genre);
            if (fetchedGenre == null)
            {
                return BadRequest("Invalid genre name");
            }

            var artistToAdd = new Artist
            {
                Name = model.Name.ToLower(),
                GenreId = fetchedGenre.Id,
                PhotoUrl = model.PhotoUrl,
            };

            UnitOfWork.ArtistRepo.Add(artistToAdd);
            await UnitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetOne), new { id = artistToAdd.Id }, null);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(ArtistAddEditDto model)
        {
            var fetchedArtist = await UnitOfWork.ArtistRepo.GetFirstOrDefaultAsync(x => x.Id == model.Id);
            if (fetchedArtist == null)
            {
                return NotFound();
            }

            if (fetchedArtist.Name != model.Name.ToLower() && ArtisNameExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                return BadRequest("Artist name should be unique");
            }

            var fetchedGenre = await GetGenreByNameAsync(model.Genre);
            if (fetchedGenre == null)
            {
                return BadRequest("Invalid genre name");
            }

            // updeting the record here
            fetchedArtist.Name = model.Name.ToLower();
            fetchedArtist.Genre = fetchedGenre;
            fetchedArtist.PhotoUrl = model.PhotoUrl;
            await UnitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fetchedArtist = await UnitOfWork.ArtistRepo.GetFirstOrDefaultAsync(x => x.Id == id);
            if (fetchedArtist == null)
            {
                return NotFound();
            }

            UnitOfWork.ArtistRepo.Remove(fetchedArtist);
            await UnitOfWork.CompleteAsync();

            return NoContent();
        }

        private async Task<bool> ArtisNameExistsAsync(string name)
        {
            return await UnitOfWork.ArtistRepo.AnyAsync(x => x.Name.ToLower() == name.ToLower());
        }

        private async Task<Genre> GetGenreByNameAsync(string name)
        {
            return await UnitOfWork.GenreRepo.GetFirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }
    }
}
