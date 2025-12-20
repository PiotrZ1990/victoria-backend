using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Victoria.Web.Models.Crm;

namespace Victoria.Web.Controllers.Crm;

public class LeadsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LeadsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var client = _httpClientFactory.CreateClient("BackendApi");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/leads");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Nie udało się pobrać leadów z API");
        }

        var json = await response.Content.ReadAsStringAsync();

        var leads = JsonSerializer.Deserialize<List<LeadViewModel>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(leads);
    }
}
