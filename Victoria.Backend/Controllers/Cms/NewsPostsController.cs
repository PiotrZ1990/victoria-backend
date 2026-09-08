using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Cms;
using Victoria.Domain.Entities.CMS;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Cms;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NewsPostsController : ControllerBase
{
    private readonly AppDbContext _db;

    public NewsPostsController(AppDbContext db)
    {
        _db = db;
    }

    // =========================================
    // LIST
    // GET: api/newsposts
    // =========================================
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Set<NewsPost>()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new NewsPostListDto
            {
                Id = x.Id,
                Title = x.Title,
                IsPublished = x.IsPublished,
                PublishedAt = x.PublishedAt,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // =========================================
    // LIST PUBLISHED
    // GET: api/newsposts/published
    // =========================================
    [HttpGet("published")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublished()
    {
        var list = await _db.Set<NewsPost>()
            .Where(x => x.IsPublished)
            .OrderByDescending(x => x.PublishedAt ?? x.CreatedAt)
            .Select(x => new NewsPostListDto
            {
                Id = x.Id,
                Title = x.Title,
                IsPublished = x.IsPublished,
                PublishedAt = x.PublishedAt,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // =========================================
    // DETAILS
    // GET: api/newsposts/{id}
    // =========================================
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.Set<NewsPost>().FirstOrDefaultAsync(a => a.Id == id);
        if (x == null) return NotFound();

        return Ok(new NewsPostDetailsDto
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            IsPublished = x.IsPublished,
            PublishedAt = x.PublishedAt,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });
    }

    // =========================================
    // CREATE
    // POST: api/newsposts
    // =========================================
    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create([FromBody] NewsPostCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required");

        if (string.IsNullOrWhiteSpace(dto.Content))
            return BadRequest("Content is required");

        var entity = new NewsPost
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            IsPublished = dto.IsPublished,
            PublishedAt = dto.IsPublished ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow
        };

        _db.Set<NewsPost>().Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // =========================================
    // UPDATE
    // PUT: api/newsposts/{id}
    // =========================================
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Update(int id, [FromBody] NewsPostUpdateDto dto)
    {
        var entity = await _db.Set<NewsPost>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required");

        if (string.IsNullOrWhiteSpace(dto.Content))
            return BadRequest("Content is required");

        var wasPublished = entity.IsPublished;

        entity.Title = dto.Title.Trim();
        entity.Content = dto.Content.Trim();
        entity.IsPublished = dto.IsPublished;
        entity.UpdatedAt = DateTime.UtcNow;

        // jeśli publikujemy pierwszy raz -> ustaw PublishedAt
        if (!wasPublished && dto.IsPublished)
            entity.PublishedAt = DateTime.UtcNow;

        // jeśli cofamy publikację -> wyczyść PublishedAt (opcjonalnie)
        if (wasPublished && !dto.IsPublished)
            entity.PublishedAt = null;

        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // =========================================
    // DELETE
    // DELETE: api/newsposts/{id}
    // =========================================
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Set<NewsPost>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.Set<NewsPost>().Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
