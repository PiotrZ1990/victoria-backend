using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Education;

namespace Victoria.Web.Controllers.Education;

public class ExamSessionsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ExamSessionsController(IHttpClientFactory httpClientFactory)
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

    private async Task<List<ExamLookupViewModel>> LoadExamsAsync(HttpClient client)
    {
        var resp = await client.GetAsync("api/exams");
        if (!resp.IsSuccessStatusCode)
            return new();

        var json = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ExamLookupViewModel>>(json, JsonOpts) ?? new();
    }

    // =========================
    // INDEX
    // GET: /ExamSessions
    // =========================
    [HttpGet]
    public async Task<IActionResult> Index(string? sort = "created_desc")
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync("api/examsessions");
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Failed to load exam sessions");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<ExamSessionListViewModel>>(json, JsonOpts) ?? new();

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

    // =========================
    // DETAILS
    // GET: /ExamSessions/Details/5
    // =========================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/examsessions/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<ExamSessionDetailsViewModel>(json, JsonOpts);
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
    // GET: /ExamSessions/Create
    // =========================
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            var client = Api();

            var vm = new ExamSessionCreateViewModel
            {
                SessionDate = DateTime.UtcNow.Date
            };

            vm.Exams = await LoadExamsAsync(client);
            ViewBag.Exams = new SelectList(vm.Exams, "Id", "Name", vm.ExamId);

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
    public async Task<IActionResult> Create(ExamSessionCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            try
            {
                var client = Api();
                vm.Exams = await LoadExamsAsync(client);
                ViewBag.Exams = new SelectList(vm.Exams, "Id", "Name", vm.ExamId);
            }
            catch { /* ignore */ }

            return View(vm);
        }

        try
        {
            var client = Api();

            var payload = new
            {
                examId = vm.ExamId,
                sessionDate = vm.SessionDate,
                location = vm.Location
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("api/examsessions", content);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"Create failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";

                vm.Exams = await LoadExamsAsync(client);
                ViewBag.Exams = new SelectList(vm.Exams, "Id", "Name", vm.ExamId);
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
    // GET: /ExamSessions/Edit/5
    // =========================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/examsessions/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<ExamSessionDetailsViewModel>(json, JsonOpts);
            if (dto == null) return NotFound();

            var vm = new ExamSessionEditViewModel
            {
                Id = dto.Id,
                ExamId = dto.ExamId,
                SessionDate = dto.SessionDate.Date,
                Location = dto.Location
            };

            vm.Exams = await LoadExamsAsync(client);
            ViewBag.Exams = new SelectList(vm.Exams, "Id", "Name", vm.ExamId);

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
    public async Task<IActionResult> Edit(int id, ExamSessionEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            try
            {
                var client = Api();
                vm.Exams = await LoadExamsAsync(client);
                ViewBag.Exams = new SelectList(vm.Exams, "Id", "Name", vm.ExamId);
            }
            catch { /* ignore */ }

            return View(vm);
        }

        try
        {
            var client = Api();

            var payload = new
            {
                examId = vm.ExamId,
                sessionDate = vm.SessionDate,
                location = vm.Location
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PutAsync($"api/examsessions/{id}", content);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"Update failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";

                vm.Exams = await LoadExamsAsync(client);
                ViewBag.Exams = new SelectList(vm.Exams, "Id", "Name", vm.ExamId);
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

            var resp = await client.DeleteAsync($"api/examsessions/{id}");

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
