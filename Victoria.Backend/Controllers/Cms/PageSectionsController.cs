using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Cms;
using Victoria.Domain.Entities.Cms;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Cms;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // docelowo Staff/Admin
public class PageSectionsController : ControllerBase
{
    private readonly AppDbContext _db;
    public PageSectionsController(AppDbContext db) { _db = db; }

    // GET: api/pagesections/page/5
    [HttpGet("page/{pageId:int}")]
    public async Task<IActionResult> GetByPage(int pageId)
    {
        var pageExists = await _db.Pages.AnyAsync(x => x.Id == pageId);
        if (!pageExists) return NotFound("Page not found");

        var list = await _db.PageSections
            .Where(x => x.PageId == pageId)
            .OrderBy(x => x.Order)
            .Select(x => new PageSectionListDto
            {
                Id = x.Id,
                PageId = x.PageId,
                SectionKey = x.SectionKey,
                Title = x.Title,
                Order = x.Order,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/pagesections/10
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.PageSections.FirstOrDefaultAsync(s => s.Id == id);
        if (x == null) return NotFound();

        return Ok(new PageSectionDetailsDto
        {
            Id = x.Id,
            PageId = x.PageId,
            SectionKey = x.SectionKey,
            Title = x.Title,
            Content = x.Content,
            Order = x.Order,
            UpdatedAt = x.UpdatedAt
        });
    }

    // POST: api/pagesections
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PageSectionCreateDto dto)
    {
        var pageExists = await _db.Pages.AnyAsync(x => x.Id == dto.PageId);
        if (!pageExists) return BadRequest("Page not found");

        if (string.IsNullOrWhiteSpace(dto.SectionKey))
            return BadRequest("SectionKey is required");

        var entity = new PageSection
        {
            PageId = dto.PageId,
            SectionKey = dto.SectionKey.Trim(),
            Title = dto.Title,
            Content = dto.Content,
            Order = dto.Order,
            UpdatedAt = DateTime.UtcNow
        };

        _db.PageSections.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/pagesections/10
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PageSectionUpdateDto dto)
    {
        var entity = await _db.PageSections.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.SectionKey))
            return BadRequest("SectionKey is required");

        entity.SectionKey = dto.SectionKey.Trim();
        entity.Title = dto.Title;
        entity.Content = dto.Content;
        entity.Order = dto.Order;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/pagesections/10
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.PageSections.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.PageSections.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
