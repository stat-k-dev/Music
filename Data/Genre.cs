using System.ComponentModel.DataAnnotations;

namespace MusicAPI.Data
{
    public class Genre
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Song> Songs { get; set; } = [];
    }

    public class GenreDTO
    {
        public Guid? Id {get;set;}
        public string? Name {get;set;}
        public string?Description{get;set}
    }
}
