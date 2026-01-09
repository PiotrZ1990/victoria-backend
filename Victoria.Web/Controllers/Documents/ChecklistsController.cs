using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Documents;

namespace Victoria.Web.Controllers.Documents;

public class ChecklistsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ChecklistsController(IHttpClientFactory httpClientFactory)
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

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // GET: /Checklists/Application?caseFileId=1
    [HttpGet]
    public async Task<IActionResult> Application(int caseFileId)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/checklists/application/{caseFileId}");
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Failed to load application checklist. Status={(int)resp.StatusCode}");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<ChecklistItemViewModel>>(json, JsonOpts) ?? new();

            ViewBag.CaseFileId = caseFileId;
            return View(items);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // GET: /Checklists/Visa?caseFileId=1
    [HttpGet]
    public async Task<IActionResult> Visa(int caseFileId)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/checklists/visa/{caseFileId}");
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Failed to load visa checklist. Status={(int)resp.StatusCode}");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<ChecklistItemViewModel>>(json, JsonOpts) ?? new();

            ViewBag.CaseFileId = caseFileId;
            return View(items);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
