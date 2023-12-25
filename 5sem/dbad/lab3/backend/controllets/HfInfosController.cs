namespace backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class HfInfosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HfInfosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HFInfo>>> GetHfInfos()
    {
        var hfInfos = await _context.HfInfos.ToListAsync();
        return Ok(hfInfos);
    }

    [HttpGet("{inila}")]
    public async Task<ActionResult<HFInfo>> GetHfInfo(string Inila)
    {
        var hfInfo = await _context.HfInfos.FindAsync(Inila);

        if (hfInfo == null)
        {
            return NotFound();
        }

        return hfInfo;
    }

    [HttpPost]
    public async Task<ActionResult<HFInfo>> PostHfInfo(HFInfo hfInfo)
    {
        _context.HfInfos.Add(hfInfo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHfInfo), new { id = hfInfo.Inila }, hfInfo);
    }

    [HttpPut("{inila}")]
    public async Task<IActionResult> UpdateHFInfo(string Inila, HFInfo HFInfo)
    {
        Console.WriteLine("HFInfo:" + HFInfo.Inila);
        Console.WriteLine("Inila:" + Inila);
        if (Inila != HFInfo.Inila)
        {
            return BadRequest();
        }

        _context.Entry(HFInfo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!HFInfoExists(Inila))
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

    private bool HFInfoExists(string Inila)
    {
        return _context.HfInfos.Any(e => e.Inila == Inila);
    }

    [HttpDelete("{inila}")]
    public async Task<IActionResult> DeleteHFInfo(string inila)
    {
        var HFInfo = await _context.HfInfos.FindAsync(inila);

        if (HFInfo == null)
        {
            return NotFound();
        }

        _context.HfInfos.Remove(HFInfo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("GetComboBoxOptions")]
    public async Task<ActionResult<IEnumerable<string>>> GetComboBoxOptionsForHFInfo([FromQuery] string columnName)
    {
        // Assuming columnName is a valid property of HFInfo
        var options = await _context.HfInfos
            .Select(GetPropertyValue(columnName))
            .Distinct()
            .ToListAsync();

        return Ok(options);
    }

    private static System.Linq.Expressions.Expression<System.Func<HFInfo, string>> GetPropertyValue(string propertyName)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(HFInfo), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);
        var lambda = System.Linq.Expressions.Expression.Lambda<System.Func<HFInfo, string>>(property, parameter);
        return lambda;
    }
}
