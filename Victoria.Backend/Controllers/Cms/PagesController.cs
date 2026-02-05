using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Cms;
using Victoria.Domain.Entities.Cms;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Cms;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class PagesController : ControllerBase
{
    private readonly AppDbContext _db;

    public PagesController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/pages
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Pages
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PageListDto
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                IsPublished = x.IsPublished,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/pages/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.Pages.FirstOrDefaultAsync(p => p.Id == id);
        if (x == null) return NotFound();

        return Ok(new PageDetailsDto
        {
            Id = x.Id,
            Title = x.Title,
            Slug = x.Slug,
            IsPublished = x.IsPublished,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });
    }

    // POST: api/pages
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PageCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required");

        if (string.IsNullOrWhiteSpace(dto.Slug))
            return BadRequest("Slug is required");

        var slug = dto.Slug.Trim().ToLowerInvariant();

        var exists = await _db.Pages.AnyAsync(x => x.Slug == slug);
        if (exists) return BadRequest("Slug must be unique");

        var entity = new Page
        {
            Title = dto.Title.Trim(),
            Slug = slug,
            IsPublished = dto.IsPublished,
            CreatedAt = DateTime.UtcNow
        };

        _db.Pages.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/pages/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PageUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required");

        if (string.IsNullOrWhiteSpace(dto.Slug))
            return BadRequest("Slug is required");

        var entity = await _db.Pages.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        var slug = dto.Slug.Trim().ToLowerInvariant();

        var slugTaken = await _db.Pages.AnyAsync(x => x.Id != id && x.Slug == slug);
        if (slugTaken) return BadRequest("Slug must be unique");

        entity.Title = dto.Title.Trim();
        entity.Slug = slug;
        entity.IsPublished = dto.IsPublished;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/pages/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Pages.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.Pages.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
