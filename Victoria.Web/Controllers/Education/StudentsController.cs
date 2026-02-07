using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Education;

namespace Victoria.Web.Controllers.Education;

public class StudentsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public StudentsController(IHttpClientFactory httpClientFactory)
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
            var resp = await client.GetAsync("api/students");
            if (!resp.IsSuccessStatusCode) throw new Exception("Failed to load students");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<StudentListViewModel>>(json, JsonOpts) ?? new();
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
            var resp = await client.GetAsync($"api/students/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<StudentDetailsViewModel>(json, JsonOpts);
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
            return View(new StudentCreateViewModel());
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                firstName = vm.FirstName,
                lastName = vm.LastName,
                email = vm.Email,
                phone = vm.Phone,
                nationality = vm.Nationality,
                dateOfBirth = vm.DateOfBirth,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("api/students", content);

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
            var resp = await client.GetAsync($"api/students/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<StudentDetailsViewModel>(json, JsonOpts);
            if (dto == null) return NotFound();

            var vm = new StudentEditViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Nationality = dto.Nationality,
                DateOfBirth = dto.DateOfBirth,
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
    public async Task<IActionResult> Edit(int id, StudentEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                firstName = vm.FirstName,
                lastName = vm.LastName,
                email = vm.Email,
                phone = vm.Phone,
                nationality = vm.Nationality,
                dateOfBirth = vm.DateOfBirth,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PutAsync($"api/students/{id}", content);

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
            var resp = await client.DeleteAsync($"api/students/{id}");

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
