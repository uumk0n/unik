namespace backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EduDocsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EduDocsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EduDocs>>> GetEduDocs()
    {
        var eduDocs = await _context.EduDocs.ToListAsync();
        return Ok(eduDocs);
    }

    [HttpGet("{inila}")]
    public async Task<ActionResult<EduDocs>> GetEduDoc(int id)
    {
        var eduDoc = await _context.EduDocs.FindAsync(id);

        if (eduDoc == null)
        {
            return NotFound();
        }

        return eduDoc;
    }

    [HttpPost]
    public async Task<ActionResult<EduDocs>> PostEduDoc(EduDocs eduDoc)
    {
        Console.WriteLine("eduDoc:" + eduDoc);
        _context.EduDocs.Add(eduDoc);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEduDoc), new { id = eduDoc.Inila }, eduDoc);
    }

    [HttpPut("{inila}")]
    public async Task<IActionResult> UpdateEduDoc(string Inila, EduDocs eduDoc)
    {
        if (Inila != eduDoc.Inila)
        {
            return BadRequest();
        }

        _context.Entry(eduDoc).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EduDocExists(Inila))
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

    private bool EduDocExists(string Inila)
    {
        return _context.EduDocs.Any(e => e.Inila == Inila);
    }

    [HttpDelete("{inila}")]
    public async Task<IActionResult> DeleteEduDoc(string inila)
    {
        var eduDoc = await _context.EduDocs.FindAsync(inila);

        if (eduDoc == null)
        {
            return NotFound();
        }

        _context.EduDocs.Remove(eduDoc);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("GetComboBoxOptions")]
    public async Task<ActionResult<IEnumerable<string>>> GetComboBoxOptionsForEduDocs([FromQuery] string columnName)
    {
        // Assuming columnName is a valid property of EduDocs
        var options = await _context.EduDocs
            .Select(GetPropertyValue(columnName))
            .Distinct()
            .ToListAsync();

        return Ok(options);
    }

    private static System.Linq.Expressions.Expression<System.Func<EduDocs, string>> GetPropertyValue(string propertyName)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(EduDocs), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);
        var lambda = System.Linq.Expressions.Expression.Lambda<System.Func<EduDocs, string>>(property, parameter);
        return lambda;
    }
}
