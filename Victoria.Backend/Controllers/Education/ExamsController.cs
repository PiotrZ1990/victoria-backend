using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Education;
using Victoria.Domain.Entities.Education;
using Victoria.Domain.Enums;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Education;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class ExamsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExamsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/exams
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Exams
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ExamListDto
            {
                Id = x.Id,
                Name = x.Name,
                ExamType = x.ExamType.ToString(),
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/exams/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.Exams.FirstOrDefaultAsync(e => e.Id == id);
        if (x == null) return NotFound();

        return Ok(new ExamDetailsDto
        {
            Id = x.Id,
            Name = x.Name,
            ExamType = x.ExamType.ToString(),
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        });
    }

    // POST: api/exams
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExamCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required");

        if (!Enum.TryParse<ExamType>(dto.ExamType, true, out var type))
            return BadRequest("Invalid ExamType");

        var entity = new Exam
        {
            Name = dto.Name.Trim(),
            ExamType = type,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Exams.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/exams/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ExamUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required");

        if (!Enum.TryParse<ExamType>(dto.ExamType, true, out var type))
            return BadRequest("Invalid ExamType");

        var entity = await _db.Exams.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null) return NotFound();

        entity.Name = dto.Name.Trim();
        entity.ExamType = type;
        entity.Description = dto.Description;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/exams/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Exams.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null) return NotFound();

        _db.Exams.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
