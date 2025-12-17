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
// [Authorize(Roles = "Staff,Admin")] // docelowo
[AllowAnonymous] // tymczasowo do testów
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public PaymentsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // =========================================
    // CREATE PAYMENT (ADD PAYMENT TO INVOICE)
    // POST: api/payments
    // =========================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PaymentCreateDto dto)
    {
        // 1️⃣ Sprawdź fakturę
        var invoice = await _dbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == dto.InvoiceId);

        if (invoice == null)
            return BadRequest("Invoice not found");

        // 2️⃣ Parsowanie enumów
        if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var method))
            return BadRequest("Invalid payment method");

        if (!Enum.TryParse<ServiceType>(dto.ServiceType, true, out var serviceType))
            return BadRequest("Invalid service type");

        // 3️⃣ Utworzenie payment
        var payment = new Payment
        {
            CaseFileId = invoice.CaseFileId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            PaymentMethod = method,
            ServiceType = serviceType,
            Status = PaymentStatus.Paid,
            PaymentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        // 4️⃣ Powiązanie Payment ↔ Invoice
        var invoicePayment = new InvoicePayment
        {
            InvoiceId = invoice.Id,
            PaymentId = payment.Id
        };

        _dbContext.InvoicePayments.Add(invoicePayment);
        await _dbContext.SaveChangesAsync();

        // =========================================
        // 5️⃣ UPDATE STATUSU FAKTURY (KLUCZOWE)
        // =========================================
        var totalPaid = await _dbContext.InvoicePayments
            .Where(x => x.InvoiceId == invoice.Id)
            .Include(x => x.Payment)
            .SumAsync(x => x.Payment.Amount);

        if (totalPaid >= invoice.TotalAmount)
        {
            invoice.Status = InvoiceStatus.Paid;
        }
        else
        {
            invoice.Status = InvoiceStatus.Issued;
        }

        await _dbContext.SaveChangesAsync();

        return Ok(MapToGetDto(payment, invoice.Id));
    }

    // =========================================
    // GET PAYMENTS FOR INVOICE
    // GET: api/payments/invoice/{invoiceId}
    // =========================================
    [HttpGet("invoice/{invoiceId:int}")]
    public async Task<IActionResult> GetByInvoice(int invoiceId)
    {
        var payments = await _dbContext.InvoicePayments
            .Where(x => x.InvoiceId == invoiceId)
            .Include(x => x.Payment)
            .Select(x => x.Payment)
            .ToListAsync();

        var result = payments.Select(p => MapToGetDto(p, invoiceId));
        return Ok(result);
    }

    // =========================================
    // PRIVATE MAPPER
    // =========================================
    private static PaymentGetDto MapToGetDto(Payment payment, int invoiceId)
    {
        return new PaymentGetDto
        {
            Id = payment.Id,
            CaseFileId = payment.CaseFileId,
            InvoiceId = invoiceId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentMethod = payment.PaymentMethod.ToString(),
            ServiceType = payment.ServiceType.ToString(),
            Status = payment.Status.ToString(),
            PaymentDate = payment.PaymentDate
        };
    }
}
