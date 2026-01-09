using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Victoria.Web.Models.Reports;

namespace Victoria.Web.Controllers;

public class ReportsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ReportsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private HttpClient Api()
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException();

        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    // =========================
    // DASHBOARD
    // GET: /Reports/Dashboard
    // =========================
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var client = Api();
            var vm = new ReportsDashboardViewModel();

            // lead conversion
            var convResp = await client.GetAsync("api/reports/leads/conversion");
            if (convResp.IsSuccessStatusCode)
            {
                var json = await convResp.Content.ReadAsStringAsync();
                vm.LeadConversion = JsonSerializer.Deserialize<LeadConversionViewModel>(json, JsonOpts);
            }

            // outstanding invoices
            var outResp = await client.GetAsync("api/reports/invoices/outstanding");
            if (outResp.IsSuccessStatusCode)
            {
                var json = await outResp.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<OutstandingInvoiceViewModel>>(json, JsonOpts) ?? new();
                vm.OutstandingCount = list.Count;
                vm.TotalOutstanding = list.Sum(x => x.RemainingAmount);
                vm.TopOutstanding = list.OrderByDescending(x => x.RemainingAmount).Take(10).ToList();
            }

            // revenue monthly
            var revResp = await client.GetAsync("api/reports/revenue/monthly");
            if (revResp.IsSuccessStatusCode)
            {
                var json = await revResp.Content.ReadAsStringAsync();
                vm.RevenueMonthly = JsonSerializer.Deserialize<List<RevenueByMonthViewModel>>(json, JsonOpts) ?? new();
            }

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // INVOICES REPORT
    // GET: /Reports/Invoices?days=14
    // =========================
    [HttpGet]
    public async Task<IActionResult> Invoices(int days = 14)
    {
        try
        {
            var client = Api();
            var vm = new ReportsInvoicesViewModel { Days = days };

            // status summary
            var statusResp = await client.GetAsync("api/reports/invoices/status-summary");
            if (statusResp.IsSuccessStatusCode)
            {
                var json = await statusResp.Content.ReadAsStringAsync();
                vm.StatusSummary = JsonSerializer.Deserialize<List<InvoiceStatusSummaryViewModel>>(json, JsonOpts) ?? new();
            }

            // overdue
            var overdueResp = await client.GetAsync($"api/reports/invoices/overdue?days={days}");
            if (overdueResp.IsSuccessStatusCode)
            {
                var json = await overdueResp.Content.ReadAsStringAsync();
                vm.Overdue = JsonSerializer.Deserialize<List<OverdueInvoiceViewModel>>(json, JsonOpts) ?? new();
            }

            // outstanding
            var outResp = await client.GetAsync("api/reports/invoices/outstanding");
            if (outResp.IsSuccessStatusCode)
            {
                var json = await outResp.Content.ReadAsStringAsync();
                vm.Outstanding = JsonSerializer.Deserialize<List<OutstandingInvoiceViewModel>>(json, JsonOpts) ?? new();
            }

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // PAYMENTS REPORT
    // GET: /Reports/Payments
    // =========================
    [HttpGet]
    public async Task<IActionResult> Payments()
    {
        try
        {
            var client = Api();
            var vm = new ReportsPaymentsViewModel();

            var revResp = await client.GetAsync("api/reports/revenue/monthly");
            if (revResp.IsSuccessStatusCode)
            {
                var json = await revResp.Content.ReadAsStringAsync();
                vm.RevenueMonthly = JsonSerializer.Deserialize<List<RevenueByMonthViewModel>>(json, JsonOpts) ?? new();
            }

            var svcResp = await client.GetAsync("api/reports/payments/by-service");
            if (svcResp.IsSuccessStatusCode)
            {
                var json = await svcResp.Content.ReadAsStringAsync();
                vm.ByService = JsonSerializer.Deserialize<List<PaymentsByServiceViewModel>>(json, JsonOpts) ?? new();
            }

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // EXPORTS (masz już) — zostawiamy
    // =========================

    [HttpGet]
    public async Task<IActionResult> OutstandingInvoicesExcel()
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync("api/reports/invoices/outstanding/excel");
            if (!resp.IsSuccessStatusCode) return BadRequest("Export failed");

            var bytes = await resp.Content.ReadAsByteArrayAsync();
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "OutstandingInvoices.xlsx");
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpGet]
    public async Task<IActionResult> OutstandingInvoicesPdf()
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync("api/reports/invoices/outstanding/pdf");
            if (!resp.IsSuccessStatusCode) return BadRequest("Export failed");

            var bytes = await resp.Content.ReadAsByteArrayAsync();
            return File(bytes, "application/pdf", "OutstandingInvoices.pdf");
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpGet]
    public async Task<IActionResult> RevenueMonthlyExcel()
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync("api/reports/revenue/monthly/excel");
            if (!resp.IsSuccessStatusCode) return BadRequest("Export failed");

            var bytes = await resp.Content.ReadAsByteArrayAsync();
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RevenueMonthly.xlsx");
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpGet]
    public async Task<IActionResult> RevenueMonthlyPdf()
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync("api/reports/revenue/monthly/pdf");
            if (!resp.IsSuccessStatusCode) return BadRequest("Export failed");

            var bytes = await resp.Content.ReadAsByteArrayAsync();
            return File(bytes, "application/pdf", "RevenueMonthly.pdf");
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
