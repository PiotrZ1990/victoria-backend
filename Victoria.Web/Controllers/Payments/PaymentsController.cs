using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
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
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync("api/payments");
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Failed to load payments");

            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<PaymentViewModel>>(json, JsonOpts) ?? new();

            return View(list);
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
