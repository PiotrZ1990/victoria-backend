using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Victoria.Web.Controllers;

public class ReportsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ReportsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateApiClientWithJwt()
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException();

        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [HttpGet]
    public async Task<IActionResult> OutstandingInvoicesExcel()
    {
        try
        {
            var client = CreateApiClientWithJwt();
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
            var client = CreateApiClientWithJwt();
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
}
