using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoGallery.Server.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string? ImageName { get; set; }
        public string? ImagePath { get; set; }
        public string? ImageUniqueName { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int AlbumId { get; set; }
        [ForeignKey("AlbumId")]
        public AlbumModel? Album { get; set; }
    }
}
