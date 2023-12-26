using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDb _db;

        public GenreController(ApplicationDb db)
        {
            _db = db;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var genres = _db.Genres.ToList();
            var toReturn = new List<GenreDto>();

            foreach (var genre in genres)
            {
                var genreDto = new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name,
                };

                toReturn.Add(genreDto);
            }


            return Ok(toReturn);
        }

        [HttpGet("get-one/{id}")]
        public IActionResult GetOne(int id)
        {
            var genre = _db.Genres.Find(id);
            if (genre == null)
            {
                return NotFound();
            }

            var toReturn = new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name,
            };

            return Ok(toReturn);
        }

        [HttpPost("create")]
        public IActionResult Create(GenreAddEditDto model)
        {
            if (GenreNameExists(model.Name))
            {
                return BadRequest("Genre name should be unique");
            }

            var genreToAdd = new Genre
            {
                Name = model.Name.ToLower()
            };

            _db.Genres.Add(genreToAdd);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("update")]
        public IActionResult Update(GenreAddEditDto model)
        {
            var fetchedGenre = _db.Genres.Find(model.Id);
            if (fetchedGenre == null)
            {
                return NotFound();
            }

            if (GenreNameExists(model.Name))
            {
                return BadRequest("Genre name should be unique");
            }

            fetchedGenre.Name = model.Name.ToLower();
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var fetchedObj = _db.Genres.Find(id);
            if (fetchedObj == null) return NotFound();

            _db.Genres.Remove(fetchedObj);
            _db.SaveChanges();

            return NoContent();
        }

        private bool GenreNameExists(string name)
        {
            var fetchedGenre = _db.Genres.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

            if (fetchedGenre != null)
            {
                return true;
            }

            return false;
        }
    }
}
