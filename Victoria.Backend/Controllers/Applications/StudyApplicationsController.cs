using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Applications;
using Victoria.Domain.Entities.Applications;
using Victoria.Domain.Enums;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Applications;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Staff,Admin")]
public class StudyApplicationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudyApplicationsController(AppDbContext db)
    {
        _db = db;
    }

    // LIST
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.StudyApplications
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new StudyApplicationGetDto
            {
                Id = x.Id,
                CaseFileId = x.CaseFileId,
                Country = x.Country,
                UniversityName = x.UniversityName,
                ProgramName = x.ProgramName,
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt,
                SubmittedAt = x.SubmittedAt
            })
            .ToListAsync();

        return Ok(items);
    }

    // LIST BY CASEFILE
    [HttpGet("casefile/{caseFileId:int}")]
    public async Task<IActionResult> GetByCaseFile(int caseFileId)
    {
        var items = await _db.StudyApplications
            .Where(x => x.CaseFileId == caseFileId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new StudyApplicationGetDto
            {
                Id = x.Id,
                CaseFileId = x.CaseFileId,
                Country = x.Country,
                UniversityName = x.UniversityName,
                ProgramName = x.ProgramName,
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt,
                SubmittedAt = x.SubmittedAt
            })
            .ToListAsync();

        return Ok(items);
    }

    // DETAILS
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.StudyApplications.FirstOrDefaultAsync(a => a.Id == id);
        if (x == null) return NotFound();

        return Ok(new StudyApplicationGetDto
        {
            Id = x.Id,
            CaseFileId = x.CaseFileId,
            Country = x.Country,
            UniversityName = x.UniversityName,
            ProgramName = x.ProgramName,
            Status = x.Status.ToString(),
            CreatedAt = x.CreatedAt,
            SubmittedAt = x.SubmittedAt
        });
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudyApplicationCreateDto dto)
    {
        var caseExists = await _db.CaseFiles.AnyAsync(c => c.Id == dto.CaseFileId);
        if (!caseExists) return BadRequest("CaseFile not found");

        var app = new StudyApplication
        {
            CaseFileId = dto.CaseFileId,
            Country = dto.Country,
            UniversityName = dto.UniversityName,
            ProgramName = dto.ProgramName,
            Status = ApplicationStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            SubmittedAt = DateTime.UtcNow
        };

        _db.StudyApplications.Add(app);
        await _db.SaveChangesAsync();

        return Ok(new { app.Id });
    }

    // UPDATE
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] StudyApplicationUpdateDto dto)
    {
        var app = await _db.StudyApplications.FirstOrDefaultAsync(x => x.Id == id);
        if (app == null) return NotFound();

        if (!Enum.TryParse<ApplicationStatus>(dto.Status, true, out var status))
            return BadRequest("Invalid status");

        app.Country = dto.Country;
        app.UniversityName = dto.UniversityName;
        app.ProgramName = dto.ProgramName;

        // prosta logika: jeśli przechodzimy na Submitted i nie ma daty, ustaw
        if (status == ApplicationStatus.Submitted && app.SubmittedAt == null)
            app.SubmittedAt = DateTime.UtcNow;

        app.Status = status;

        await _db.SaveChangesAsync();
        return Ok(new { app.Id });
    }

    // DELETE (Admin)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var app = await _db.StudyApplications.FirstOrDefaultAsync(x => x.Id == id);
        if (app == null) return NotFound();

        _db.StudyApplications.Remove(app);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
