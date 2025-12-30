using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Documents;
using Victoria.Domain.Entities.Documents;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Documents;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Staff,Admin")]
[AllowAnonymous]
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
        // 1) sprawdź czy CaseFile istnieje
        var caseExists = await _db.CaseFiles.AnyAsync(x => x.Id == dto.CaseFileId);
        if (!caseExists)
            return NotFound("CaseFile not found");

        // 2) pobierz wszystkie pozycje checklisty-szablonu
        var templates = await _db.Set<ApplicationDocumentChecklist>()
            .OrderBy(x => x.Id)
            .ToListAsync();

        if (templates.Count == 0)
            return BadRequest("No Application checklist templates found");

        // 3) sprawdź co już istnieje (żeby init był idempotentny)
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
            CaseFileId = dto.CaseFileId,
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
        if (!caseExists)
            return NotFound("CaseFile not found");

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
            CaseFileId = dto.CaseFileId,
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
    // UPDATE APPLICATION CHECKLIST ITEM
    // PATCH: api/checklists/application/item/{itemId}
    // =========================================
    [HttpPatch("application/item/{itemId:int}")]
    public async Task<IActionResult> UpdateApplicationItem(int itemId, [FromBody] ChecklistItemUpdateDto dto)
    {
        try
        {
            var item = await _db.CaseApplicationChecklistItems
                .FirstOrDefaultAsync(x => x.Id == itemId);

            if (item == null)
                return NotFound("Checklist item not found");

            item.IsCompleted = dto.IsCompleted;
            item.DocumentId = dto.DocumentId;
            item.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { item.Id, item.IsCompleted, item.DocumentId, item.UpdatedAt });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                error = ex.Message,
                inner = ex.InnerException?.Message,
                stack = ex.StackTrace
            });
        }
    }


    // =========================================
    // UPDATE VISA CHECKLIST ITEM
    // PATCH: api/checklists/visa/item/{itemId}
    // =========================================
    [HttpPatch("visa/item/{itemId:int}")]
    public async Task<IActionResult> UpdateVisaItem(int itemId, [FromBody] ChecklistItemUpdateDto dto)
    {
        var item = await _db.CaseVisaChecklistItems
            .FirstOrDefaultAsync(x => x.Id == itemId);

        if (item == null)
            return NotFound("Checklist item not found");

        item.IsCompleted = dto.IsCompleted;
        item.DocumentId = dto.DocumentId;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { item.Id, item.IsCompleted, item.DocumentId, item.UpdatedAt });
    }

    [HttpPost("seed-visa-items/{caseFileId:int}")]
    public async Task<IActionResult> SeedVisaItemsForCase(int caseFileId)
    {
        var caseFile = await _db.CaseFiles.FirstOrDefaultAsync(x => x.Id == caseFileId);
        if (caseFile == null) return NotFound("CaseFile not found");

        var templates = await _db.VisaDocumentChecklists.ToListAsync();
        if (!templates.Any()) return BadRequest("No Visa checklist templates found");

        var already = await _db.CaseVisaChecklistItems.AnyAsync(x => x.CaseFileId == caseFileId);
        if (already) return Ok("Already seeded");

        foreach (var t in templates)
        {
            _db.CaseVisaChecklistItems.Add(new CaseVisaChecklistItem
            {
                CaseFileId = caseFileId,
                ChecklistId = t.Id,
                IsCompleted = false
            });
        }

        await _db.SaveChangesAsync();
        return Ok("Seeded");
    }


}
