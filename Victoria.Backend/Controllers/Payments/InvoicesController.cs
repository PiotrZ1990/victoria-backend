using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Payments;
using Victoria.Domain.Entities.Payments;
using Victoria.Domain.Enums;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Payments;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Staff,Admin")]
[AllowAnonymous]
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
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",
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
    // UPDATE INVOICE STATUS
    // PUT: api/invoices/{id}/status
    // =========================================
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] InvoiceUpdateStatusDto dto)
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
