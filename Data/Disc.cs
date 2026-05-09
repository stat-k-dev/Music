using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicAPI.Data
{
    public class Disc
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public Guid ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public Artist? Artist { get; set; }

        [Required]
        public DateTime AddedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivatedDate { get; set; }

        public virtual ICollection<Song> Songs { get; set; } = [];
    }

    public class DiscDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid? ArtistId { get; set; }
    }
}
