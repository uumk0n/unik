using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend;
[Route("api/[controller]")]
[ApiController]
public class WorksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WorksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Work>>> GetWorks()
    {
        var works = await _context.Works.ToListAsync();
        return Ok(works);
    }

    [HttpGet("{regorgnum}")]
    public async Task<ActionResult<Work>> GetWork(string regOrgNum)
    {
        var work = await _context.Works.FindAsync(regOrgNum);

        if (work == null)
        {
            return NotFound();
        }

        return work;
    }

    [HttpPost]
    public async Task<ActionResult<Work>> PostWork(Work work)
    {
        _context.Works.Add(work);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWork), new { regOrgNum = work.RegOrgNum }, work);
    }

    [HttpPut("{regorgnum}")]
    public async Task<IActionResult> UpdateWork(string RegOrgNum, Work Work)
    {
        if (RegOrgNum != Work.RegOrgNum)
        {
            return BadRequest();
        }

        _context.Entry(Work).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WorkExists(RegOrgNum))
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

    private bool WorkExists(string RegOrgNum)
    {
        return _context.Works.Any(e => e.RegOrgNum == RegOrgNum);
    }

    [HttpDelete("{regorgnum}")]
    public async Task<IActionResult> DeleteWork(string RegOrgNum)
    {
        var Work = await _context.Works.FindAsync(RegOrgNum);

        if (Work == null)
        {
            return NotFound();
        }

        _context.Works.Remove(Work);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("GetComboBoxOptions")]
    public async Task<ActionResult<IEnumerable<string>>> GetComboBoxOptionsForWorks([FromQuery] string columnName)
    {
        // Assuming columnName is a valid property of Works
        var options = await _context.Works
            .Select(GetPropertyValue(columnName))
            .Distinct()
            .ToListAsync();

        return Ok(options);
    }

    private static System.Linq.Expressions.Expression<System.Func<Work, string>> GetPropertyValue(string propertyName)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Work), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);
        var lambda = System.Linq.Expressions.Expression.Lambda<System.Func<Work, string>>(property, parameter);
        return lambda;
    }
}
