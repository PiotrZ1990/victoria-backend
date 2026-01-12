using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Victoria.Web.Controllers.Cms;

public class NewsPostsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public NewsPostsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient Api()
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException();

        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    // =========================
    // LIST
    // =========================
    public async Task<IActionResult> Index()
    {
        var client = Api();

        var resp = await client.GetAsync("api/newsposts");
        if (!resp.IsSuccessStatusCode)
            throw new Exception("Failed to load news");

        var json = await resp.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List<NewsPostListViewModel>>(json, JsonOpts) ?? new();

        return View(list);
    }

    // =========================
    // DETAILS
    // =========================
    public async Task<IActionResult> Details(int id)
    {
        var client = Api();

        var resp = await client.GetAsync($"api/newsposts/{id}");
        if (!resp.IsSuccessStatusCode)
            return NotFound();

        var json = await resp.Content.ReadAsStringAsync();
        var vm = JsonSerializer.Deserialize<NewsPostDetailsViewModel>(json, JsonOpts);

        return View(vm);
    }

    // =========================
    // CREATE
    // =========================
    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsPostCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var client = Api();

        var content = new StringContent(
            JsonSerializer.Serialize(vm),
            Encoding.UTF8,
            "application/json");

        var resp = await client.PostAsync("api/newsposts", content);
        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.Error = "Create failed";
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // EDIT
    // =========================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var client = Api();

        var resp = await client.GetAsync($"api/newsposts/{id}");
        if (!resp.IsSuccessStatusCode)
            return NotFound();

        var json = await resp.Content.ReadAsStringAsync();
        var vm = JsonSerializer.Deserialize<NewsPostEditViewModel>(json, JsonOpts);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NewsPostEditViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var client = Api();

        var content = new StringContent(
            JsonSerializer.Serialize(vm),
            Encoding.UTF8,
            "application/json");

        var resp = await client.PutAsync($"api/newsposts/{id}", content);
        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.Error = "Update failed";
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // DELETE
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var client = Api();

        await client.DeleteAsync($"api/newsposts/{id}");
        return RedirectToAction(nameof(Index));
    }
}
