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
public class ExamResultsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExamResultsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/examresults
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.ExamResults
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ExamResultListDto
            {
                Id = x.Id,
                ExamSessionId = x.ExamSessionId,
                StudentId = x.StudentId,
                Score = x.Score,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/examresults/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.ExamResults.FirstOrDefaultAsync(e => e.Id == id);
        if (x == null) return NotFound();

        return Ok(new ExamResultDetailsDto
        {
            Id = x.Id,
            ExamSessionId = x.ExamSessionId,
            StudentId = x.StudentId,
            Score = x.Score,
            Notes = x.Notes,
            CreatedAt = x.CreatedAt
        });
    }

    // POST: api/examresults
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExamResultCreateDto dto)
    {
        // walidacja FK
        var sessionExists = await _db.ExamSessions.AnyAsync(x => x.Id == dto.ExamSessionId);
        if (!sessionExists) return BadRequest("ExamSession not found");

        var studentExists = await _db.Students.AnyAsync(x => x.Id == dto.StudentId);
        if (!studentExists) return BadRequest("Student not found");

        var entity = new ExamResult
        {
            ExamSessionId = dto.ExamSessionId,
            StudentId = dto.StudentId,
            Score = dto.Score,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _db.ExamResults.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/examresults/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ExamResultUpdateDto dto)
    {
        var entity = await _db.ExamResults.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        // FK (opcjonalnie: możesz pozwolić nie zmieniać, ale tu wspieramy zmianę)
        var sessionExists = await _db.ExamSessions.AnyAsync(x => x.Id == dto.ExamSessionId);
        if (!sessionExists) return BadRequest("ExamSession not found");

        var studentExists = await _db.Students.AnyAsync(x => x.Id == dto.StudentId);
        if (!studentExists) return BadRequest("Student not found");

        entity.ExamSessionId = dto.ExamSessionId;
        entity.StudentId = dto.StudentId;
        entity.Score = dto.Score;
        entity.Notes = dto.Notes;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/examresults/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ExamResults.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.ExamResults.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
