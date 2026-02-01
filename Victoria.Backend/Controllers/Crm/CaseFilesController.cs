using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Victoria.Backend.DTOs.Cases;
using Victoria.Backend.DTOs.Crm;
using Victoria.Domain.Entities.Cases;
using Victoria.Domain.Enums;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Crm;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
//[Authorize(Roles = "Staff,Admin")]
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var lead = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == dto.LeadId);

        if (lead == null)
            return NotFound("Lead not found");

        if (lead.Status == "Converted")
            return BadRequest("Lead already converted");

        var caseFile = new CaseFile
        {
            ClientName = lead.FullName,
            LeadId = lead.Id,
            CaseNumber = $"CASE-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Stage = CaseStage.New,
            InternalNotes = dto.InternalNotes,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.CaseFiles.Add(caseFile);

        lead.Status = "Converted";

        await _dbContext.SaveChangesAsync();

        // ===============================
        // AUTO-GENERATE CHECKLIST ITEMS FOR NEW CASE
        // ===============================

        // 1) Application checklist items (dla aplikacji na studia)
        var appTemplates = await _dbContext.ApplicationDocumentChecklists
            .ToListAsync();

        foreach (var t in appTemplates)
        {
            _dbContext.CaseApplicationChecklistItems.Add(new()
            {
                CaseFileId = caseFile.Id,
                ChecklistId = t.Id,
                IsCompleted = false
            });
        }

        // 2) Visa checklist items (dla wizy)
        var visaTemplates = await _dbContext.VisaDocumentChecklists
            .ToListAsync();

        foreach (var t in visaTemplates)
        {
            _dbContext.CaseVisaChecklistItems.Add(new()
            {
                CaseFileId = caseFile.Id,
                ChecklistId = t.Id,
                IsCompleted = false
            });
        }

        // zapisujemy nowe checklist itemy
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

        return Ok(caseFiles.Select(MapToGetDto).ToList());
    }

    // =========================================
    // GET CASEFILE BY ID
    // GET: api/casefiles/{id}
    // =========================================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var caseFile = await _dbContext.CaseFiles.FirstOrDefaultAsync(x => x.Id == id);

        if (caseFile == null)
            return NotFound();

        return Ok(MapToGetDto(caseFile));
    }

    // =========================================
    // UPDATE CASEFILE (NOTES ONLY)
    // PUT: api/casefiles/{id}
    // =========================================
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CaseFileUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var caseFile = await _dbContext.CaseFiles.FirstOrDefaultAsync(x => x.Id == id);

        if (caseFile == null)
            return NotFound();

        // Stage NIE ruszamy tutaj – stage ma osobny endpoint /stage z regułami biznesowymi
        caseFile.InternalNotes = dto.InternalNotes;

        await _dbContext.SaveChangesAsync();

        return Ok(MapToGetDto(caseFile));
    }

    // =========================================
    // READINESS SUMMARY
    // GET: api/casefiles/{id}/readiness
    // =========================================
    [HttpGet("{id:int}/readiness")]
    public async Task<IActionResult> GetReadiness(int id)
    {
        var caseFile = await _dbContext.CaseFiles.FirstOrDefaultAsync(x => x.Id == id);
        if (caseFile == null)
            return NotFound("CaseFile not found");

        // =========================
        // APPLICATION
        // =========================
        var requiredApp = await _dbContext.ApplicationDocumentChecklists
            .CountAsync(x => x.IsRequired);

        var completedApp = await _dbContext.CaseApplicationChecklistItems
            .Include(x => x.Checklist)
            .Where(x => x.CaseFileId == id && x.Checklist.IsRequired && x.IsCompleted)
            .CountAsync();

        // =========================
        // VISA
        // =========================
        var requiredVisa = await _dbContext.VisaDocumentChecklists
            .CountAsync(x => x.IsRequired);

        var completedVisa = await _dbContext.CaseVisaChecklistItems
            .Include(x => x.Checklist)
            .Where(x => x.CaseFileId == id && x.Checklist.IsRequired && x.IsCompleted)
            .CountAsync();

        // =========================
        // ACCOMMODATION
        // =========================
        var requiredAcc = await _dbContext.AccommodationDocumentChecklists
            .CountAsync(x => x.IsRequired);

        var completedAcc = await _dbContext.CaseAccommodationChecklistItems
            .Include(x => x.Checklist)
            .Where(x => x.CaseFileId == id && x.Checklist.IsRequired && x.IsCompleted)
            .CountAsync();

        // =========================
        // BUSINESS RULES
        // =========================
        var canMoveToApplication = requiredApp > 0 && completedApp >= requiredApp;

        var canMoveToVisa = canMoveToApplication
                            && requiredVisa > 0
                            && completedVisa >= requiredVisa;

        var canMoveToAccommodation = canMoveToVisa
                                     && requiredAcc > 0
                                     && completedAcc >= requiredAcc;

        var canComplete = canMoveToAccommodation;

        // =========================
        // RESULT DTO
        // =========================
        var dto = new CaseReadinessDto
        {
            CaseFileId = caseFile.Id,
            Stage = caseFile.Stage.ToString(),

            RequiredApplicationItems = requiredApp,
            CompletedApplicationItems = completedApp,

            RequiredVisaItems = requiredVisa,
            CompletedVisaItems = completedVisa,

            RequiredAccommodationItems = requiredAcc,
            CompletedAccommodationItems = completedAcc,

            CanMoveToApplication = canMoveToApplication,
            CanMoveToVisa = canMoveToVisa,
            CanMoveToAccommodation = canMoveToAccommodation,
            CanComplete = canComplete
        };

        return Ok(dto);
    }


    // =========================================
    // CHANGE CASE STAGE (BUSINESS RULES)
    // PUT: api/casefiles/{id}/stage
    // =========================================
    [HttpPut("{id:int}/stage")]
    public async Task<IActionResult> ChangeStage(int id, [FromBody] CaseStageUpdateDto dto)
    {
        var caseFile = await _dbContext.CaseFiles.FirstOrDefaultAsync(x => x.Id == id);
        if (caseFile == null)
            return NotFound("CaseFile not found");

        if (!Enum.TryParse<CaseStage>(dto.Stage, true, out var newStage))
            return BadRequest("Invalid stage");

        // =========================
        // APPLICATION
        // =========================
        var requiredApp = await _dbContext.ApplicationDocumentChecklists.CountAsync(x => x.IsRequired);
        var completedApp = await _dbContext.CaseApplicationChecklistItems
            .Include(x => x.Checklist)
            .Where(x => x.CaseFileId == id && x.Checklist.IsRequired && x.IsCompleted)
            .CountAsync();

        // =========================
        // VISA
        // =========================
        var requiredVisa = await _dbContext.VisaDocumentChecklists.CountAsync(x => x.IsRequired);
        var completedVisa = await _dbContext.CaseVisaChecklistItems
            .Include(x => x.Checklist)
            .Where(x => x.CaseFileId == id && x.Checklist.IsRequired && x.IsCompleted)
            .CountAsync();

        // =========================
        // ACCOMMODATION
        // =========================
        var requiredAcc = await _dbContext.AccommodationDocumentChecklists.CountAsync(x => x.IsRequired);
        var completedAcc = await _dbContext.CaseAccommodationChecklistItems
            .Include(x => x.Checklist)
            .Where(x => x.CaseFileId == id && x.Checklist.IsRequired && x.IsCompleted)
            .CountAsync();

        // =========================
        // BUSINESS RULES
        // =========================
        var canMoveToApplication = requiredApp > 0 && completedApp >= requiredApp;
        var canMoveToVisa = canMoveToApplication && requiredVisa > 0 && completedVisa >= requiredVisa;
        var canMoveToAccommodation = canMoveToVisa && requiredAcc > 0 && completedAcc >= requiredAcc;
        var canComplete = canMoveToAccommodation;

        bool allowed = newStage switch
        {
            CaseStage.New => true,
            CaseStage.Planning => true,
            CaseStage.Application => canMoveToApplication,
            CaseStage.Visa => canMoveToVisa,
            CaseStage.Accommodation => canMoveToAccommodation,
            CaseStage.Completed => canComplete,
            CaseStage.Cancelled => true,
            _ => false
        };

        if (!allowed)
        {
            return BadRequest(new
            {
                message = "Stage change not allowed by business rules",
                requestedStage = newStage.ToString(),

                requiredApplicationItems = requiredApp,
                completedApplicationItems = completedApp,

                requiredVisaItems = requiredVisa,
                completedVisaItems = completedVisa,

                requiredAccommodationItems = requiredAcc,
                completedAccommodationItems = completedAcc,

                canMoveToApplication,
                canMoveToVisa,
                canMoveToAccommodation,
                canComplete
            });
        }

        caseFile.Stage = newStage;
        await _dbContext.SaveChangesAsync();

        return Ok(new { caseFile.Id, stage = caseFile.Stage.ToString() });
    }


    // =========================================
    // DELETE CASEFILE (ADMIN ONLY)
    // DELETE: api/casefiles/{id}
    // =========================================
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var caseFile = await _dbContext.CaseFiles.FirstOrDefaultAsync(x => x.Id == id);

        if (caseFile == null)
            return NotFound();

        _dbContext.CaseFiles.Remove(caseFile);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyCases()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var list = await _dbContext.CaseFiles
            .Where(x => x.ClientUserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.Stage,
                x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
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
