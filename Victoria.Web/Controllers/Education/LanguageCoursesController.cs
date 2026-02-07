using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Education;

namespace Victoria.Web.Controllers.Education;

public class LanguageCoursesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public LanguageCoursesController(IHttpClientFactory httpClientFactory)
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

    // GET: /LanguageCourses
    [HttpGet]
    public async Task<IActionResult> Index(string? sort = "created_desc")
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync("api/languagecourses");
            if (!resp.IsSuccessStatusCode) throw new Exception("Failed");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<LanguageCourseListViewModel>>(json, JsonOpts) ?? new();

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

    // GET: /LanguageCourses/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync($"api/languagecourses/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<LanguageCourseDetailsViewModel>(json, JsonOpts);
            if (vm == null) return NotFound();

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // GET: /LanguageCourses/Create
    [HttpGet]
    public IActionResult Create()
    {
        try
        {
            _ = Api();
            return View(new LanguageCourseCreateViewModel());
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // POST: /LanguageCourses/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LanguageCourseCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                name = vm.Name,
                language = vm.Language,
                level = vm.Level,
                description = vm.Description,
                price = vm.Price,
                currency = vm.Currency,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("api/languagecourses", content);

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = "Create failed";
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

    // GET: /LanguageCourses/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync($"api/languagecourses/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<LanguageCourseDetailsViewModel>(json, JsonOpts);
            if (dto == null) return NotFound();

            var vm = new LanguageCourseEditViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Language = dto.Language,
                Level = dto.Level,
                Description = dto.Description,
                Price = dto.Price,
                Currency = dto.Currency,
                IsActive = dto.IsActive
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // POST: /LanguageCourses/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LanguageCourseEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                name = vm.Name,
                language = vm.Language,
                level = vm.Level,
                description = vm.Description,
                price = vm.Price,
                currency = vm.Currency,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PutAsync($"api/languagecourses/{id}", content);

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = "Update failed";
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

    // POST: /LanguageCourses/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.DeleteAsync($"api/languagecourses/{id}");

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
