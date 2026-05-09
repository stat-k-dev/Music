using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAPI.Data;
using MusicAPI.Models;

namespace MusicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongController : ControllerBase
    {
        private readonly MusicContext _context;

        public SongController(MusicContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs(int skip = 0, int take = 10)
        {
            return await _context.Songs.Skip(skip).Take(take).ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Song>>> SearchSongs(
            string title,
            int skip = 0,
            int take = 10
        )
        {
            return await _context
                .Songs.Where(s => s.Title.Contains(title))
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSong(Guid id)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            return song;
        }

        [HttpGet("artist/{artistId}")]
        public async Task<ActionResult<IEnumerable<Song>>> GetArtistSongs(
            Guid artistId,
            int skip = 0,
            int take = 10
        )
        {
            var songs = await _context
                .Songs.Where(s => s.ArtistId == artistId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return Ok(songs);
        }

        [HttpGet("disc/{discId}")]
        public async Task<ActionResult<IEnumerable<Song>>> GetDiscSongs(
            Guid discId,
            int skip = 0,
            int take = 10
        )
        {
            var songs = await _context
                .Songs.Where(s => s.DiscId == discId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return Ok(songs);
        }

        [HttpGet("genre/{genreId}")]
        public async Task<ActionResult<IEnumerable<Song>>> GetGenreSongs(
            Guid genreId,
            int skip = 0,
            int take = 10
        )
        {
            var songs = await _context
                .Songs.Where(s => s.GenreId == genreId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return Ok(songs);
        }

        [HttpPost]
        public async Task<ActionResult<Song>> CreateSong(SongDTO song)
        {
            if (string.IsNullOrWhiteSpace(song.Title))
            {
                return BadRequest("The song title is required.");
            }
            if (_context.Songs.Any(s => s.Title == song.Title))
            {
                return Conflict("A song with the same title already exists.");
            }
            if (song.GenreId != Guid.Empty && !_context.Genres.Any(g => g.Id == song.GenreId))
            {
                return BadRequest("The specified genre does not exist.");
            }
            if (song.DiscId != Guid.Empty && !_context.Discs.Any(d => d.Id == song.DiscId))
            {
                return BadRequest("The specified disc does not exist.");
            }
            if (song.ArtistId != Guid.Empty && !_context.Artists.Any(a => a.Id == song.ArtistId))
            {
                return BadRequest("The specified artist does not exist.");
            }
            var newSong = new Song
            {
                Id = Guid.NewGuid(),
                Title = song.Title,
                Duration = song.Duration,
                GenreId = song.GenreId ?? Guid.Empty,
                DiscId = song.DiscId ?? Guid.Empty,
                ArtistId = song.ArtistId ?? Guid.Empty,
                AddedDate = DateTime.UtcNow,
                IsActive = true,
            };
            _context.Songs.Add(newSong);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSong), new { id = newSong.Id }, newSong);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Song>> UpdateSong(Guid id, Song song)
        {
            if (id != song.Id)
            {
                return BadRequest();
            }

            if (!SongExists(id))
            {
                return NotFound();
            }

            _context.Entry(song).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Song>> DeleteSong(Guid id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            song.IsActive = false;
            song.DeactivatedDate = DateTime.UtcNow;
            _context.Entry(song).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return song;
        }

        private bool SongExists(Guid id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}
