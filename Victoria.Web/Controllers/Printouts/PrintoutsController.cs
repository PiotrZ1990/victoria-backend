using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Victoria.Web.Controllers.Printouts;

public class PrintoutsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PrintoutsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient Api()
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException();

        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    // GET: /Printouts/InvoiceDocx?invoiceId=1
    [HttpGet]
    public async Task<IActionResult> InvoiceDocx(int invoiceId)
    {
        try
        {
            var client = Api();

            // 🔥 trafiamy w backend: api/printouts/invoices/{id}/word
            var resp = await client.GetAsync($"api/printouts/invoices/{invoiceId}/word");
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"DOCX export failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. {body}";
                return RedirectToAction("Details", "Invoices", new { id = invoiceId });
            }

            var bytes = await resp.Content.ReadAsByteArrayAsync();

            var contentType = resp.Content.Headers.ContentType?.ToString()
                ?? "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            var fileName = resp.Content.Headers.ContentDisposition?.FileNameStar
                ?? resp.Content.Headers.ContentDisposition?.FileName
                ?? $"invoice_{invoiceId}.docx";

            fileName = fileName.Trim('"');

            return File(bytes, contentType, fileName);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
