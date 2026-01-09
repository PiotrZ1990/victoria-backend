using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Cases;
using Victoria.Web.Models.Payments;

namespace Victoria.Web.Controllers.Cases;

public class CaseFilesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CaseFilesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // =========================
    // Helper: API client + JWT
    // =========================
    private HttpClient Api()
    {
        var token = HttpContext.Session.GetString("JWT");
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException();

        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // =========================
    // LIST
    // GET: /CaseFiles
    // =========================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = Api();

            var response = await client.GetAsync("api/casefiles");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to load case files from API");

            var json = await response.Content.ReadAsStringAsync();
            var caseFiles = JsonSerializer.Deserialize<List<CaseFileViewModel>>(json, JsonOpts) ?? new();

            return View(caseFiles);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // DETAILS
    // GET: /CaseFiles/Details/5
    // =========================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();

            // 1) CaseFile
            var response = await client.GetAsync($"api/casefiles/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<CaseFileDetailsViewModel>(json, JsonOpts);

            if (vm == null)
                return NotFound();

            // 2) Invoices for case
            var invoicesResponse = await client.GetAsync($"api/invoices/casefile/{id}");
            if (invoicesResponse.IsSuccessStatusCode)
            {
                var invoicesJson = await invoicesResponse.Content.ReadAsStringAsync();
                var invoices = JsonSerializer.Deserialize<List<InvoiceViewModel>>(invoicesJson, JsonOpts) ?? new();
                vm.Invoices = invoices;
            }
            else
            {
                vm.Invoices = new();
            }

            // 3) Readiness
            var readinessResp = await client.GetAsync($"api/casefiles/{id}/readiness");
            if (readinessResp.IsSuccessStatusCode)
            {
                var readinessJson = await readinessResp.Content.ReadAsStringAsync();
                var readiness = JsonSerializer.Deserialize<CaseReadinessViewModel>(readinessJson, JsonOpts);

                ViewBag.Readiness = readiness;
                ViewBag.StageUpdate = new CaseStageUpdateViewModel
                {
                    CaseFileId = id,
                    Stage = readiness?.Stage ?? vm.Stage
                };
            }

            // 4) Update form (notes + stage)
            ViewBag.UpdateVm = new CaseFileUpdateViewModel
            {
                Id = vm.Id,
                Stage = vm.Stage,
                InternalNotes = vm.InternalNotes
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // UPDATE (notes + stage) -> PUT api/casefiles/{id}
    // POST: /CaseFiles/Update
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(CaseFileUpdateViewModel vm)
    {
        try
        {
            var client = Api();

            var payload = new
            {
                stage = vm.Stage,
                internalNotes = vm.InternalNotes
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/casefiles/{vm.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Update failed: {(int)response.StatusCode} {response.StatusCode}. {body}";
                return RedirectToAction(nameof(Details), new { id = vm.Id });
            }

            TempData["Success"] = "Case updated.";
            return RedirectToAction(nameof(Details), new { id = vm.Id });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // CHANGE STAGE (business rules) -> PUT api/casefiles/{id}/stage
    // POST: /CaseFiles/ChangeStage
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStage(CaseStageUpdateViewModel vm)
    {
        try
        {
            var client = Api();

            var payload = new { stage = vm.Stage };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await client.PutAsync($"api/casefiles/{vm.CaseFileId}/stage", content);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"Stage change failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";
                return RedirectToAction(nameof(Details), new { id = vm.CaseFileId });
            }

            TempData["Success"] = "Stage updated.";
            return RedirectToAction(nameof(Details), new { id = vm.CaseFileId });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
