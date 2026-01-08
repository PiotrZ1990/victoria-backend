using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Visa;

namespace Victoria.Web.Controllers.Visa;

public class VisaApplicationsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public VisaApplicationsController(IHttpClientFactory httpClientFactory)
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

    // LIST (by CaseFile)
    // GET: /VisaApplications?caseFileId=1
    [HttpGet]
    public async Task<IActionResult> Index(int caseFileId)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/visaapplications/by-casefile/{caseFileId}");
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Failed to load VisaApplications from API");

            var json = await resp.Content.ReadAsStringAsync();

            var list = JsonSerializer.Deserialize<List<VisaApplicationListViewModel>>(
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
    // GET: /VisaApplications/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/visaapplications/{id}");
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            var json = await resp.Content.ReadAsStringAsync();

            var item = JsonSerializer.Deserialize<VisaApplicationDetailsViewModel>(
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
    // GET: /VisaApplications/Create?caseFileId=1
    [HttpGet]
    public IActionResult Create(int caseFileId)
    {
        try
        {
            _ = Api();
            return View(new VisaApplicationCreateViewModel
            {
                CaseFileId = caseFileId,
                Country = "UK",
                VisaType = "Student",
                Status = "Draft"
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
    public async Task<IActionResult> Create(VisaApplicationCreateViewModel vm)
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
                visaType = vm.VisaType,
                status = vm.Status,
                appointmentDate = vm.AppointmentDate,
                decisionDate = vm.DecisionDate
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await client.PostAsync("api/visaapplications/from-casefile", content);

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

    // EDIT (GET) - linkujemy tylko z Details
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/visaapplications/{id}");
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<VisaApplicationDetailsViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (item == null) return NotFound();

            var vm = new VisaApplicationEditViewModel
            {
                Id = item.Id,
                Country = item.Country,
                VisaType = item.VisaType,
                Status = item.Status,
                AppointmentDate = item.AppointmentDate,
                DecisionDate = item.DecisionDate
            };

            ViewBag.CaseFileId = item.CaseFileId;
            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // EDIT (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VisaApplicationEditViewModel vm, int caseFileId)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CaseFileId = caseFileId;
            return View(vm);
        }

        try
        {
            var client = Api();

            var payload = new
            {
                country = vm.Country,
                visaType = vm.VisaType,
                status = vm.Status,
                appointmentDate = vm.AppointmentDate,
                decisionDate = vm.DecisionDate
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await client.PutAsync($"api/visaapplications/{id}", content);

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = "Update failed";
                ViewBag.CaseFileId = caseFileId;
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

    // DELETE (POST) - odpalamy z Details
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int caseFileId)
    {
        try
        {
            var client = Api();

            var resp = await client.DeleteAsync($"api/visaapplications/{id}");

            if (!resp.IsSuccessStatusCode)
                TempData["Error"] = "Delete failed";
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
