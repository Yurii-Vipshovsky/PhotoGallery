using System.ComponentModel.DataAnnotations;

namespace PhotoGallery.Server.Models
{
    public class AlbumModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public string UserId { get; set; }
        public List<ImageModel>? Images { get; set; }
    }
}
