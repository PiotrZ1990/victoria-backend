using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Education;

namespace Victoria.Web.Controllers.Education;

public class CourseGroupsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public CourseGroupsController(IHttpClientFactory httpClientFactory)
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
            var resp = await client.GetAsync("api/coursegroups");
            if (!resp.IsSuccessStatusCode) throw new Exception("Failed");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<CourseGroupListViewModel>>(json, JsonOpts) ?? new();
            sort = (sort ?? "created_desc").ToLowerInvariant();
            items = sort switch
            {
                "created_asc" => items.OrderBy(x => x.StartDate).ToList(),
                "created_desc" => items.OrderByDescending(x => x.StartDate).ToList(),

                _ => items.OrderByDescending(x => x.StartDate).ToList()
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
            var resp = await client.GetAsync($"api/coursegroups/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<CourseGroupDetailsViewModel>(json, JsonOpts);
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
            return View(new CourseGroupCreateAndEditViewModel());
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseGroupCreateAndEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                languageCourseId = vm.LanguageCourseId,
                groupName = vm.GroupName,
                startDate = vm.StartDate,
                endDate = vm.EndDate,
                capacity = vm.Capacity,
                location = vm.Location,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("api/coursegroups", content);

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

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync($"api/coursegroups/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<CourseGroupDetailsViewModel>(json, JsonOpts);
            if (dto == null) return NotFound();

            var vm = new CourseGroupEditViewModel
            {
                Id = dto.Id,
                LanguageCourseId = dto.LanguageCourseId,
                GroupName = dto.GroupName,
                StartDate = dto.StartDate.Date,
                EndDate = dto.EndDate.Date,
                Capacity = dto.Capacity,
                Location = dto.Location,
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
    public async Task<IActionResult> Edit(int id, CourseGroupEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                languageCourseId = vm.LanguageCourseId,
                groupName = vm.GroupName,
                startDate = vm.StartDate,
                endDate = vm.EndDate,
                capacity = vm.Capacity,
                location = vm.Location,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PutAsync($"api/coursegroups/{id}", content);

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.DeleteAsync($"api/coursegroups/{id}");

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
