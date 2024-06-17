using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhotoGallery.Server.Models;

namespace PhotoGallery.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AlbumModel> Albums { get; set; } = default!;
        public DbSet<ImageModel> Images { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ImageModel>()
                .HasOne(i => i.Album)
                .WithMany(a => a.Images)
                .HasForeignKey(i => i.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
