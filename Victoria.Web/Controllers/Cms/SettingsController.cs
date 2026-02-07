using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Cms;

namespace Victoria.Web.Controllers.Cms;

public class SettingsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public SettingsController(IHttpClientFactory httpClientFactory)
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

    [HttpGet]
    public async Task<IActionResult> Index(string? sort = "created_desc")
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync("api/settings");
            if (!resp.IsSuccessStatusCode) throw new Exception("Failed");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<SettingListViewModel>>(json, JsonOpts) ?? new();
            sort = (sort ?? "created_desc").ToLowerInvariant();
            items = sort switch
            {
                "created_asc" => items.OrderBy(x => x.UpdatedAt).ToList(),
                "created_desc" => items.OrderByDescending(x => x.UpdatedAt).ToList(),

                _ => items.OrderByDescending(x => x.UpdatedAt).ToList()
            };
            ViewBag.Sort = sort;
            return View(items);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync($"api/settings/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<SettingDetailsViewModel>(json, JsonOpts);
            if (vm == null) return NotFound();

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        try
        {
            _ = Api();
            return View(new SettingCreateViewModel());
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SettingCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                key = vm.Key,
                value = vm.Value,
                description = vm.Description
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("api/settings", content);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"Create failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";
                return View(vm);
            }

            TempData["Success"] = "Created";
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync($"api/settings/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<SettingDetailsViewModel>(json, JsonOpts);
            if (dto == null) return NotFound();

            var vm = new SettingEditViewModel
            {
                Id = dto.Id,
                Key = dto.Key,
                Value = dto.Value,
                Description = dto.Description
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SettingEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                key = vm.Key,
                value = vm.Value,
                description = vm.Description
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PutAsync($"api/settings/{id}", content);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"Update failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";
                return View(vm);
            }

            TempData["Success"] = "Updated";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.DeleteAsync($"api/settings/{id}");

            if (!resp.IsSuccessStatusCode) TempData["Error"] = "Delete failed";
            else TempData["Success"] = "Deleted";

            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
