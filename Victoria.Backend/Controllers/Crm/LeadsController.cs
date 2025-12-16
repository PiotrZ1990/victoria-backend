using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Crm;
using Victoria.Domain.Entities.CRM;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Crm;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public LeadsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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

        if (lead == null)
            return NotFound();

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

        if (lead == null)
            return NotFound();

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

        if (lead == null)
            return NotFound();

        _dbContext.Leads.Remove(lead);
        await _dbContext.SaveChangesAsync();

        return NoContent();
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
