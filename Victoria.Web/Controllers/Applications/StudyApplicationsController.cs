using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Applications;

namespace Victoria.Web.Controllers.Applications;

public class StudyApplicationsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public StudyApplicationsController(IHttpClientFactory httpClientFactory)
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

    // LIST dla CaseFile
    // GET: /StudyApplications?caseFileId=1
    [HttpGet]
    public async Task<IActionResult> Index(int caseFileId)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/StudyApplications/casefile/{caseFileId}");
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Failed to load StudyApplications from API");

            var json = await resp.Content.ReadAsStringAsync();

            var list = JsonSerializer.Deserialize<List<StudyApplicationListViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            ViewBag.CaseFileId = caseFileId;
            return View(list);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // DETAILS
    // GET: /StudyApplications/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/StudyApplications/{id}");
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            var json = await resp.Content.ReadAsStringAsync();

            var item = JsonSerializer.Deserialize<StudyApplicationDetailsViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (item == null) return NotFound();

            return View(item);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // CREATE (GET)
    // GET: /StudyApplications/Create?caseFileId=1
    [HttpGet]
    public IActionResult Create(int caseFileId)
    {
        try
        {
            _ = Api();

            return View(new StudyApplicationCreateViewModel
            {
                CaseFileId = caseFileId,
                Country = "UK"
            });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudyApplicationCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                caseFileId = vm.CaseFileId,
                country = vm.Country,
                universityName = vm.UniversityName,
                programName = vm.ProgramName
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await client.PostAsync("api/StudyApplications", content);

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = "Create failed";
                return View(vm);
            }

            TempData["Success"] = "Created";
            return RedirectToAction(nameof(Index), new { caseFileId = vm.CaseFileId });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // EDIT (GET)
    // GET: /StudyApplications/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/StudyApplications/{id}");
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            var json = await resp.Content.ReadAsStringAsync();

            var item = JsonSerializer.Deserialize<StudyApplicationDetailsViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (item == null) return NotFound();

            var vm = new StudyApplicationEditViewModel
            {
                Id = item.Id,
                CaseFileId = item.CaseFileId,
                Country = item.Country,
                UniversityName = item.UniversityName,
                ProgramName = item.ProgramName,
                Status = item.Status
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // EDIT (POST)
    // POST: /StudyApplications/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StudyApplicationEditViewModel vm)
    {
        if (id != vm.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                country = vm.Country,
                universityName = vm.UniversityName,
                programName = vm.ProgramName,
                status = vm.Status
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await client.PutAsync($"api/StudyApplications/{id}", content);

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Update failed ({(int)resp.StatusCode})";
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

    // DELETE (POST)
    // POST: /StudyApplications/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int caseFileId)
    {
        try
        {
            var client = Api();

            var resp = await client.DeleteAsync($"api/StudyApplications/{id}");

            if (!resp.IsSuccessStatusCode)
                TempData["Error"] = $"Delete failed ({(int)resp.StatusCode})";
            else
                TempData["Success"] = "Deleted";

            return RedirectToAction(nameof(Index), new { caseFileId });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
