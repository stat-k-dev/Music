using Microsoft.EntityFrameworkCore;

namespace MusicAPI.Models
{
    using Data;
    public class MusicContext : DbContext
    {
        public MusicContext(DbContextOptions<MusicContext> options) : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Disc> Discs { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Artist)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.ArtistId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Disc>()
                .HasOne(d => d.Artist)
                .WithMany(a => a.Discs)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Song>()
                .HasOne(s => s.Disc)
                .WithMany(d => d.Songs)
                .HasForeignKey(s => s.DiscId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}