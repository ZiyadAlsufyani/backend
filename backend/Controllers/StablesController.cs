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
            var stables = await _context.Stables
                .FromSqlRaw("SELECT * FROM Stable")
                .ToListAsync();
            return Ok(stables);
        }

        // GET: api/Stables/stablel
        [HttpGet("{id}")]
        public async Task<ActionResult<Stable>> GetStable(string id)
        {
            var stable = await _context.Stables
                .FromSql($"SELECT * FROM Stable WHERE stableId = {id}")
                .FirstOrDefaultAsync();

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
            await _context.Database.ExecuteSqlAsync(
                $"INSERT INTO Stable (stableId, stableName, location, colors) VALUES ({stable.StableId}, {stable.StableName}, {stable.Location}, {stable.Colors})"
            );

            var createdStable = await _context.Stables
                .FromSql($"SELECT * FROM Stable WHERE stableId = {stable.StableId}")
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetStable), new { id = stable.StableId }, createdStable);
        }

        // PUT: api/Stables/stablel
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStable(string id, Stable stable)
        {
            if (id != stable.StableId)
            {
                return BadRequest();
            }

            try
            {
                var stableExists = await _context.Database
                    .SqlQuery<int>($"SELECT COUNT(*) as Value FROM Stable WHERE stableId = {id}")
                    .FirstOrDefaultAsync() > 0;

                if (!stableExists)
                {
                    return NotFound();
                }

                await _context.Database.ExecuteSqlAsync(
                    $"UPDATE Stable SET stableName = {stable.StableName}, location = {stable.Location}, colors = {stable.Colors} WHERE stableId = {id}"
                );

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // DELETE: api/Stables/stablel
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStable(string id)
        {
            var stableExists = await _context.Database
                .SqlQuery<int>($"SELECT COUNT(*) as Value FROM Stable WHERE stableId = {id}")
                .FirstOrDefaultAsync() > 0;

            if (!stableExists)
            {
                return NotFound();
            }

            await _context.Database.ExecuteSqlAsync(
                $"DELETE FROM Stable WHERE stableId = {id}"
            );

            return NoContent();
        }
    }
}
