using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Moq;
using PhotoGallery.Server.Controllers;
using PhotoGallery.Server.Models;
using PhotoGallery.Server.Data;

[TestClass]
public class ImageControllerTests
{
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly ApplicationDbContext _context;
    private readonly Mock<IHostEnvironment> _mockHostEnvironment;

    public ImageControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options);

        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            new Mock<IUserStore<IdentityUser>>().Object,
            null, null, null, null, null, null, null, null);

        _mockHostEnvironment = new Mock<IHostEnvironment>();
        _mockHostEnvironment.Setup(m => m.ContentRootPath).Returns(Directory.GetCurrentDirectory());

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var user = new IdentityUser { UserName = "testuser"};
        _context.Users.Add(user);

        var user1 = new IdentityUser { UserName = "testuser1" };
        _context.Users.Add(user1);

        var album = new AlbumModel { Name = "Test Album", UserId = user.UserName, Description = "test" };
        _context.Albums.Add(album);

        var imageToDel = new ImageModel { AlbumId = album.Id, ImageName = "TestImage", ImagePath = "TestPath", ImageUniqueName = "Unique1", Dislikes = 0, Likes = 0 };
        _context.Images.Add(imageToDel);
        
        var image = new ImageModel { AlbumId = album.Id, ImageName = "TestImage1", ImagePath = "TestPath1", ImageUniqueName = "Unique2", Dislikes = 0, Likes = 0 };
        _context.Images.Add(image);

        _context.SaveChanges();
    }

    [TestMethod]
    public async Task DeleteImage_ShouldReturnForbid_WhenUserIsNotAuthorized()
    {
        // Arrange
        var controller = new ImageController(_context, _mockUserManager.Object, _mockHostEnvironment.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "testuser1")
                    }))
                }
            }
        };

        // Act
        var result = await controller.Delete(2);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }

    [TestMethod]
    public async Task DeleteImage_ShouldReturnOk_WhenImageExistsAndUserIsAuthorized()
    {
        // Arrange
        var controller = new ImageController(_context, _mockUserManager.Object, _mockHostEnvironment.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "testuser")
                    }))
                }
            }
        };

        // Act
        var result = await controller.Delete(1);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkResult));
        Assert.IsNull(await _context.Images.FindAsync(1));
    }

    [TestMethod]
    public async Task DeleteImage_ShouldReturnNotFound_WhenImageDoesNotExist()
    {
        // Arrange
        var controller1 = new ImageController(_context, _mockUserManager.Object, _mockHostEnvironment.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                   {
                        new Claim(ClaimTypes.NameIdentifier, "testuser")
                   }))
                }
            }
        };

        // Act
        var result = await controller1.Delete(256);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
}
