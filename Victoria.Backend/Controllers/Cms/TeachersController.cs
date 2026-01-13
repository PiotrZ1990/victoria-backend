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
public class TeachersController : ControllerBase
{
    private readonly AppDbContext _db;

    public TeachersController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/teachers
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Teachers
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new TeacherListDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Title = x.Title,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/teachers/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.Teachers.FirstOrDefaultAsync(t => t.Id == id);
        if (x == null) return NotFound();

        return Ok(new TeacherDetailsDto
        {
            Id = x.Id,
            FullName = x.FullName,
            Title = x.Title,
            Email = x.Email,
            Phone = x.Phone,
            Bio = x.Bio,
            PhotoUrl = x.PhotoUrl,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        });
    }

    // POST: api/teachers
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TeacherCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
            return BadRequest("FullName is required");

        var entity = new Teacher
        {
            FullName = dto.FullName,
            Title = dto.Title,
            Email = dto.Email,
            Phone = dto.Phone,
            Bio = dto.Bio,
            PhotoUrl = dto.PhotoUrl,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Teachers.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/teachers/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TeacherUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
            return BadRequest("FullName is required");

        var entity = await _db.Teachers.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.FullName = dto.FullName;
        entity.Title = dto.Title;
        entity.Email = dto.Email;
        entity.Phone = dto.Phone;
        entity.Bio = dto.Bio;
        entity.PhotoUrl = dto.PhotoUrl;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/teachers/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Teachers.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.Teachers.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
