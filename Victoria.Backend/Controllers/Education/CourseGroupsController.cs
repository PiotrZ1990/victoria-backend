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
public class CourseGroupsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CourseGroupsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/coursegroups
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.CourseGroups
            .Include(x => x.LanguageCourse)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new CourseGroupListDto
            {
                Id = x.Id,
                LanguageCourseId = x.LanguageCourseId,
                CourseName = x.LanguageCourse!.Name,
                GroupName = x.GroupName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Capacity = x.Capacity,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/coursegroups/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.CourseGroups
            .Include(g => g.LanguageCourse)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (x == null) return NotFound();

        return Ok(new CourseGroupDetailsDto
        {
            Id = x.Id,
            LanguageCourseId = x.LanguageCourseId,
            CourseName = x.LanguageCourse!.Name,
            GroupName = x.GroupName,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            Capacity = x.Capacity,
            Location = x.Location,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        });
    }

    // POST: api/coursegroups
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CourseGroupCreateDto dto)
    {
        var courseExists = await _db.LanguageCourses.AnyAsync(x => x.Id == dto.LanguageCourseId);
        if (!courseExists) return BadRequest("LanguageCourse not found");

        if (dto.EndDate < dto.StartDate) return BadRequest("EndDate must be >= StartDate");
        if (dto.Capacity < 1) return BadRequest("Capacity must be >= 1");

        var entity = new CourseGroup
        {
            LanguageCourseId = dto.LanguageCourseId,
            GroupName = dto.GroupName,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Capacity = dto.Capacity,
            Location = dto.Location,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.CourseGroups.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/coursegroups/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CourseGroupUpdateDto dto)
    {
        var entity = await _db.CourseGroups.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        var courseExists = await _db.LanguageCourses.AnyAsync(x => x.Id == dto.LanguageCourseId);
        if (!courseExists) return BadRequest("LanguageCourse not found");

        if (dto.EndDate < dto.StartDate) return BadRequest("EndDate must be >= StartDate");
        if (dto.Capacity < 1) return BadRequest("Capacity must be >= 1");

        entity.LanguageCourseId = dto.LanguageCourseId;
        entity.GroupName = dto.GroupName;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.Capacity = dto.Capacity;
        entity.Location = dto.Location;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/coursegroups/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.CourseGroups.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.CourseGroups.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
    // GET: api/coursegroups/by-course/{courseId}
    [HttpGet("by-course/{courseId:int}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var list = await _db.CourseGroups
            .Where(x => x.LanguageCourseId == courseId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.LanguageCourseId,
                x.GroupName,
                x.StartDate,
                x.EndDate,
                x.Capacity,
                x.IsActive,
                x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

}
