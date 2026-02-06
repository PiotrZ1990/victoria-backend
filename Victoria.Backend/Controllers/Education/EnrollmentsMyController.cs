using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Domain.Entities.Education;
using Victoria.Infrastructure.Data;
using Victoria.Infrastructure.Identity;

namespace Victoria.Backend.Controllers.Education;

[ApiController]
[Route("api/enrollments")]
[Authorize] // klient musi być zalogowany
public class EnrollmentsMyController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public EnrollmentsMyController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // GET: api/enrollments/my
    [HttpGet("my")]
    [Authorize(Roles = "Admin,Staff,Student")]
    public async Task<IActionResult> GetMy()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        if (!user.StudentId.HasValue)
            return BadRequest("This user is not linked to Student (StudentId is NULL).");

        var studentId = user.StudentId.Value;

        var list = await _db.Enrollments
            .Where(e => e.StudentId == studentId)
            .Include(e => e.CourseGroup)
                .ThenInclude(g => g.LanguageCourse)
            .OrderByDescending(e => e.EnrolledAt)
            .Select(e => new
            {
                e.Id,
                e.StudentId,
                e.CourseGroupId,
                CourseName = e.CourseGroup.LanguageCourse.Name,
                GroupName = e.CourseGroup.GroupName,
                e.CourseGroup.StartDate,
                e.CourseGroup.EndDate,
                e.CourseGroup.Location,
                e.EnrolledAt
            })
            .ToListAsync();

        return Ok(list);
    }

    public class EnrollMyRequest
    {
        public int CourseGroupId { get; set; }
    }

    // POST: api/enrollments/my
    [HttpPost("my")]
    [Authorize(Roles = "Admin,Staff,Student")]
    public async Task<IActionResult> EnrollMy([FromBody] EnrollMyRequest req)
    {
        if (req.CourseGroupId <= 0)
            return BadRequest("CourseGroupId is required.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        if (!user.StudentId.HasValue)
            return BadRequest("This user is not linked to Student (StudentId is NULL).");

        var studentId = user.StudentId.Value;

        var group = await _db.CourseGroups
            .Include(g => g.LanguageCourse)
            .FirstOrDefaultAsync(g => g.Id == req.CourseGroupId);

        if (group == null)
            return NotFound("CourseGroup not found.");

        if (!group.IsActive)
            return BadRequest("CourseGroup is not active.");

        // capacity check
        var enrolledCount = await _db.Enrollments.CountAsync(e => e.CourseGroupId == group.Id);
        if (enrolledCount >= group.Capacity)
            return BadRequest("CourseGroup is full (capacity reached).");

        // unique (StudentId, CourseGroupId) masz w indeksie, ale damy czytelny błąd:
        var exists = await _db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseGroupId == group.Id);
        if (exists)
            return BadRequest("Already enrolled in this CourseGroup.");

        var entity = new Enrollment
        {
            StudentId = studentId,
            CourseGroupId = group.Id,
            EnrolledAt = DateTime.UtcNow
        };

        _db.Enrollments.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            entity.Id,
            entity.StudentId,
            entity.CourseGroupId,
            CourseName = group.LanguageCourse.Name,
            GroupName = group.GroupName
        });
    }
    // =========================================
    // DELETE: api/enrollments/my/{enrollmentId}
    // Usuwa zapis tylko jeśli należy do zalogowanego usera
    // =========================================
    [Authorize(Roles = "Admin,Staff,Student")]
    [HttpDelete("my/{enrollmentId:int}")]
    public async Task<IActionResult> UnenrollMy(int enrollmentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        if (!user.StudentId.HasValue)
            return BadRequest("User has no StudentId assigned.");

        var studentId = user.StudentId.Value;

        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.StudentId == studentId);

        if (enrollment == null)
            return NotFound("Enrollment not found or not yours.");

        _db.Enrollments.Remove(enrollment);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
