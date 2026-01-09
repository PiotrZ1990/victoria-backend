using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Visa;
using Victoria.Domain.Entities.Visa;
using Victoria.Domain.Enums;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Visa;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Staff,Admin")]
public class VisaApplicationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public VisaApplicationsController(AppDbContext db)
    {
        _db = db;
    }

    // =========================================
    // CREATE FROM CASEFILE
    // POST: api/visaapplications/from-casefile
    // =========================================
    [HttpPost("from-casefile")]
    public async Task<IActionResult> CreateFromCaseFile(
        [FromBody] VisaApplicationCreateFromCaseFileDto dto)
    {
        var caseExists = await _db.CaseFiles.AnyAsync(x => x.Id == dto.CaseFileId);
        if (!caseExists)
            return BadRequest("CaseFile not found");

        if (!Enum.TryParse<VisaType>(dto.VisaType, true, out var visaType))
            return BadRequest("Invalid visa type");

        if (!Enum.TryParse<ApplicationStatus>(dto.Status, true, out var status))
            status = ApplicationStatus.Draft;

        var entity = new VisaApplication
        {
            CaseFileId = dto.CaseFileId,
            Country = dto.Country,
            VisaType = visaType,
            Status = status,
            AppointmentDate = dto.AppointmentDate,
            DecisionDate = dto.DecisionDate,
            CreatedAt = DateTime.UtcNow
        };

        _db.VisaApplications.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(Map(entity));
    }

    // =========================================
    // LIST BY CASEFILE
    // GET: api/visaapplications/by-casefile/{caseFileId}
    // =========================================
    [HttpGet("by-casefile/{caseFileId:int}")]
    public async Task<IActionResult> GetByCaseFile(int caseFileId)
    {
        var list = await _db.VisaApplications
            .Where(x => x.CaseFileId == caseFileId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(list.Select(Map));
    }

    // =========================================
    // GET BY ID
    // GET: api/visaapplications/{id}
    // =========================================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _db.VisaApplications.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return NotFound();

        return Ok(Map(entity));
    }

    // =========================================
    // UPDATE
    // PUT: api/visaapplications/{id}
    // =========================================
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] VisaApplicationUpdateDto dto)
    {
        var entity = await _db.VisaApplications.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return NotFound();

        if (!Enum.TryParse<VisaType>(dto.VisaType, true, out var visaType))
            return BadRequest("Invalid visa type");

        if (!Enum.TryParse<ApplicationStatus>(dto.Status, true, out var status))
            return BadRequest("Invalid status");

        entity.Country = dto.Country;
        entity.VisaType = visaType;
        entity.Status = status;
        entity.AppointmentDate = dto.AppointmentDate;
        entity.DecisionDate = dto.DecisionDate;

        await _db.SaveChangesAsync();
        return Ok(Map(entity));
    }

    // =========================================
    // DELETE
    // DELETE: api/visaapplications/{id}
    // =========================================
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.VisaApplications.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return NotFound();

        _db.VisaApplications.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // =========================================
    // MAPPER
    // =========================================
    private static VisaApplicationGetDto Map(VisaApplication x) => new()
    {
        Id = x.Id,
        CaseFileId = x.CaseFileId,
        Country = x.Country,
        VisaType = x.VisaType.ToString(),
        Status = x.Status.ToString(),
        AppointmentDate = x.AppointmentDate,
        DecisionDate = x.DecisionDate,
        CreatedAt = x.CreatedAt
    };
}
