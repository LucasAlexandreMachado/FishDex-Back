using FishPokedex.Data;
using FishPokedex.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishPokedex.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatchDetailsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatchDetail>>> GetCatchDetails()
    {
        return await _context.CatchDetails
            .Include(cd => cd.Catch)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CatchDetail>> GetCatchDetail(int id)
    {
        var catchDetail = await _context.CatchDetails
            .Include(cd => cd.Catch)
            .FirstOrDefaultAsync(cd => cd.Id == id);

        if (catchDetail == null)
        {
            return NotFound();
        }

        return catchDetail;
    }

    [HttpGet("catch/{catchId}")]
    public async Task<ActionResult<CatchDetail>> GetCatchDetailByCatchId(int catchId)
    {
        var catchDetail = await _context.CatchDetails
            .Include(cd => cd.Catch)
            .FirstOrDefaultAsync(cd => cd.CatchId == catchId);

        if (catchDetail == null)
        {
            return NotFound();
        }

        return catchDetail;
    }

    [HttpPost]
    public async Task<ActionResult<CatchDetail>> PostCatchDetail(CatchDetail catchDetail)
    {
        // Check if the referenced Catch exists
        var catchExists = await _context.Catches.AnyAsync(c => c.Id == catchDetail.CatchId);
        if (!catchExists)
        {
            return BadRequest("The referenced Catch does not exist.");
        }

        // Check if the Catch already has a detail registered
        var alreadyHasDetail = await _context.CatchDetails.AnyAsync(cd => cd.CatchId == catchDetail.CatchId);
        if (alreadyHasDetail)
        {
            return Conflict("This Catch already has a detail registered.");
        }

        _context.CatchDetails.Add(catchDetail);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCatchDetail), new { id = catchDetail.Id }, catchDetail);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCatchDetail(int id, CatchDetail catchDetail)
    {
        if (id != catchDetail.Id)
        {
            return BadRequest();
        }

        _context.Entry(catchDetail).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CatchDetailExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCatchDetail(int id)
    {
        var catchDetail = await _context.CatchDetails.FindAsync(id);
        if (catchDetail == null)
        {
            return NotFound();
        }

        _context.CatchDetails.Remove(catchDetail);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CatchDetailExists(int id)
    {
        return _context.CatchDetails.Any(cd => cd.Id == id);
    }
}
