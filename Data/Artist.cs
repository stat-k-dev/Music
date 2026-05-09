 using System.ComponentModel.DataAnnotations;

namespace MusicAPI.Data
{
    public class Artist
    {
        [Key]
        public Guid Id { get; set; }
    [Required]
        public string? Name { get; set; }
    [Required]
        public DateTime AddedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivatedDate { get; set; }


        virtual public ICollection<Song> Songs { get; set; }=[];
        virtual public ICollection<Disc> Discs { get; set; }    =[];
    }

       public class ArtistDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
    }
}