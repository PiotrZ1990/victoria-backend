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
[AllowAnonymous]
//[Authorize(Roles = "Staff,Admin")]
public class VisaApplicationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public VisaApplicationsController(AppDbContext db)
    {
        _db = db;
    }

    // POST: api/visaapplications/from-casefile
    [HttpPost("from-casefile")]
    public async Task<IActionResult> CreateFromCaseFile([FromBody] VisaApplicationCreateFromCaseFileDto dto)
    {
        var caseFile = await _db.CaseFiles.FirstOrDefaultAsync(x => x.Id == dto.CaseFileId);
        if (caseFile == null) return NotFound("CaseFile not found");

        var existing = await _db.VisaApplications.FirstOrDefaultAsync(x => x.CaseFileId == dto.CaseFileId);
        if (existing != null) return BadRequest("VisaApplication already exists for this CaseFile");

        if (!Enum.TryParse<VisaType>(dto.VisaType, true, out var visaType))
            return BadRequest("Invalid visa type");

        var visa = new VisaApplication
        {
            CaseFileId = dto.CaseFileId,
            Country = dto.Country,
            VisaType = visaType,
            Status = ApplicationStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        _db.VisaApplications.Add(visa);
        await _db.SaveChangesAsync();

        return Ok(visa);
    }

    // GET: api/visaapplications/by-casefile/{caseFileId}
    [HttpGet("by-casefile/{caseFileId:int}")]
    public async Task<IActionResult> GetByCaseFile(int caseFileId)
    {
        var visa = await _db.VisaApplications.FirstOrDefaultAsync(x => x.CaseFileId == caseFileId);
        if (visa == null) return NotFound("VisaApplication not found");
        return Ok(visa);
    }
}
