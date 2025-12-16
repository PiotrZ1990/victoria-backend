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
    // CREATE LEAD
    // POST: api/leads
    // =========================
    [HttpPost]
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

            Status = "New",              // startowy status CRM
            CreatedAt = DateTime.UtcNow
        };


        _dbContext.Leads.Add(lead);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetLeadById),
            new { id = lead.Id },
            lead);
    }

    // =========================
    // GET LEAD BY ID
    // GET: api/leads/{id}
    // =========================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetLeadById(int id)
    {
        var lead = await _dbContext.Leads.FindAsync(id);

        if (lead == null)
            return NotFound();

        return Ok(lead);
    }

    // =========================
    // GET ALL LEADS (LIST)
    // GET: api/leads
    // =========================
    [HttpGet]
    public async Task<IActionResult> GetAllLeads()
    {
        var leads = await _dbContext.Leads
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(leads);
    }
}
