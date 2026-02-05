using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Victoria.Backend.DTOs.Payments;
using Victoria.Domain.Entities.Payments;
using Victoria.Domain.Enums;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Payments;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class InvoicesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public InvoicesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // =========================================
    // CREATE INVOICE
    // POST: api/invoices
    // =========================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InvoiceCreateDto dto)
    {
        var caseFileExists = await _dbContext.CaseFiles
            .AnyAsync(x => x.Id == dto.CaseFileId);

        if (!caseFileExists)
            return BadRequest("CaseFile not found");

        var invoice = new Invoice
        {
            CaseFileId = dto.CaseFileId,

            // jeśli chcesz numer z formularza, a nie auto:
            // InvoiceNumber = dto.InvoiceNumber,
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",

            // jeśli chcesz datę z formularza:
            // IssueDate = dto.IssueDate,
            IssueDate = DateTime.UtcNow,

            TotalAmount = dto.TotalAmount,
            Currency = dto.Currency,
            Status = InvoiceStatus.Issued,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Invoices.Add(invoice);
        await _dbContext.SaveChangesAsync();

        return Ok(MapToGetDto(invoice));
    }

    // =========================================
    // GET ALL INVOICES
    // GET: api/invoices
    // =========================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var invoices = await _dbContext.Invoices
            .OrderByDescending(x => x.IssueDate)
            .ToListAsync();

        return Ok(invoices.Select(MapToGetDto));
    }

    // =========================================
    // GET INVOICE BY ID
    // GET: api/invoices/{id}
    // =========================================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _dbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            return NotFound();

        return Ok(MapToGetDto(invoice));
    }

    // =========================================
    // FULL UPDATE INVOICE (EDIT)
    // PUT: api/invoices/{id}
    // =========================================
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] InvoiceUpdateDto dto)
    {
        var invoice = await _dbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            return NotFound();

        if (!Enum.TryParse<InvoiceStatus>(dto.Status, true, out var status))
            return BadRequest("Invalid invoice status");

        invoice.InvoiceNumber = dto.InvoiceNumber;
        invoice.IssueDate = dto.IssueDate;
        invoice.TotalAmount = dto.TotalAmount;
        invoice.Currency = dto.Currency;
        invoice.Status = status;

        await _dbContext.SaveChangesAsync();

        return Ok(MapToGetDto(invoice));
    }

    // =========================================
    // UPDATE ONLY STATUS
    // PUT: api/invoices/{id}/status
    // =========================================
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] InvoiceUpdateStatusDto dto)
    {
        var invoice = await _dbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            return NotFound();

        if (!Enum.TryParse<InvoiceStatus>(dto.Status, true, out var status))
            return BadRequest("Invalid invoice status");

        invoice.Status = status;
        await _dbContext.SaveChangesAsync();

        return Ok(MapToGetDto(invoice));
    }

    // =========================================
    // DELETE INVOICE
    // DELETE: api/invoices/{id}
    // =========================================
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var invoice = await _dbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            return NotFound();

        // jeśli masz InvoicePayments, to najbezpieczniej usunąć powiązania:
        var links = await _dbContext.InvoicePayments
            .Where(x => x.InvoiceId == id)
            .ToListAsync();

        if (links.Count > 0)
            _dbContext.InvoicePayments.RemoveRange(links);

        _dbContext.Invoices.Remove(invoice);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyInvoices()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var invoices = await _dbContext.Invoices
            .Include(i => i.CaseFile)
            .Where(i => i.CaseFile.ClientUserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new
            {
                i.Id,
                i.CaseFileId,
                i.InvoiceNumber,
                i.IssueDate,
                i.TotalAmount,
                i.Currency,
                Status = i.Status.ToString(),
                i.CreatedAt
            })
            .ToListAsync();

        return Ok(invoices);
    }
    // GET: api/invoices/by-case/{caseFileId}
    [HttpGet("by-case/{caseFileId:int}")]
    public async Task<IActionResult> GetByCase(int caseFileId)
    {
        var list = await _dbContext.Invoices
            .Where(x => x.CaseFileId == caseFileId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.InvoiceNumber,
                x.IssueDate,
                x.TotalAmount,
                x.Currency,
                Status = x.Status.ToString()
            })
            .ToListAsync();

        return Ok(list);
    }


    // =========================================
    // PRIVATE MAPPER
    // =========================================
    private static InvoiceGetDto MapToGetDto(Invoice invoice)
    {
        return new InvoiceGetDto
        {
            Id = invoice.Id,
            CaseFileId = invoice.CaseFileId,
            InvoiceNumber = invoice.InvoiceNumber,
            TotalAmount = invoice.TotalAmount,
            Currency = invoice.Currency,
            Status = invoice.Status.ToString(),
            IssueDate = invoice.IssueDate
        };
    }
}
