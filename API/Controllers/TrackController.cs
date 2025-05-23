using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : ApiCoreController
    {
        private readonly IConfiguration _config;

        public TrackController(Context db, IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<List<TrackDto>>> GetAll()
        {
            return Ok(await  UnitOfWork.TrackRepo.GetAllTracksAsync());
        }

        [HttpGet("get-one/{id}")]
        public async Task<ActionResult<TrackDto>> GetOne(int id)
        {
            var track = await UnitOfWork.TrackRepo.GetTrackByIdAsync(id);

            if (track == null)
            {
                return NotFound();
            }

            return track;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(IFormFile file, [FromQuery] TrackAddEditDto model)
        {
            var album = await UnitOfWork.AlbumRepo.GetFirstOrDefaultAsync(x => x.Id == model.AlbumId);
            if (album == null) return BadRequest("Invalid albumId");

            if (file == null || file.Length == 0)
            {
                return BadRequest("Please choose a file");
            }

            var fileMaxAllowedSize = int.Parse(_config["File:MaxAllowedSize"]);
            if (file.Length > fileMaxAllowedSize)
            {
                return BadRequest(string.Format("File is too large, it cannot be more than {0} MB", fileMaxAllowedSize / 1000000));
            }

            if (!IsAcceptableContentType(file.ContentType))
            {
                return BadRequest(string.Format("Invalid content type. It must be one of the following {0}", string.Join(", ", GetAcceptableContentTypes())));
            }

            var trackToAdd = new Track
            {
                Name = model.Name,
                AlbumId = model.AlbumId,
                ContentType = file.ContentType,
                Contents = GetFileContents(file)
            };

            UnitOfWork.TrackRepo.Add(trackToAdd);
            await UnitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetOne), new { id = trackToAdd.Id }, null);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(IFormFile file, [FromQuery] TrackAddEditDto model)
        {
            var track = await UnitOfWork.TrackRepo.GetFirstOrDefaultAsync(x => x.Id == model.Id);
            if (track == null) return NotFound();

            var album = await UnitOfWork.AlbumRepo.GetFirstOrDefaultAsync(x => x.Id == model.AlbumId);
            if (album == null) return BadRequest("Invalid albumId");

            if (file != null && file.Length > 0)
            {
                var fileMaxAllowedSize = int.Parse(_config["File:MaxAllowedSize"]);
                if (file.Length > fileMaxAllowedSize)
                {
                    return BadRequest(string.Format("File is too large, it cannot be more than {0} MB", fileMaxAllowedSize / 1000000));
                }

                if (!IsAcceptableContentType(file.ContentType))
                {
                    return BadRequest(string.Format("Invalid content type. It must be one of the following {0}", string.Join(", ", GetAcceptableContentTypes())));
                }

                track.Contents = GetFileContents(file);
                track.ContentType = file.ContentType;
            }

            track.Name = model.Name;
            track.AlbumId = model.AlbumId;

            await UnitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fetchedTrack = await UnitOfWork.TrackRepo.GetFirstOrDefaultAsync(x => x.Id == id);
            if (fetchedTrack == null) return NotFound();

            UnitOfWork.TrackRepo.Remove(fetchedTrack);
            await UnitOfWork.CompleteAsync();

            return NoContent();
        }

        private string[] GetAcceptableContentTypes()
        {
            return _config.GetSection("File:TrackAcceptableContentType").Get<string[]>();
        }

        private bool IsAcceptableContentType(string contentType)
        {
            var allowedTypes = GetAcceptableContentTypes();

            foreach(var type in allowedTypes)
            {
                if (contentType.ToLower().Equals(type.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        private byte[] GetFileContents(IFormFile file)
        {
            byte[] contents;
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            contents = memoryStream.ToArray();

            return contents;
        }
    }
}
