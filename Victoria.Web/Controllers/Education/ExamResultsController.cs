using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Education;

namespace Victoria.Web.Controllers.Education;

public class ExamResultsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ExamResultsController(IHttpClientFactory httpClientFactory)
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

    // ✅ POPRAWIONE: bierzemy listę sesji i mapujemy na lookup z Display
    private async Task<List<ExamSessionLookupViewModel>> LoadSessionsAsync(HttpClient client)
    {
        var resp = await client.GetAsync("api/examsessions");
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync();
        var sessions = JsonSerializer.Deserialize<List<ExamSessionListViewModel>>(json, JsonOpts) ?? new();

        return sessions
            .OrderByDescending(x => x.SessionDate)
            .Select(s =>
            {
                var loc = string.IsNullOrWhiteSpace(s.Location) ? "-" : s.Location;
                return new ExamSessionLookupViewModel
                {
                    Id = s.Id,
                    Display = $"{s.ExamName} | {s.SessionDate:yyyy-MM-dd} | {loc}"
                };
            })
            .ToList();
    }

    private async Task<List<StudentLookupViewModel>> LoadStudentsAsync(HttpClient client)
    {
        var resp = await client.GetAsync("api/students");
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<StudentLookupViewModel>>(json, JsonOpts) ?? new();
    }

    // =========================
    // INDEX
    // GET: /ExamResults
    // =========================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync("api/examresults");
            if (!resp.IsSuccessStatusCode) throw new Exception("Failed");

            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<ExamResultListViewModel>>(json, JsonOpts) ?? new();
            return View(list);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // DETAILS
    // GET: /ExamResults/Details/5
    // =========================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync($"api/examresults/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<ExamResultDetailsViewModel>(json, JsonOpts);
            if (vm == null) return NotFound();

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // CREATE (GET)
    // =========================
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            var client = Api();

            var vm = new ExamResultCreateViewModel
            {
                Sessions = await LoadSessionsAsync(client),
                Students = await LoadStudentsAsync(client)
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // CREATE (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamResultCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            try
            {
                var client = Api();
                vm.Sessions = await LoadSessionsAsync(client);
                vm.Students = await LoadStudentsAsync(client);
            }
            catch { }
            return View(vm);
        }

        try
        {
            var client = Api();

            var payload = new
            {
                examSessionId = vm.ExamSessionId,
                studentId = vm.StudentId,
                score = vm.Score,
                notes = vm.Notes
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("api/examresults", content);

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = "Create failed";
                vm.Sessions = await LoadSessionsAsync(client);
                vm.Students = await LoadStudentsAsync(client);
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

    // =========================
    // EDIT (GET)
    // =========================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/examresults/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<ExamResultDetailsViewModel>(json, JsonOpts);
            if (dto == null) return NotFound();

            var vm = new ExamResultEditViewModel
            {
                Id = dto.Id,
                ExamSessionId = dto.ExamSessionId,
                StudentId = dto.StudentId,
                Score = dto.Score,
                Notes = dto.Notes,
                Sessions = await LoadSessionsAsync(client),
                Students = await LoadStudentsAsync(client)
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // EDIT (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExamResultEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            try
            {
                var client = Api();
                vm.Sessions = await LoadSessionsAsync(client);
                vm.Students = await LoadStudentsAsync(client);
            }
            catch { }
            return View(vm);
        }

        try
        {
            var client = Api();

            var payload = new
            {
                examSessionId = vm.ExamSessionId,
                studentId = vm.StudentId,
                score = vm.Score,
                notes = vm.Notes
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PutAsync($"api/examresults/{id}", content);

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = "Update failed";
                vm.Sessions = await LoadSessionsAsync(client);
                vm.Students = await LoadStudentsAsync(client);
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

    // =========================
    // DELETE (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.DeleteAsync($"api/examresults/{id}");

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
