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
    public class TrackController : ControllerBase
    {
        private readonly ApplicationDb _db;
        private readonly IConfiguration _config;

        public TrackController(ApplicationDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<List<TrackDto>>> GetAll()
        {
            var tracks = await _db.Tracks
                .Select(x => new TrackDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    AlbumName = x.Album.Name,
                    ArtistNames = x.Album.Artists.Select(a => a.Artist.Name).ToList(),
                    ContentType = x.ContentType,
                    Contents = x.Contents
                }).ToListAsync();

            return tracks;
        }

        [HttpGet("get-one/{id}")]
        public async Task<ActionResult<TrackDto>> GetOne(int id)
        {
            var track = await _db.Tracks
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

            if (track == null)
            {
                return NotFound();
            }

            return track;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(IFormFile file, [FromQuery] TrackAddEditDto model)
        {
            var album = await _db.Albums.FindAsync(model.AlbumId);
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

            _db.Tracks.Add(trackToAdd);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOne), new { id = trackToAdd.Id }, null);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(IFormFile file, [FromQuery] TrackAddEditDto model)
        {
            var track = await _db.Tracks.FindAsync(model.Id);
            if (track == null) return NotFound();

            var album = await _db.Albums.FindAsync(model.AlbumId);
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

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fetchedTrack = await _db.Tracks.FindAsync(id);
            if (fetchedTrack == null) return NotFound();

            _db.Tracks.Remove(fetchedTrack);
            await _db.SaveChangesAsync();

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
