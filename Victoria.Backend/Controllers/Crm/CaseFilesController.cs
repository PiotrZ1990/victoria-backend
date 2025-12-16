using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Crm;
using Victoria.Domain.Entities.Cases;
using Victoria.Domain.Enums;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Crm;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Staff,Admin")]
public class CaseFilesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public CaseFilesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // =========================================
    // CREATE CASEFILE FROM LEAD
    // POST: api/casefiles/from-lead
    // =========================================
    [HttpPost("from-lead")]
    public async Task<IActionResult> CreateFromLead([FromBody] CaseFileCreateFromLeadDto dto)
    {
        var lead = await _dbContext.Leads
            .FirstOrDefaultAsync(x => x.Id == dto.LeadId);

        if (lead == null)
            return NotFound("Lead not found");

        if (lead.Status == "Converted")
            return BadRequest("Lead already converted");

        var caseFile = new CaseFile
        {
            LeadId = lead.Id,
            CaseNumber = $"CASE-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Stage = CaseStage.New,
            InternalNotes = dto.InternalNotes,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.CaseFiles.Add(caseFile);

        // aktualizacja statusu leada
        lead.Status = "Converted";

        await _dbContext.SaveChangesAsync();

        return Ok(MapToGetDto(caseFile));
    }

    // =========================================
    // GET ALL CASEFILES
    // GET: api/casefiles
    // =========================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var caseFiles = await _dbContext.CaseFiles
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var result = caseFiles
            .Select(MapToGetDto)
            .ToList();

        return Ok(result);
    }

    // =========================================
    // GET CASEFILE BY ID
    // GET: api/casefiles/{id}
    // =========================================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var caseFile = await _dbContext.CaseFiles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (caseFile == null)
            return NotFound();

        return Ok(MapToGetDto(caseFile));
    }

    // =========================================
    // UPDATE CASEFILE
    // PUT: api/casefiles/{id}
    // =========================================
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CaseFileUpdateDto dto)
    {
        var caseFile = await _dbContext.CaseFiles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (caseFile == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Stage))
        {
            if (!Enum.TryParse<CaseStage>(dto.Stage, true, out var stage))
                return BadRequest("Invalid case stage");

            caseFile.Stage = stage;
        }

        caseFile.InternalNotes = dto.InternalNotes;

        await _dbContext.SaveChangesAsync();

        return Ok(MapToGetDto(caseFile));
    }

    // =========================================
    // DELETE CASEFILE (ADMIN ONLY)
    // DELETE: api/casefiles/{id}
    // =========================================
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var caseFile = await _dbContext.CaseFiles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (caseFile == null)
            return NotFound();

        _dbContext.CaseFiles.Remove(caseFile);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // =========================================
    // PRIVATE MAPPER
    // =========================================
    private static CaseFileGetDto MapToGetDto(CaseFile caseFile)
    {
        return new CaseFileGetDto
        {
            Id = caseFile.Id,
            CaseNumber = caseFile.CaseNumber,
            LeadId = caseFile.LeadId,
            Stage = caseFile.Stage.ToString(),
            InternalNotes = caseFile.InternalNotes,
            CreatedAt = caseFile.CreatedAt
        };
    }
}
