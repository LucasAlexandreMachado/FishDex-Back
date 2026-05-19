using FishPokedex.Data;
using FishPokedex.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishPokedex.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatchesController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Catch>>> GetCatches()
    {
        return await _context.Catches
            .Include(c => c.Species)
            .Include(c => c.CatchDetail)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Catch>> GetCatch(int id)
    {
        var catchEntity = await _context.Catches
            .Include(c => c.Species)
            .Include(c => c.CatchDetail)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (catchEntity == null)
        {
            return NotFound();
        }

        return catchEntity;
    }

    [HttpPost]
    public async Task<ActionResult<Catch>> PostCatch(Catch catchEntity)
    {
        // Set CatchDate to UtcNow
        catchEntity.CatchDate = DateTime.UtcNow;

        _context.Catches.Add(catchEntity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCatch), new { id = catchEntity.Id }, catchEntity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCatch(int id, Catch catchEntity)
    {
        if (id != catchEntity.Id)
        {
            return BadRequest();
        }

        _context.Entry(catchEntity).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CatchExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCatch(int id)
    {
        var catchEntity = await _context.Catches.FindAsync(id);
        if (catchEntity == null)
        {
            return NotFound();
        }

        _context.Catches.Remove(catchEntity);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CatchExists(int id)
    {
        return _context.Catches.Any(c => c.Id == id);
    }
}
