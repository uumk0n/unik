using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend;
[Route("api/[controller]")]
[ApiController]
public class InstitutsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InstitutsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Institut>>> GetInstituts()
    {
        var instituts = await _context.Instituts.ToListAsync();
        return Ok(instituts);
    }

    [HttpGet("{regedudoc}")]
    public async Task<ActionResult<Institut>> GetInstitut(int regedudoc)
    {
        var institut = await _context.Instituts.FindAsync(regedudoc);

        if (institut == null)
        {
            return NotFound();
        }

        return institut;
    }

    [HttpPost]
    public async Task<ActionResult<Institut>> PostInstitut(Institut institut)
    {
        _context.Instituts.Add(institut);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInstitut), new { regedudoc = institut.RegEduDoc }, institut);
    }

    [HttpPut("{regedudoc}")]
    public async Task<IActionResult> UpdateInstitut(string RegEduDoc, Institut Institut)
    {
        if (RegEduDoc != Institut.RegEduDoc)
        {
            return BadRequest();
        }

        _context.Entry(Institut).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InstitutExists(RegEduDoc))
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

    private bool InstitutExists(string RegEduDoc)
    {
        return _context.Instituts.Any(e => e.RegEduDoc == RegEduDoc);
    }

    [HttpDelete("{regedudoc}")]
    public async Task<IActionResult> DeleteInstitut(string RegEduDoc)
    {
        var Institut = await _context.Instituts.FindAsync(RegEduDoc);

        if (Institut == null)
        {
            return NotFound();
        }

        _context.Instituts.Remove(Institut);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("GetComboBoxOptions")]
    public async Task<ActionResult<IEnumerable<string>>> GetComboBoxOptionsForInstituts([FromQuery] string columnName)
    {
        // Assuming columnName is a valid property of Institut
        var options = await _context.Instituts
            .Select(GetPropertyValue(columnName))
            .Distinct()
            .ToListAsync();

        return Ok(options);
    }

    private static System.Linq.Expressions.Expression<System.Func<Institut, string>> GetPropertyValue(string propertyName)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Institut), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);
        var lambda = System.Linq.Expressions.Expression.Lambda<System.Func<Institut, string>>(property, parameter);
        return lambda;
    }
}
