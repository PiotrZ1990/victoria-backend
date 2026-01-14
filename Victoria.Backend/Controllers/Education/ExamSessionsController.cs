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
public class ExamSessionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExamSessionsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/examsessions
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.ExamSessions
            .Include(x => x.Exam)
            .OrderByDescending(x => x.SessionDate)
            .Select(x => new ExamSessionListDto
            {
                Id = x.Id,
                ExamId = x.ExamId,
                ExamName = x.Exam.Name,
                SessionDate = x.SessionDate,
                Location = x.Location,
                Capacity = x.Capacity,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/examsessions/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.ExamSessions
            .Include(s => s.Exam)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (x == null) return NotFound();

        return Ok(new ExamSessionDetailsDto
        {
            Id = x.Id,
            ExamId = x.ExamId,
            ExamName = x.Exam.Name,
            SessionDate = x.SessionDate,
            Location = x.Location,
            Capacity = x.Capacity,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        });
    }

    // POST: api/examsessions
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExamSessionCreateDto dto)
    {
        var examExists = await _db.Exams.AnyAsync(e => e.Id == dto.ExamId);
        if (!examExists) return BadRequest("Exam not found");

        if (dto.Capacity < 0) return BadRequest("Capacity must be >= 0");

        var entity = new ExamSession
        {
            ExamId = dto.ExamId,
            SessionDate = dto.SessionDate,
            Location = dto.Location,
            Capacity = dto.Capacity,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.ExamSessions.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/examsessions/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ExamSessionUpdateDto dto)
    {
        var entity = await _db.ExamSessions.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        var examExists = await _db.Exams.AnyAsync(e => e.Id == dto.ExamId);
        if (!examExists) return BadRequest("Exam not found");

        if (dto.Capacity < 0) return BadRequest("Capacity must be >= 0");

        entity.ExamId = dto.ExamId;
        entity.SessionDate = dto.SessionDate;
        entity.Location = dto.Location;
        entity.Capacity = dto.Capacity;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/examsessions/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ExamSessions.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.ExamSessions.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup()
    {
        var list = await _db.ExamSessions
            .Include(x => x.Exam)
            .Where(x => x.IsActive)
            .OrderBy(x => x.SessionDate)
            .Select(x => new
            {
                id = x.Id,
                display =
                    x.Exam.Name + " | " +
                    x.SessionDate.ToString("yyyy-MM-dd HH:mm") +
                    (x.Location != null ? " | " + x.Location : "") +
                    " | capacity: " + x.Capacity
            })
            .ToListAsync();

        return Ok(list);
    }

}
