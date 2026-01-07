using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Reports;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Reports;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
//[Authorize(Roles = "Admin,Staff")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ReportsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // =========================================
    // REVENUE PER MONTH
    // GET: api/reports/revenue/monthly
    // =========================================
    [HttpGet("revenue/monthly")]
    public async Task<IActionResult> GetRevenuePerMonth()
    {
        var result = await _dbContext.Payments
            .Where(p => p.Status == Domain.Enums.PaymentStatus.Paid)
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new RevenueByMonthDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalRevenue = g.Sum(x => x.Amount)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        return Ok(result);
    }

    // =========================================
    // PAYMENTS BY SERVICE TYPE
    // GET: api/reports/payments/by-service
    // =========================================
    [HttpGet("payments/by-service")]
    public async Task<IActionResult> GetPaymentsByServiceType()
    {
        var result = await _dbContext.Payments
            .Where(p => p.Status == Domain.Enums.PaymentStatus.Paid)
            .GroupBy(p => p.ServiceType)
            .Select(g => new PaymentsByServiceDto
            {
                ServiceType = g.Key.ToString(),
                TotalAmount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync();

        return Ok(result);
    }

    // =========================================
    // INVOICE STATUS SUMMARY
    // GET: api/reports/invoices/status-summary
    // =========================================
    [HttpGet("invoices/status-summary")]
    public async Task<IActionResult> GetInvoiceStatusSummary()
    {
        var result = await _dbContext.Invoices
            .GroupBy(i => i.Status)
            .Select(g => new InvoiceStatusSummaryDto
            {
                Status = g.Key.ToString(),
                Count = g.Count()
            })
            .ToListAsync();

        return Ok(result);
    }

    // =========================================
    // LEAD CONVERSION
    // GET: api/reports/leads/conversion
    // =========================================
    [HttpGet("leads/conversion")]
    public async Task<IActionResult> GetLeadConversion()
    {
        var totalLeads = await _dbContext.Leads.CountAsync();
        var convertedLeads = await _dbContext.Leads
            .CountAsync(l => l.Status == "Converted");

        var conversionRate = totalLeads == 0
            ? 0
            : Math.Round((double)convertedLeads / totalLeads * 100, 2);

        var result = new LeadConversionDto
        {
            TotalLeads = totalLeads,
            ConvertedLeads = convertedLeads,
            ConversionRate = conversionRate
        };

        return Ok(result);
    }
    
    // =========================================
    // CASES BY STAGE
    // GET: api/reports/cases/by-stage?from=2026-01-01&to=2026-01-31
    // =========================================
    [HttpGet("cases/by-stage")]
    public async Task<IActionResult> GetCasesByStage([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var query = _dbContext.CaseFiles.AsQueryable();

        if (from.HasValue)
            query = query.Where(x => x.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.CreatedAt < to.Value.AddDays(1));

        var result = await query
            .GroupBy(x => x.Stage)
            .Select(g => new CaseStageSummaryDto
            {
                Stage = g.Key.ToString(),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return Ok(result);
    }

    // =========================================
    // OVERDUE INVOICES
    // GET: api/reports/invoices/overdue?days=14
    // =========================================
    [HttpGet("invoices/overdue")]
    public async Task<IActionResult> GetOverdueInvoices([FromQuery] int days = 14)
    {
        var today = DateTime.UtcNow.Date;
        var cutoff = today.AddDays(-days);

        // bierzemy nieopłacone w całości i starsze niż cutoff
        var invoices = await _dbContext.Invoices
            .Where(i => i.IssueDate.Date <= cutoff
                        && (i.Status == Domain.Enums.InvoiceStatus.Issued
                            || i.Status == Domain.Enums.InvoiceStatus.PartiallyPaid
                            || i.Status == Domain.Enums.InvoiceStatus.Overdue))
            .OrderBy(i => i.IssueDate)
            .ToListAsync();

        var invoiceIds = invoices.Select(x => x.Id).ToList();

        // suma płatności per invoice
        var paidByInvoice = await _dbContext.InvoicePayments
            .Where(x => invoiceIds.Contains(x.InvoiceId))
            .Include(x => x.Payment)
            .GroupBy(x => x.InvoiceId)
            .Select(g => new
            {
                InvoiceId = g.Key,
                Paid = g.Sum(x => x.Payment.Amount)
            })
            .ToListAsync();

        var dictPaid = paidByInvoice.ToDictionary(x => x.InvoiceId, x => x.Paid);

        var result = invoices.Select(i =>
        {
            var paid = dictPaid.TryGetValue(i.Id, out var p) ? p : 0m;
            var remaining = Math.Max(0m, i.TotalAmount - paid);
            var overdueDays = (today - i.IssueDate.Date).Days;

            return new OverdueInvoiceDto
            {
                InvoiceId = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                CaseFileId = i.CaseFileId,
                TotalAmount = i.TotalAmount,
                PaidAmount = paid,
                RemainingAmount = remaining,
                Currency = i.Currency,
                IssueDate = i.IssueDate,
                DaysOverdue = overdueDays
            };
        })
        .Where(x => x.RemainingAmount > 0)
        .OrderByDescending(x => x.DaysOverdue)
        .ToList();

        return Ok(result);
    }

    // =========================================
    // OUTSTANDING INVOICES (NOT FULLY PAID)
    // GET: api/reports/invoices/outstanding
    // =========================================
    [HttpGet("invoices/outstanding")]
    public async Task<IActionResult> GetOutstandingInvoices()
    {
        var invoices = await _dbContext.Invoices
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();

        var invoiceIds = invoices.Select(x => x.Id).ToList();

        var paidByInvoice = await _dbContext.InvoicePayments
            .Where(x => invoiceIds.Contains(x.InvoiceId))
            .Include(x => x.Payment)
            .GroupBy(x => x.InvoiceId)
            .Select(g => new
            {
                InvoiceId = g.Key,
                Paid = g.Sum(x => x.Payment.Amount)
            })
            .ToListAsync();

        var dictPaid = paidByInvoice.ToDictionary(x => x.InvoiceId, x => x.Paid);

        var result = invoices
            .Select(i =>
            {
                var paid = dictPaid.TryGetValue(i.Id, out var p) ? p : 0m;
                var remaining = Math.Max(0m, i.TotalAmount - paid);

                return new OutstandingInvoiceDto
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    CaseFileId = i.CaseFileId,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = paid,
                    RemainingAmount = remaining,
                    Currency = i.Currency,
                    Status = i.Status.ToString(),
                    IssueDate = i.IssueDate
                };
            })
            .Where(x => x.RemainingAmount > 0)
            .OrderByDescending(x => x.RemainingAmount)
            .ToList();

        return Ok(result);
    }

    // =========================================
    // CASES MISSING REQUIRED DOCS
    // GET: api/reports/cases/missing-documents
    // =========================================
    [HttpGet("cases/missing-documents")]
    public async Task<IActionResult> GetCasesMissingDocuments()
    {
        var requiredApp = await _dbContext.ApplicationDocumentChecklists.CountAsync(x => x.IsRequired);
        var requiredVisa = await _dbContext.VisaDocumentChecklists.CountAsync(x => x.IsRequired);

        var cases = await _dbContext.CaseFiles
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.CaseNumber,
                Stage = x.Stage.ToString()
            })
            .ToListAsync();

        var caseIds = cases.Select(x => x.Id).ToList();

        var completedAppByCase = await _dbContext.CaseApplicationChecklistItems
            .Include(x => x.Checklist)
            .Where(x => caseIds.Contains(x.CaseFileId) && x.Checklist.IsRequired && x.IsCompleted)
            .GroupBy(x => x.CaseFileId)
            .Select(g => new { CaseFileId = g.Key, Count = g.Count() })
            .ToListAsync();

        var completedVisaByCase = await _dbContext.CaseVisaChecklistItems
            .Include(x => x.Checklist)
            .Where(x => caseIds.Contains(x.CaseFileId) && x.Checklist.IsRequired && x.IsCompleted)
            .GroupBy(x => x.CaseFileId)
            .Select(g => new { CaseFileId = g.Key, Count = g.Count() })
            .ToListAsync();

        var dictApp = completedAppByCase.ToDictionary(x => x.CaseFileId, x => x.Count);
        var dictVisa = completedVisaByCase.ToDictionary(x => x.CaseFileId, x => x.Count);

        var result = cases.Select(c =>
        {
            var doneApp = dictApp.TryGetValue(c.Id, out var a) ? a : 0;
            var doneVisa = dictVisa.TryGetValue(c.Id, out var v) ? v : 0;

            return new CaseMissingDocsDto
            {
                CaseFileId = c.Id,
                CaseNumber = c.CaseNumber,
                Stage = c.Stage,
                MissingRequiredApplicationItems = Math.Max(0, requiredApp - doneApp),
                MissingRequiredVisaItems = Math.Max(0, requiredVisa - doneVisa)
            };
        })
        // pokaż tylko te sprawy, gdzie czegoś brakuje
        .Where(x => x.MissingRequiredApplicationItems > 0 || x.MissingRequiredVisaItems > 0)
        .OrderByDescending(x => x.MissingRequiredApplicationItems + x.MissingRequiredVisaItems)
        .ToList();

        return Ok(result);
    }

}
