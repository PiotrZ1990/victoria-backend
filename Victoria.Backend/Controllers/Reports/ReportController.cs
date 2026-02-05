using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using Victoria.Backend.DTOs.Reports;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Reports;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
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

    // =========================================
    // OUTSTANDING INVOICES -> EXCEL EXPORT
    // GET: api/reports/invoices/outstanding/excel
    // =========================================
    [HttpGet("invoices/outstanding/excel")]
    public async Task<IActionResult> ExportOutstandingInvoicesToExcel()
    {
        // 1) Pobierz dane jak w /invoices/outstanding
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

        var rows = invoices
            .Select(i =>
            {
                var paid = dictPaid.TryGetValue(i.Id, out var p) ? p : 0m;
                var remaining = Math.Max(0m, i.TotalAmount - paid);

                return new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.CaseFileId,
                    i.IssueDate,
                    i.Currency,
                    Status = i.Status.ToString(),
                    Total = i.TotalAmount,
                    Paid = paid,
                    Remaining = remaining
                };
            })
            .Where(x => x.Remaining > 0)
            .OrderByDescending(x => x.Remaining)
            .ToList();

        // 2) Zrób Excela
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Outstanding Invoices");

        // Nagłówki
        ws.Cell(1, 1).Value = "InvoiceId";
        ws.Cell(1, 2).Value = "InvoiceNumber";
        ws.Cell(1, 3).Value = "CaseFileId";
        ws.Cell(1, 4).Value = "IssueDate";
        ws.Cell(1, 5).Value = "Currency";
        ws.Cell(1, 6).Value = "Status";
        ws.Cell(1, 7).Value = "TotalAmount";
        ws.Cell(1, 8).Value = "PaidAmount";
        ws.Cell(1, 9).Value = "RemainingAmount";

        ws.Range(1, 1, 1, 9).Style.Font.Bold = true;

        // Dane
        var r = 2;
        foreach (var x in rows)
        {
            ws.Cell(r, 1).Value = x.Id;
            ws.Cell(r, 2).Value = x.InvoiceNumber;
            ws.Cell(r, 3).Value = x.CaseFileId;
            ws.Cell(r, 4).Value = x.IssueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            ws.Cell(r, 5).Value = x.Currency;
            ws.Cell(r, 6).Value = x.Status;
            ws.Cell(r, 7).Value = x.Total;
            ws.Cell(r, 8).Value = x.Paid;
            ws.Cell(r, 9).Value = x.Remaining;
            r++;
        }

        ws.Columns().AdjustToContents();

        // 3) Zwróć plik
        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"outstanding_invoices_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx";
        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    // =========================================
    // REVENUE PER MONTH -> EXCEL EXPORT
    // GET: api/reports/revenue/monthly/excel
    // =========================================
    [HttpGet("revenue/monthly/excel")]
    public async Task<IActionResult> ExportRevenueMonthlyToExcel()
    {
        var rows = await _dbContext.Payments
            .Where(p => p.Status == Domain.Enums.PaymentStatus.Paid)
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalRevenue = g.Sum(x => x.Amount)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Revenue Monthly");

        ws.Cell(1, 1).Value = "Year";
        ws.Cell(1, 2).Value = "Month";
        ws.Cell(1, 3).Value = "TotalRevenue";

        ws.Range(1, 1, 1, 3).Style.Font.Bold = true;

        var r = 2;
        foreach (var x in rows)
        {
            ws.Cell(r, 1).Value = x.Year;
            ws.Cell(r, 2).Value = x.Month;
            ws.Cell(r, 3).Value = x.TotalRevenue;
            r++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"revenue_monthly_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx";
        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    // =========================================
    // OUTSTANDING INVOICES -> PDF EXPORT
    // GET: api/reports/invoices/outstanding/pdf
    // =========================================
    [HttpGet("invoices/outstanding/pdf")]
    public async Task<IActionResult> ExportOutstandingInvoicesToPdf()
    {
        // dane jak w /invoices/outstanding
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

        var rows = invoices
            .Select(i =>
            {
                var paid = dictPaid.TryGetValue(i.Id, out var p) ? p : 0m;
                var remaining = Math.Max(0m, i.TotalAmount - paid);

                return new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.CaseFileId,
                    i.IssueDate,
                    i.Currency,
                    Status = i.Status.ToString(),
                    Total = i.TotalAmount,
                    Paid = paid,
                    Remaining = remaining
                };
            })
            .Where(x => x.Remaining > 0)
            .OrderByDescending(x => x.Remaining)
            .ToList();

        // QuestPDF license (wymagane)
        QuestPDF.Settings.License = LicenseType.Community;

        var pdfBytes = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("Victoria - Outstanding Invoices").FontSize(18).SemiBold();
                    col.Item().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(45);   // Id
                        columns.RelativeColumn(2);    // InvoiceNumber
                        columns.ConstantColumn(65);   // CaseFileId
                        columns.ConstantColumn(75);   // IssueDate
                        columns.ConstantColumn(45);   // Curr
                        columns.RelativeColumn(1);    // Status
                        columns.ConstantColumn(70);   // Total
                        columns.ConstantColumn(70);   // Paid
                        columns.ConstantColumn(85);   // Remaining
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Id");
                        header.Cell().Element(HeaderCell).Text("Number");
                        header.Cell().Element(HeaderCell).Text("Case");
                        header.Cell().Element(HeaderCell).Text("Date");
                        header.Cell().Element(HeaderCell).Text("Cur");
                        header.Cell().Element(HeaderCell).Text("Status");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Total");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Paid");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Remaining");

                        static IContainer HeaderCell(IContainer c) =>
                            c.PaddingVertical(4).PaddingHorizontal(2)
                             .Background(Colors.Grey.Lighten3)
                             .BorderBottom(1).BorderColor(Colors.Grey.Darken1)
                             .DefaultTextStyle(x => x.SemiBold().FontSize(10));
                    });

                    foreach (var r in rows)
                    {
                        table.Cell().Element(Cell).Text(r.Id.ToString());
                        table.Cell().Element(Cell).Text(r.InvoiceNumber);
                        table.Cell().Element(Cell).Text(r.CaseFileId.ToString());
                        table.Cell().Element(Cell).Text(r.IssueDate.ToString("yyyy-MM-dd"));
                        table.Cell().Element(Cell).Text(r.Currency);
                        table.Cell().Element(Cell).Text(r.Status);
                        table.Cell().Element(Cell).AlignRight().Text(r.Total.ToString("0.00"));
                        table.Cell().Element(Cell).AlignRight().Text(r.Paid.ToString("0.00"));
                        table.Cell().Element(Cell).AlignRight().Text(r.Remaining.ToString("0.00"));
                    }

                    static IContainer Cell(IContainer c) =>
                        c.PaddingVertical(3).PaddingHorizontal(2)
                         .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                         .DefaultTextStyle(x => x.FontSize(10));
                });

                page.Footer().AlignRight().Text(t =>
                {
                    t.Span("Page ");
                    t.CurrentPageNumber();
                    t.Span(" / ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();

        var fileName = $"outstanding_invoices_{DateTime.UtcNow:yyyyMMdd_HHmm}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

    // =========================================
    // REVENUE MONTHLY -> PDF EXPORT
    // GET: api/reports/revenue/monthly/pdf
    // =========================================
    [HttpGet("revenue/monthly/pdf")]
    public async Task<IActionResult> ExportRevenueMonthlyToPdf()
    {
        var rows = await _dbContext.Payments
            .Where(p => p.Status == Domain.Enums.PaymentStatus.Paid)
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalRevenue = g.Sum(x => x.Amount)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        QuestPDF.Settings.License = LicenseType.Community;

        var pdfBytes = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("Victoria - Revenue per Month").FontSize(18).SemiBold();
                    col.Item().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(70);   // Year
                        columns.ConstantColumn(70);   // Month
                        columns.RelativeColumn();     // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Year");
                        header.Cell().Element(HeaderCell).Text("Month");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Total revenue");

                        static IContainer HeaderCell(IContainer c) =>
                            c.PaddingVertical(4).PaddingHorizontal(2)
                             .Background(Colors.Grey.Lighten3)
                             .BorderBottom(1).BorderColor(Colors.Grey.Darken1)
                             .DefaultTextStyle(x => x.SemiBold().FontSize(10));
                    });

                    foreach (var r in rows)
                    {
                        table.Cell().Element(Cell).Text(r.Year.ToString());
                        table.Cell().Element(Cell).Text(r.Month.ToString("D2"));
                        table.Cell().Element(Cell).AlignRight().Text(r.TotalRevenue.ToString("0.00"));
                    }

                    static IContainer Cell(IContainer c) =>
                        c.PaddingVertical(3).PaddingHorizontal(2)
                         .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                         .DefaultTextStyle(x => x.FontSize(10));
                });

                page.Footer().AlignRight().Text(t =>
                {
                    t.Span("Page ");
                    t.CurrentPageNumber();
                    t.Span(" / ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();

        var fileName = $"revenue_monthly_{DateTime.UtcNow:yyyyMMdd_HHmm}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

}
