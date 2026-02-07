using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Education;

namespace Victoria.Web.Controllers.Education;

public class ExamsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ExamsController(IHttpClientFactory httpClientFactory)
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
            var resp = await client.GetAsync("api/exams");
            if (!resp.IsSuccessStatusCode) throw new Exception("Failed to load exams");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<ExamListViewModel>>(json, JsonOpts) ?? new();

            sort = (sort ?? "created_desc").ToLowerInvariant();
            items = sort switch
            {
                "created_asc" => items.OrderBy(x => x.CreatedAt).ToList(),
                "created_desc" => items.OrderByDescending(x => x.CreatedAt).ToList(),

                _ => items.OrderByDescending(x => x.CreatedAt).ToList()
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
            var resp = await client.GetAsync($"api/exams/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<ExamDetailsViewModel>(json, JsonOpts);
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
            return View(new ExamCreateViewModel());
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                name = vm.Name,
                examType = vm.ExamType,
                description = vm.Description,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("api/exams", content);

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
            var resp = await client.GetAsync($"api/exams/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<ExamDetailsViewModel>(json, JsonOpts);
            if (dto == null) return NotFound();

            var vm = new ExamEditViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                ExamType = dto.ExamType,
                Description = dto.Description,
                IsActive = dto.IsActive
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
    public async Task<IActionResult> Edit(int id, ExamEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                name = vm.Name,
                examType = vm.ExamType,
                description = vm.Description,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PutAsync($"api/exams/{id}", content);

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
            var resp = await client.DeleteAsync($"api/exams/{id}");

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
