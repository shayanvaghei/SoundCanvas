using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class GenreController : ApiCoreController
    {
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll()
        {
            var genres = await UnitOfWork.GenreRepo.GetAllAsync();
            return Ok(Mapper.Map<IEnumerable<GenreDto>>(genres));
        }

        [HttpGet("get-one/{id}")]
        public async Task<ActionResult<GenreDto>> GetOne(int id)
        {
            var genre = await UnitOfWork.GenreRepo.GetFirstOrDefaultAsync(x => x.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<GenreDto>(genre));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(GenreAddEditDto model)
        {
            if (await GenreNameExistsAsync(model.Name))
            {
                return BadRequest("Genre name should be unique");
            }

            var genreToAdd = new Genre
            {
                Name = model.Name.ToLower()
            };

            UnitOfWork.GenreRepo.Add(genreToAdd);
            await UnitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(GenreAddEditDto model)
        {
            var fetchedGenre = await UnitOfWork.GenreRepo.GetFirstOrDefaultAsync(x => x.Id == model.Id);
            if (fetchedGenre == null)
            {
                return NotFound();
            }

            if (await GenreNameExistsAsync(model.Name))
            {
                return BadRequest("Genre name should be unique");
            }

            fetchedGenre.Name = model.Name.ToLower();
            await UnitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fetchedObj = await UnitOfWork.GenreRepo.GetFirstOrDefaultAsync(x => x.Id == id);
            if (fetchedObj == null) return NotFound();

            UnitOfWork.GenreRepo.Remove(fetchedObj);
            await UnitOfWork.CompleteAsync();

            return NoContent();
        }

        private async Task<bool> GenreNameExistsAsync(string name)
        {
            return await UnitOfWork.GenreRepo.AnyAsync(x => x.Name.ToLower() == name.ToLower());
        }
    }
}
