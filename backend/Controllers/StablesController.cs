using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StablesController : ControllerBase
    {
        private readonly RacingDbContext _context;

        public StablesController(RacingDbContext context)
        {
            _context = context;
        }

        // GET: api/Stables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stable>>> GetStables()
        {
            return await _context.Stables.ToListAsync();
        }

        // GET: api/Stables/stablel
        [HttpGet("{id}")]
        public async Task<ActionResult<Stable>> GetStable(string id)
        {
            var stable = await _context.Stables.FindAsync(id);

            if (stable == null)
            {
                return NotFound();
            }

            return stable;
        }

        // POST: api/Stables
        [HttpPost]
        public async Task<ActionResult<Stable>> PostStable(Stable stable)
        {
            _context.Stables.Add(stable);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStable), new { id = stable.StableId }, stable);
        }

        // PUT: api/Stables/stablel
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStable(string id, Stable stable)
        {
            if (id != stable.StableId)
            {
                return BadRequest();
            }

            _context.Entry(stable).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Stables/stablel
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStable(string id)
        {
            var stable = await _context.Stables.FindAsync(id);
            if (stable == null)
            {
                return NotFound();
            }

            _context.Stables.Remove(stable);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StableExists(string id)
        {
            return _context.Stables.Any(e => e.StableId == id);
        }
    }
}
