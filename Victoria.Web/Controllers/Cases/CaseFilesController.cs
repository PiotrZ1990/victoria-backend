using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Victoria.Web.Models.Cases;

namespace Victoria.Web.Controllers.Cases;

public class CaseFilesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CaseFilesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = _httpClientFactory.CreateClient("BackendApi");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/casefiles");

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to load case files from API");

        var json = await response.Content.ReadAsStringAsync();

        var caseFiles = JsonSerializer.Deserialize<List<CaseFileViewModel>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(caseFiles);
    }
    public async Task<IActionResult> Details(int id)
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = _httpClientFactory.CreateClient("BackendApi");

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"api/casefiles/{id}");

        if (!response.IsSuccessStatusCode)
            return NotFound();

        var json = await response.Content.ReadAsStringAsync();

        var vm = System.Text.Json.JsonSerializer.Deserialize<Victoria.Web.Models.Cases.CaseFileDetailsViewModel>(
            json,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (vm == null)
            return NotFound();
        
        // Pobierz faktury dla tej sprawy
        var invoicesResponse = await client.GetAsync($"api/invoices/casefile/{id}");
        if (invoicesResponse.IsSuccessStatusCode)
        {
            var invoicesJson = await invoicesResponse.Content.ReadAsStringAsync();

            var invoices = System.Text.Json.JsonSerializer.Deserialize<List<Victoria.Web.Models.Payments.InvoiceViewModel>>(
                invoicesJson,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            vm.Invoices = invoices ?? new();
        }
        return View(vm);
    }

}
