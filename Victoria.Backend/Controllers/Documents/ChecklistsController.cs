using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Documents;
using Victoria.Domain.Entities.Documents;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Documents;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[Authorize(Roles = "Admin,Staff")]
public class ChecklistsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ChecklistsController(AppDbContext db)
    {
        _db = db;
    }

    // =========================================
    // INIT APPLICATION CHECKLIST FOR CASEFILE
    // POST: api/checklists/init-application
    // =========================================
    [HttpPost("init-application")]
    public async Task<IActionResult> InitApplication([FromBody] ChecklistInitDto dto)
    {
        var caseExists = await _db.CaseFiles.AnyAsync(x => x.Id == dto.CaseFileId);
        if (!caseExists) return NotFound("CaseFile not found");

        var templates = await _db.Set<ApplicationDocumentChecklist>()
            .OrderBy(x => x.Id)
            .ToListAsync();

        if (templates.Count == 0)
            return BadRequest("No Application checklist templates found");

        var existingChecklistIds = await _db.CaseApplicationChecklistItems
            .Where(x => x.CaseFileId == dto.CaseFileId)
            .Select(x => x.ChecklistId)
            .ToListAsync();

        var toAdd = templates
            .Where(t => !existingChecklistIds.Contains(t.Id))
            .Select(t => new CaseApplicationChecklistItem
            {
                CaseFileId = dto.CaseFileId,
                ChecklistId = t.Id,
                IsCompleted = false,
                DocumentId = null,
                UpdatedAt = DateTime.UtcNow
            })
            .ToList();

        _db.CaseApplicationChecklistItems.AddRange(toAdd);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            dto.CaseFileId,
            Added = toAdd.Count,
            TotalTemplates = templates.Count
        });
    }

    // =========================================
    // INIT VISA CHECKLIST FOR CASEFILE
    // POST: api/checklists/init-visa
    // =========================================
    [HttpPost("init-visa")]
    public async Task<IActionResult> InitVisa([FromBody] ChecklistInitDto dto)
    {
        var caseExists = await _db.CaseFiles.AnyAsync(x => x.Id == dto.CaseFileId);
        if (!caseExists) return NotFound("CaseFile not found");

        var templates = await _db.Set<VisaDocumentChecklist>()
            .OrderBy(x => x.Id)
            .ToListAsync();

        if (templates.Count == 0)
            return BadRequest("No Visa checklist templates found");

        var existingChecklistIds = await _db.CaseVisaChecklistItems
            .Where(x => x.CaseFileId == dto.CaseFileId)
            .Select(x => x.ChecklistId)
            .ToListAsync();

        var toAdd = templates
            .Where(t => !existingChecklistIds.Contains(t.Id))
            .Select(t => new CaseVisaChecklistItem
            {
                CaseFileId = dto.CaseFileId,
                ChecklistId = t.Id,
                IsCompleted = false,
                DocumentId = null,
                UpdatedAt = DateTime.UtcNow
            })
            .ToList();

        _db.CaseVisaChecklistItems.AddRange(toAdd);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            dto.CaseFileId,
            Added = toAdd.Count,
            TotalTemplates = templates.Count
        });
    }

    // =========================================
    // GET APPLICATION CHECKLIST FOR CASEFILE
    // GET: api/checklists/application/{caseFileId}
    // =========================================
    [HttpGet("application/{caseFileId:int}")]
    public async Task<IActionResult> GetApplication(int caseFileId)
    {
        var items = await _db.CaseApplicationChecklistItems
            .Where(x => x.CaseFileId == caseFileId)
            .Include(x => x.Checklist)
            .OrderBy(x => x.ChecklistId)
            .Select(x => new ChecklistItemGetDto
            {
                ItemId = x.Id,
                ChecklistId = x.ChecklistId,
                DocumentType = x.Checklist.DocumentType,
                IsRequired = x.Checklist.IsRequired,
                IsCompleted = x.IsCompleted,
                DocumentId = x.DocumentId,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        return Ok(items);
    }

    // =========================================
    // GET VISA CHECKLIST FOR CASEFILE
    // GET: api/checklists/visa/{caseFileId}
    // =========================================
    [HttpGet("visa/{caseFileId:int}")]
    public async Task<IActionResult> GetVisa(int caseFileId)
    {
        var items = await _db.CaseVisaChecklistItems
            .Where(x => x.CaseFileId == caseFileId)
            .Include(x => x.Checklist)
            .OrderBy(x => x.ChecklistId)
            .Select(x => new ChecklistItemGetDto
            {
                ItemId = x.Id,
                ChecklistId = x.ChecklistId,
                DocumentType = x.Checklist.DocumentType,
                IsRequired = x.Checklist.IsRequired,
                IsCompleted = x.IsCompleted,
                DocumentId = x.DocumentId,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        return Ok(items);
    }

    // =========================================
    // UPDATE APPLICATION ITEM
    // PUT: api/checklists/application/items/{itemId}
    // =========================================
    [HttpPut("application/items/{itemId:int}")]
    public async Task<IActionResult> UpdateApplicationItem(int itemId, [FromBody] ChecklistItemUpdateDto dto)
    {
        var item = await _db.CaseApplicationChecklistItems
            .FirstOrDefaultAsync(x => x.Id == itemId);

        if (item == null)
            return NotFound("Checklist item not found");

        item.IsCompleted = dto.IsCompleted;

        if (dto.DocumentId.HasValue)
            item.DocumentId = dto.DocumentId;

        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { item.Id, item.CaseFileId, item.IsCompleted, item.DocumentId, item.UpdatedAt });
    }

    // =========================================
    // UPDATE VISA ITEM
    // PUT: api/checklists/visa/items/{itemId}
    // =========================================
    [HttpPut("visa/items/{itemId:int}")]
    public async Task<IActionResult> UpdateVisaItem(int itemId, [FromBody] ChecklistItemUpdateDto dto)
    {
        var item = await _db.CaseVisaChecklistItems
            .FirstOrDefaultAsync(x => x.Id == itemId);

        if (item == null)
            return NotFound("Checklist item not found");

        item.IsCompleted = dto.IsCompleted;

        if (dto.DocumentId.HasValue)
            item.DocumentId = dto.DocumentId;

        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { item.Id, item.CaseFileId, item.IsCompleted, item.DocumentId, item.UpdatedAt });
    }
}
