using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PhotoGallery.Server.Data;
using PhotoGallery.Server.Models;
using System.Security.Claims;

namespace PhotoGallery.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostEnvironment _env;

        public ImageController(ApplicationDbContext context, 
                                UserManager<IdentityUser> userManager, 
                                IHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // POST: api/image/AddImageToAlbum
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddImageToAlbum([FromForm] int albumId, IFormFile image)
        {
            if (image == null)
            {
                return BadRequest("Image are required");
            }

            var album = await _context.Albums.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == albumId);

            if (album == null)
            {
                return BadRequest("Album not found");
            }

            try
            {
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                ImageModel newImage = new ImageModel
                {
                    ImageName = image.FileName,
                    ImageUniqueName = uniqueFileName,
                    ImagePath = filePath,
                    AlbumId = albumId,
                    Likes = 0,
                    Dislikes = 0
                };

                album.Images.Add(newImage);

                _context.Albums.Update(album);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/image/Delete/:imageId
        [Authorize]
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(int imageId)
        {
            var image = await _context.Images.Include(i => i.Album).FirstOrDefaultAsync(i => i.Id == imageId);
            if (image == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userId);

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin && image.Album.UserId != userId)
            {
                return Forbid("Only creators can delete own albums");
            }

            string filePath = image.ImagePath;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/image/Like/:imageId
        [Authorize]
        [HttpPost("{imageId}")]
        public async Task<IActionResult> Like(int imageId)
        {
            var image = await _context.Images.FindAsync(imageId);
            if (image == null)
            {
                return NotFound("Image doesn't exist");
            }

            image.Likes++;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/image/Dislike/:imageId
        [Authorize]
        [HttpPost("{imageId}")]
        public async Task<IActionResult> Dislike(int imageId)
        {
            var image = await _context.Images.FindAsync(imageId);
            if (image == null)
            {
                return NotFound("Image doesn't exist");
            }

            image.Dislikes++;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
