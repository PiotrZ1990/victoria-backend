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


}
