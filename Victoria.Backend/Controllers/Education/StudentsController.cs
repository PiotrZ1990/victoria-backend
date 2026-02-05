using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Education;
using Victoria.Domain.Entities.Education;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Education;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudentsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/students
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Students
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new StudentListDto
            {
                Id = x.Id,
                FullName = x.FirstName + " " + x.LastName,
                Email = x.Email,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/students/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (x == null) return NotFound();

        return Ok(new StudentDetailsDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Phone = x.Phone,
            Nationality = x.Nationality,
            DateOfBirth = x.DateOfBirth,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        });
    }

    // POST: api/students
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            return BadRequest("FirstName and LastName are required");

        var entity = new Student
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email,
            Phone = dto.Phone,
            Nationality = dto.Nationality,
            DateOfBirth = dto.DateOfBirth,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Students.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/students/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] StudentUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            return BadRequest("FirstName and LastName are required");

        var entity = await _db.Students.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.FirstName = dto.FirstName.Trim();
        entity.LastName = dto.LastName.Trim();
        entity.Email = dto.Email;
        entity.Phone = dto.Phone;
        entity.Nationality = dto.Nationality;
        entity.DateOfBirth = dto.DateOfBirth;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/students/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Students.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.Students.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
