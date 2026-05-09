using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAPI.Data;
using MusicAPI.Models;

namespace MusicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly MusicContext _context;

        public ArtistController(MusicContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Artist>>> GetArtists(int skip = 0, int take = 10)
        {
            return await _context.Artists.Skip(skip).Take(take).ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Artist>>> SearchArtists(
            string? name,
            int skip = 0,
            int take = 10
        )
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name query parameter is required.");
            }

            return await _context
                .Artists.Where(a => a.Name != null && a.Name.Contains(name.Trim()))
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Artist>> GetArtist(Guid id)
        {
            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
            {
                return NotFound();
            }

            return artist;
        }

        [HttpGet("{id}/discs")]
        public async Task<ActionResult<IEnumerable<Disc>>> GetArtistDiscs(Guid id)
        {
            var artist = await _context
                .Artists.Include(a => a.Discs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null)
            {
                return NotFound();
            }

            return Ok(artist.Discs);
        }

        [HttpGet("{id}/songs")]
        public async Task<ActionResult<IEnumerable<Song>>> GetArtistSongs(Guid id)
        {
            var artist = await _context
                .Artists.Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null)
            {
                return NotFound();
            }

            return Ok(artist.Songs);
        }

        [HttpPost]
        public async Task<ActionResult<Artist>> CreateArtist(ArtistDTO artist)
        {
            if (string.IsNullOrWhiteSpace(artist.Name))
            {
                return BadRequest("Artist name is required.");
            }
            if (
                _context.Artists.Any(a =>
                    a.Name != null && a.Name.Trim().ToUpper() == artist.Name.Trim().ToUpper()
                )
            )
            {
                return Conflict("An artist with the same name already exists.");
            }
            var artistEntity = new Artist
            {
                Id = Guid.NewGuid(),
                Name = artist.Name.Trim(),
                AddedDate = DateTime.UtcNow,
                IsActive = true,
            };
            _context.Artists.Add(artistEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArtist), new { id = artistEntity.Id }, artist);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Artist>> UpdateArtist(Guid id, ArtistDTO artist)
        {
            if (id != artist.Id)
            {
                return BadRequest();
            }

            if (!ArtistExists(id))
            {
                return NotFound();
            }

            var artistEntity = await _context.Artists.FindAsync(id);
            artistEntity.Name = artist.Name.Trim() ?? artistEntity.Name;

            _context.Entry(artistEntity).State = EntityState.Modified;

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
        public async Task<IActionResult> DeleteArtist(Guid id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            artist.IsActive = false;
            artist.DeactivatedDate = DateTime.UtcNow;
            _context.Entry(artist).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArtistExists(Guid id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
}
