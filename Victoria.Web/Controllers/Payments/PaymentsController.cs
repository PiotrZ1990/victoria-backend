using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Victoria.Domain.Entities.Payments;
using Victoria.Web.Models.Payments;

namespace Victoria.Web.Controllers.Payments;

public class PaymentsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public PaymentsController(IHttpClientFactory httpClientFactory)
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

    // GET: /Payments
    [HttpGet]
    public async Task<IActionResult> Index(string? sort = "created_desc")
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync("api/payments");
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Failed to load payments");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<PaymentViewModel>>(json, JsonOpts) ?? new();

            sort = (sort ?? "created_desc").ToLowerInvariant();
                items = sort switch
                {
                    "created_asc" => items.OrderBy(x => x.PaymentDate).ToList(),
                    "created_desc" => items.OrderByDescending(x => x.PaymentDate).ToList(),

                    "amount_asc" => items.OrderBy(x => x.Amount).ToList(),
                    "amount_desc" => items.OrderByDescending(x => x.Amount).ToList(),

                    "invoice_asc" => items.OrderBy(x => x.InvoiceId).ToList(),
                    "invoice_desc" => items.OrderByDescending(x => x.InvoiceId).ToList(),

                    "method_asc" => items.OrderBy(x => x.PaymentMethod).ToList(),
                    "method_desc" => items.OrderByDescending(x => x.PaymentMethod).ToList(),

                    _ => items.OrderByDescending(x => x.PaymentDate).ToList()
                };
                ViewBag.Sort = sort;
            return View(items);

        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // GET: /Payments/Case?caseFileId=1
    [HttpGet]
    public async Task<IActionResult> Case(int caseFileId)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/payments/casefile/{caseFileId}");
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Failed to load case payments");

            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<PaymentViewModel>>(json, JsonOpts) ?? new();

            ViewBag.CaseFileId = caseFileId;
            return View(list);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
