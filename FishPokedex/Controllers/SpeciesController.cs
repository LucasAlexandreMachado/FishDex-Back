using FishPokedex.Data;
using FishPokedex.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishPokedex.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpeciesController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Species>>> GetSpecies()
    {
        return await _context.Species.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Species>> GetSpecies(int id)
    {
        var species = await _context.Species
            .Include(s => s.Catches)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (species == null)
        {
            return NotFound();
        }

        return species;
    }

    [HttpPost]
    public async Task<ActionResult<Species>> PostSpecies(Species species)
    {
        _context.Species.Add(species);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSpecies), new { id = species.Id }, species);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSpecies(int id, Species species)
    {
        if (id != species.Id)
        {
            return BadRequest();
        }

        _context.Entry(species).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SpeciesExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecies(int id)
    {
        var species = await _context.Species.FindAsync(id);
        if (species == null)
        {
            return NotFound();
        }

        _context.Species.Remove(species);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete species because there are linked catches.");
        }

        return NoContent();
    }

    private bool SpeciesExists(int id)
    {
        return _context.Species.Any(s => s.Id == id);
    }
}
