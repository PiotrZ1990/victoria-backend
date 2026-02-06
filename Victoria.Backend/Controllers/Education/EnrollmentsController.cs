using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Education;
using Victoria.Domain.Entities.Education;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Education;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public EnrollmentsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/enrollments
    [HttpGet]
    [Authorize(Roles = "Admin,Staff,Student")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Enrollments
            .Include(x => x.Student)
            .Include(x => x.CourseGroup).ThenInclude(g => g.LanguageCourse)
            .OrderByDescending(x => x.EnrolledAt)
            .Select(x => new EnrollmentListDto
            {
                Id = x.Id,
                StudentId = x.StudentId,
                StudentName = (x.Student!.FirstName + " " + x.Student!.LastName).Trim(),
                CourseGroupId = x.CourseGroupId,
                CourseName = x.CourseGroup!.LanguageCourse!.Name,
                GroupName = x.CourseGroup!.GroupName,
                Status = x.Status,
                EnrolledAt = x.EnrolledAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/enrollments/{id}
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Staff,Student")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.CourseGroup).ThenInclude(g => g.LanguageCourse)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (x == null) return NotFound();

        return Ok(new EnrollmentDetailsDto
        {
            Id = x.Id,
            StudentId = x.StudentId,
            StudentName = (x.Student!.FirstName + " " + x.Student!.LastName).Trim(),
            CourseGroupId = x.CourseGroupId,
            CourseName = x.CourseGroup!.LanguageCourse!.Name,
            GroupName = x.CourseGroup.GroupName,
            EnrolledAt = x.EnrolledAt,
            Status = x.Status,
            Notes = x.Notes
        });
    }

    // POST: api/enrollments
    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create([FromBody] EnrollmentCreateDto dto)
    {
        var studentExists = await _db.Students.AnyAsync(s => s.Id == dto.StudentId);
        if (!studentExists) return BadRequest("Student not found");

        var groupExists = await _db.CourseGroups.AnyAsync(g => g.Id == dto.CourseGroupId);
        if (!groupExists) return BadRequest("CourseGroup not found");

        // pilnujemy unikalności StudentId+CourseGroupId (masz Unique Index)
        var already = await _db.Enrollments.AnyAsync(e =>
            e.StudentId == dto.StudentId && e.CourseGroupId == dto.CourseGroupId);

        if (already) return BadRequest("This student is already enrolled in this group.");

        var entity = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseGroupId = dto.CourseGroupId,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status,
            Notes = dto.Notes,
            EnrolledAt = DateTime.UtcNow
        };

        _db.Enrollments.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/enrollments/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Update(int id, [FromBody] EnrollmentUpdateDto dto)
    {
        var entity = await _db.Enrollments.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.Status = string.IsNullOrWhiteSpace(dto.Status) ? entity.Status : dto.Status;
        entity.Notes = dto.Notes;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/enrollments/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Enrollments.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.Enrollments.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
