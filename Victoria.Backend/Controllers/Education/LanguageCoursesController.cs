using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Education;
using Victoria.Domain.Entities.Education;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Education;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // docelowo Staff/Admin
public class LanguageCoursesController : ControllerBase
{
    private readonly AppDbContext _db;

    public LanguageCoursesController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/languagecourses
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.LanguageCourses
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new LanguageCourseListDto
            {
                Id = x.Id,
                Name = x.Name,
                Language = x.Language,
                Level = x.Level,
                Price = x.Price,
                Currency = x.Currency,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/languagecourses/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.LanguageCourses.FirstOrDefaultAsync(a => a.Id == id);
        if (x == null) return NotFound();

        return Ok(new LanguageCourseDetailsDto
        {
            Id = x.Id,
            Name = x.Name,
            Language = x.Language,
            Level = x.Level,
            Description = x.Description,
            Price = x.Price,
            Currency = x.Currency,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });
    }

    // POST: api/languagecourses
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LanguageCourseCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required");
        if (string.IsNullOrWhiteSpace(dto.Language)) return BadRequest("Language is required");
        if (string.IsNullOrWhiteSpace(dto.Level)) return BadRequest("Level is required");
        if (dto.Price < 0) return BadRequest("Price must be >= 0");

        var entity = new LanguageCourse
        {
            Name = dto.Name.Trim(),
            Language = dto.Language.Trim(),
            Level = dto.Level.Trim(),
            Description = dto.Description,
            Price = dto.Price,
            Currency = string.IsNullOrWhiteSpace(dto.Currency) ? "GBP" : dto.Currency.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _db.LanguageCourses.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/languagecourses/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] LanguageCourseUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required");
        if (string.IsNullOrWhiteSpace(dto.Language)) return BadRequest("Language is required");
        if (string.IsNullOrWhiteSpace(dto.Level)) return BadRequest("Level is required");
        if (dto.Price < 0) return BadRequest("Price must be >= 0");

        var entity = await _db.LanguageCourses.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.Name = dto.Name.Trim();
        entity.Language = dto.Language.Trim();
        entity.Level = dto.Level.Trim();
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.Currency = string.IsNullOrWhiteSpace(dto.Currency) ? "GBP" : dto.Currency.Trim();
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/languagecourses/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.LanguageCourses.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.LanguageCourses.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
