using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoGallery.Server.Data;
using PhotoGallery.Server.Models;
using System.Security.Claims;

namespace PhotoGallery.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlbumController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/album/GetAlbum/:id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbum(int id)
        {
            var album = await _context.Albums.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);
            
            if (album == null)
            {
                return NotFound("Album doesn't exist");
            }

            var albumWithImageCount = new
            {
                Id = album.Id,
                Name = album.Name,
                Description = album.Description,
                userId = album.UserId,
                ImageCount = album.Images.Count
            };

            return Ok(albumWithImageCount);
        }

        // GET: api/album/GetUserAlbums/:userId
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserAlbums(string userId)
        {
            var userAlbums = await _context.Albums.Include(a => a.Images)
                                                    .Where(a => a.UserId == userId)
                                                    .ToListAsync();
            
            foreach (var album in userAlbums)
            {
                album.Images = album.Images.Take(1).ToList();
                if (album.Images.Count > 0)
                {
                    album.Images[0].ImagePath = GetImageUrl(album.Images[0].ImageUniqueName);
                }
            }

            return Ok(userAlbums);
        }

        // GET: api/album/GetImages/:albumId?pageNumber?pageSize
        [HttpGet("{albumId}")]
        public async Task<IActionResult> GetImages(int albumId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var album = await _context.Albums.Include(a => a.Images)
                                              .FirstOrDefaultAsync(a => a.Id == albumId);

            if (album == null)
            {
                return NotFound("Album doesn't exist");
            }

            var images = album.Images.Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToList();

            foreach(ImageModel image in images)
            {
                image.ImagePath = GetImageUrl(image.ImageUniqueName);
            }

            return Ok(images);
        }

        // POST: api/album/Create
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostAlbumModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            AlbumModel newAlbum = new AlbumModel
            {
                Name = model.albumName,
                Description = model.description,
                UserId = userId,
                Images = new List<ImageModel>()
            };

            await _context.Albums.AddAsync(newAlbum);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: api/album/GetAlbums
        [HttpGet]
        public async Task<IActionResult> GetAlbums()
        {
            var albums = await _context.Albums.Include(a => a.Images).ToListAsync();

            foreach(var album in albums)
            {
                album.Images = album.Images.Take(1).ToList();
                if(album.Images.Count > 0)
                {
                    album.Images[0].ImagePath = GetImageUrl(album.Images[0].ImageUniqueName);
                }                
            }

            return Ok(albums);
        }

        // DELETE: api/album/Delete/:id
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var albumModel = await _context.Albums.FindAsync(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (albumModel != null && albumModel.UserId == userId)
            {
                _context.Albums.Remove(albumModel);
            }
            else
            {
                return BadRequest("Can't delete specific album");
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        private string GetImageUrl(string imageName)
        {
            return $"/uploads/{imageName}";
        }
    }
}
