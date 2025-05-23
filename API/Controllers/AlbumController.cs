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
    public class AlbumController : ApiCoreController
    {
        [HttpGet("get-all")]
        public async Task<ActionResult<List<AlbumDto>>> GetAll()
        {
            return Ok(await UnitOfWork.AlbumRepo.GetAlbumsAsync());
        }

        [HttpGet("get-one/{id}")]
        public async Task<ActionResult<AlbumDto>> GetOne(int id)
        {
            var album = await UnitOfWork.AlbumRepo.GetAlbymByIdAsync(id);
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

            UnitOfWork.AlbumRepo.Add(albumToAdd);
            await UnitOfWork.CompleteAsync();

            await AssignArtistsToAlbumAsync(albumToAdd.Id, model.ArtistIds);
            await UnitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(AlbumAddEditDto model)
        {
            var fetchedAlbum = await UnitOfWork.AlbumRepo.GetFirstOrDefaultAsync(x => x.Id == model.Id, includeProperties: "Artists");
            if (fetchedAlbum == null) return NotFound();

            if (fetchedAlbum.Name != model.Name.ToLower() && await AlbumNameExistsAsyn(model.Name))
            {
                return BadRequest("Album name should be unique");
            }

            // clear all existing Artists
            foreach (var artist in fetchedAlbum.Artists)
            {
                var fetchedArtistAlbumBridge = await UnitOfWork.ArtistAlbumBridgeRepo
                    .GetFirstOrDefaultAsync(x => x.ArtistId == artist.ArtistId && x.AlbumId == fetchedAlbum.Id);
                UnitOfWork.ArtistAlbumBridgeRepo.Remove(fetchedArtistAlbumBridge);
            }

            await UnitOfWork.CompleteAsync();

            fetchedAlbum.Name = model.Name.ToLower();
            fetchedAlbum.PhotoUrl = model.PhotoUrl;

            await AssignArtistsToAlbumAsync(fetchedAlbum.Id, model.ArtistIds);
            await UnitOfWork.CompleteAsync(); ;

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fetchedAlbum = await UnitOfWork.AlbumRepo.GetFirstOrDefaultAsync(x => x.Id == id, includeProperties: "Artists");
            if (fetchedAlbum == null) return NotFound();

            // clear all existing Artists
            foreach (var artist in fetchedAlbum.Artists)
            {
                var fetchedArtistAlbumBridge = await UnitOfWork.ArtistAlbumBridgeRepo
                     .GetFirstOrDefaultAsync(x => x.ArtistId == artist.ArtistId && x.AlbumId == fetchedAlbum.Id);
                UnitOfWork.ArtistAlbumBridgeRepo.Remove(fetchedArtistAlbumBridge);
            }

            await UnitOfWork.CompleteAsync();

            UnitOfWork.AlbumRepo.Remove(fetchedAlbum);
            await UnitOfWork.CompleteAsync();

            return NoContent();
        }

        private async Task<bool> AlbumNameExistsAsyn(string albumName)
        {
            return await UnitOfWork.AlbumRepo.AnyAsync(x => x.Name == albumName.ToLower());
        }

        private async Task AssignArtistsToAlbumAsync(int albumId, List<int> artistIds)
        {
            // removing any duplicate artistsIds
            artistIds = artistIds.Distinct().ToList();

            foreach (var artistId in artistIds)
            {
                var artist = await UnitOfWork.ArtistRepo.GetFirstOrDefaultAsync(x => x.Id == artistId);
                if (artist != null)
                {
                    var artistAlbumBridgeToAdd = new ArtistAlbumBridge
                    {
                        AlbumId = albumId,
                        ArtistId = artistId
                    };

                    UnitOfWork.ArtistAlbumBridgeRepo.Add(artistAlbumBridgeToAdd);
                }
            }
        }
    }
}
