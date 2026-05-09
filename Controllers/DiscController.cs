using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAPI.Data;
using MusicAPI.Models;

namespace MusicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscController : ControllerBase
    {
        private readonly MusicContext _context;

        public DiscController(MusicContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Disc>>> GetDiscs(int skip = 0, int take = 10)
        {
            return await _context.Discs.Skip(skip).Take(take).ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Disc>>> SearchDiscs(
            string? title,
            int skip = 0,
            int take = 10
        )
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title query parameter is required.");
            }

            var response = await _context
                .Discs.Where(d => d.Name != null && d.Name.Contains(title.Trim()))
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            if (response == null || response.Count == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Disc>> GetDisc(Guid id)
        {
            var disc = await _context.Discs.FindAsync(id);

            if (disc == null)
            {
                return NotFound();
            }

            return disc;
        }

        [HttpPost]
        public async Task<ActionResult<Disc>> CreateDisc(DiscDTO disc)
        {
            if (string.IsNullOrWhiteSpace(disc.Name))
            {
                return BadRequest("Disc name is required.");
            }
            if (
                _context.Discs.Any(a =>
                    a.Name != null && a.Name.Trim().ToUpper() == disc.Name.Trim().ToUpper()
                )
            )
            {
                return Conflict("A disc with the same name already exists.");
            }
            var discEntity = new Disc
            {
                Id = Guid.NewGuid(),
                Name = disc.Name.Trim(),
                ReleaseDate = disc.ReleaseDate,
                ArtistId = disc.ArtistId ?? Guid.Empty,
                AddedDate = DateTime.UtcNow,
                IsActive = true,
            };
            _context.Discs.Add(discEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDisc), new { id = discEntity.Id }, discEntity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Disc>> UpdateDisc(Guid id, Disc disc)
        {
            if (id != disc.Id)
            {
                return BadRequest();
            }

            if (!DiscExists(id))
            {
                return NotFound();
            }

            _context.Entry(disc).State = EntityState.Modified;

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
        public async Task<IActionResult> DeleteDisc(Guid id)
        {
            var disc = await _context.Discs.FindAsync(id);
            if (disc == null)
            {
                return NotFound();
            }

            disc.IsActive = false;
            disc.DeactivatedDate = DateTime.UtcNow;
            _context.Entry(disc).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiscExists(Guid id)
        {
            return _context.Discs.Any(e => e.Id == id);
        }
    }
}
