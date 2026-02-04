using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Crm;
using Victoria.Domain.Entities.CRM;
using Victoria.Domain.Entities.Education;
using Victoria.Infrastructure.Data;
using Victoria.Infrastructure.Identity;

namespace Victoria.Backend.Controllers.Crm;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public LeadsController(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    // =========================
    // CREATE LEAD (PUBLIC)
    // POST: api/leads
    // =========================
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateLead([FromBody] LeadCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var lead = new Lead
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Source = dto.Source,
            InterestedCountry = dto.InterestedCountry,
            InterestedService = dto.InterestedService,
            Status = "New",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Leads.Add(lead);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetLeadById),
            new { id = lead.Id },
            MapToGetDto(lead));
    }

    // =========================
    // GET LEAD BY ID (STAFF, ADMIN)
    // GET: api/leads/{id}
    // =========================
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<IActionResult> GetLeadById(int id)
    {
        var lead = await _dbContext.Leads.FindAsync(id);
        if (lead == null) return NotFound();
        return Ok(MapToGetDto(lead));
    }

    // =========================
    // GET ALL LEADS (STAFF, ADMIN)
    // GET: api/leads
    // =========================
    [HttpGet]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<IActionResult> GetAllLeads()
    {
        var leads = await _dbContext.Leads
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(leads.Select(MapToGetDto));
    }

    // =========================
    // UPDATE LEAD (STAFF, ADMIN)
    // PUT: api/leads/{id}
    // =========================
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<IActionResult> UpdateLead(int id, [FromBody] LeadUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var lead = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id);
        if (lead == null) return NotFound();

        lead.FullName = dto.FullName;
        lead.Email = dto.Email;
        lead.PhoneNumber = dto.PhoneNumber;
        lead.Source = dto.Source;
        lead.InterestedCountry = dto.InterestedCountry;
        lead.InterestedService = dto.InterestedService;
        lead.Status = dto.Status;

        await _dbContext.SaveChangesAsync();
        return Ok(MapToGetDto(lead));
    }

    // =========================
    // DELETE LEAD (ADMIN ONLY)
    // DELETE: api/leads/{id}
    // =========================
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLead(int id)
    {
        var lead = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id);
        if (lead == null) return NotFound();

        _dbContext.Leads.Remove(lead);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // =========================
    // CREATE CLIENT ACCOUNT FOR LEAD
    // POST: api/leads/{leadId}/create-client-account
    // =========================
    [HttpPost("{leadId:int}/create-client-account")]
    //[Authorize(Roles = "Staff,Admin")]
    public async Task<IActionResult> CreateClientAccount(int leadId)
    {
        var lead = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == leadId);
        if (lead == null) return NotFound("Lead not found");

        if (string.IsNullOrWhiteSpace(lead.Email))
            return BadRequest("Lead has no email. Cannot create account.");

        var email = lead.Email.Trim();

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null)
            return BadRequest("User with this email already exists.");

        var tempPassword = "Temp#" + Guid.NewGuid().ToString("N")[..8] + "a!";

        // 1) Create Identity user
        var fullName = (lead.FullName ?? "").Trim();
        if (string.IsNullOrWhiteSpace(fullName))
            fullName = email.Split('@')[0];

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName
        };

        var res = await _userManager.CreateAsync(user, tempPassword);
        if (!res.Succeeded)
            return BadRequest(string.Join(" | ", res.Errors.Select(e => e.Description)));

        // 2) Create Student + link to user.StudentId
        var (firstName, lastName) = SplitName(user.FullName, user.Email);

        var student = new Student
        {
            FirstName = firstName,
            LastName = lastName,
            Email = user.Email,
            Phone = lead.PhoneNumber,
            Nationality = null,
            DateOfBirth = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();

        user.StudentId = student.Id;
        var upd = await _userManager.UpdateAsync(user);
        if (!upd.Succeeded)
            return BadRequest(string.Join(" | ", upd.Errors.Select(e => e.Description)));

        return Ok(new
        {
            userId = user.Id,
            login = email,
            tempPassword,
            studentId = student.Id
        });
    }

    private static (string FirstName, string LastName) SplitName(string? fullName, string? email)
    {
        var name = (fullName ?? "").Trim();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return (parts[0], "Client");

            var first = parts[0];
            var last = string.Join(" ", parts.Skip(1));
            return (first, last);
        }

        var local = (email ?? "client").Split('@')[0].Trim();
        if (string.IsNullOrWhiteSpace(local)) local = "Client";
        return (local, "User");
    }

    // =========================
    // ATTACH CLIENT USER TO CASEFILE BY LEAD
    // POST: api/leads/{leadId}/attach-user-to-case
    // =========================
    [HttpPost("{leadId:int}/attach-user-to-case")]
    //[Authorize(Roles = "Staff,Admin")]
    public async Task<IActionResult> AttachUserToCase(int leadId, [FromBody] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("userId required");

        var caseFile = await _dbContext.CaseFiles.FirstOrDefaultAsync(x => x.LeadId == leadId);
        if (caseFile == null) return NotFound("CaseFile for this lead not found");

        caseFile.ClientUserId = userId;
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            caseFileId = caseFile.Id,
            caseFile.ClientUserId
        });
    }

    // =========================
    // PRIVATE MAPPER
    // =========================
    private static LeadGetDto MapToGetDto(Lead lead)
    {
        return new LeadGetDto
        {
            Id = lead.Id,
            FullName = lead.FullName,
            Email = lead.Email,
            PhoneNumber = lead.PhoneNumber,
            Source = lead.Source,
            InterestedCountry = lead.InterestedCountry,
            InterestedService = lead.InterestedService,
            Status = lead.Status,
            CreatedAt = lead.CreatedAt
        };
    }
}
