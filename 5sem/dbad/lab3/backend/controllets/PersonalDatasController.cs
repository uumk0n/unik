using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace backend;
[Route("api/[controller]")]
[ApiController]
public class PersonalDatasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PersonalDatasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonalData>>> GetPersonalDatas()
    {
        var personalDatas = await _context.PersonalDatas.ToListAsync();
        return Ok(personalDatas);
    }

    [HttpGet("{fcs}")]
    public async Task<ActionResult<PersonalData>> GetPersonalData(int inila)
    {
        var personalData = await _context.PersonalDatas.FindAsync(inila);

        if (personalData == null)
        {
            return NotFound();
        }

        return personalData;
    }

    [HttpPost]
    public async Task<ActionResult<PersonalData>> PostPersonalData(PersonalData personalData)
    {
        _context.PersonalDatas.Add(personalData);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPersonalData), new { inila = personalData.Inila }, personalData);
    }

    [HttpPut("{fcs}")]
    public async Task<IActionResult> UpdatePersonalData(string Fcs, PersonalData PersonalData)
    {
        if (Fcs != PersonalData.Fcs)
        {
            return BadRequest();
        }

        _context.Entry(PersonalData).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PersonalDataExists(Fcs))
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

    private bool PersonalDataExists(string Fcs)
    {
        return _context.PersonalDatas.Any(e => e.Fcs == Fcs);
    }

    [HttpDelete("{fcs}")]
    public async Task<IActionResult> DeletePersonalData(string Fcs)
    {
        var PersonalData = await _context.PersonalDatas.FindAsync(Fcs);

        if (PersonalData == null)
        {
            return NotFound();
        }

        _context.PersonalDatas.Remove(PersonalData);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("GetComboBoxOptions")]
    public async Task<ActionResult<IEnumerable<string>>> GetComboBoxOptionsForPersonalDatas([FromQuery] string columnName)
    {
        // Assuming columnName is a valid property of PersonalDatas
        var options = await _context.PersonalDatas
            .Select(GetPropertyValue(columnName))
            .Distinct()
            .ToListAsync();

        return Ok(options);
    }

    private static System.Linq.Expressions.Expression<System.Func<PersonalData, string>> GetPropertyValue(string propertyName)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(PersonalData), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);
        var lambda = System.Linq.Expressions.Expression.Lambda<System.Func<PersonalData, string>>(property, parameter);
        return lambda;
    }

    [HttpPost("GenerateReport")]
    public async Task<IActionResult> GenerateReport([FromBody] List<string> tableNames)
    {
        Console.WriteLine($"Received tableNames: {string.Join(", ", tableNames)}");

        try
        {
            var combinedJsonData = await CreateCombinedJsonAsync(tableNames);
            return Ok(combinedJsonData);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating report: {ex.Message}");
        }
    }

    private async Task<List<Dictionary<string, object>>> CreateCombinedJsonAsync(List<string> tableNames)
    {
        var result = new List<Dictionary<string, object>>();

        try
        {
            // Fetch data for each table
            var eduDocsData = _context.EduDocs.ToList();
            var hfInfoData = _context.HfInfos.ToList();
            var institutData = _context.Instituts.ToList();
            var personalData = _context.PersonalDatas.ToList();
            var workData = _context.Works.ToList();

            // Combine the data using LINQ join operations
            var combinedData = from personal in personalData
                               join eduDocs in eduDocsData on personal.Inila equals eduDocs.Inila into eduDocsGroup
                               from eduDocsItem in eduDocsGroup.DefaultIfEmpty()
                               join hfInfo in hfInfoData on personal.Inila equals hfInfo.Inila into hfInfoGroup
                               from hfInfoItem in hfInfoGroup.DefaultIfEmpty()
                               join institute in institutData on eduDocsItem?.RegEduDoc equals institute?.RegEduDoc into instituteGroup
                               from instituteItem in instituteGroup.DefaultIfEmpty()
                               join work in workData on hfInfoItem?.RegOrgNum equals work?.RegOrgNum into workGroup
                               from workItem in workGroup.DefaultIfEmpty()
                               select new Dictionary<string, object>
                           {
                               { "EduDocs", eduDocsItem },
                               { "HfInfos", hfInfoItem },
                               { "Institutes", instituteItem },
                               { "PersonalData", personal },
                               { "Works", workItem }
                           };

            result.AddRange(combinedData);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return result;
    }



    private List<Dictionary<string, object>> ConvertDataToJson<T>(List<T> dataList)
    {
        var jsonList = new List<Dictionary<string, object>>();

        foreach (var dataItem in dataList)
        {
            var propertyDict = new Dictionary<string, object>();

            foreach (var property in typeof(T).GetProperties())
            {
                propertyDict[property.Name] = property.GetValue(dataItem);
            }

            jsonList.Add(propertyDict);
        }

        return jsonList;
    }

}
