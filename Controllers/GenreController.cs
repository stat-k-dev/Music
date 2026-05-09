using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAPI.Data;
using MusicAPI.Models;

namespace MusicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly MusicContext _context;

        public GenreController(MusicContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres(int skip = 0, int take = 10)
        {
            return await _context.Genres.Skip(skip).Take(take).ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Genre>>> SearchGenres(
            string name,
            int skip = 0,
            int take = 10
        )
        {
            return await _context
                .Genres.Where(g => g.Name.Contains(name))
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> GetGenre(Guid id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            return genre;
        }

        [HttpPost]
        public async Task<ActionResult<Genre>> CreateGenre(GenreDTO genre)
        {
            if (string.IsNullOrWhiteSpace(genre.Name))
            {
                return BadRequest("Genre name is required.");
            }
            if (
                _context.Genres.Any(g =>
                    g.Name != null && g.Name.Trim().ToUpper() == genre.Name.Trim().ToUpper()
                )
            )
            {
                return Conflict("A genre with the same name already exists.");
            }
            var genreEntity = new Genre { Id = Guid.NewGuid(), Name = genre.Name.Trim() };
            _context.Genres.Add(genreEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGenre), new { id = genreEntity.Id }, genreEntity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Genre>> UpdateGenre(Guid id, Genre genre)
        {
            if (id != genre.Id)
            {
                return BadRequest();
            }

            _context.Entry(genre).State = EntityState.Modified;

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
        public async Task<ActionResult<Genre>> DeleteGenre(Guid id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return genre;
        }
    }
}
