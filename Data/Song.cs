using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicAPI.Data
{
    public class Song
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be a positive integer.")]
        public int Duration { get; set; } // Duration in seconds

        [Required]
        public Guid GenreId { get; set; }

        [ForeignKey("GenreId")]
        public Genre? Genre { get; set; }
        public Guid DiscId { get; set; }

        [ForeignKey("DiscId")]
        public Disc? Disc { get; set; }

        [Required]
        public Guid ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public Artist? Artist { get; set; }

        [Required]
        public DateTime AddedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivatedDate { get; set; }
    }

    public class SongDTO
    {
        public string? Title { get; set; }
        public int Duration { get; set; }
        public Guid? GenreId { get; set; }
        public Guid? DiscId { get; set; }
        public Guid? ArtistId { get; set; }
    }
}
