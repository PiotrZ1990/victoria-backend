using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Documents;

namespace Victoria.Web.Controllers.Documents;

public class DocumentsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public DocumentsController(IHttpClientFactory httpClientFactory)
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

    // =========================
    // UPLOAD (GET)
    // /Documents/UploadStudy?studyApplicationId=1
    // =========================
    [HttpGet]
    public IActionResult UploadStudy(int studyApplicationId)
    {
        try
        {
            _ = Api();
            return View(new DocumentUploadStudyViewModel
            {
                StudyApplicationId = studyApplicationId,
                Title = "Passport scan"
            });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // UPLOAD (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadStudy(DocumentUploadStudyViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var client = Api();

            using var form = new MultipartFormDataContent();

            // UWAGA: nazwy pól muszą pasować do DocumentUploadRequest w backendzie
            form.Add(new StringContent(vm.Title), "Title");
            if (!string.IsNullOrWhiteSpace(vm.Description))
                form.Add(new StringContent(vm.Description), "Description");

            form.Add(new StringContent(vm.StudyApplicationId.ToString()), "StudyApplicationId");

            using var fileStream = vm.File.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(vm.File.ContentType ?? "application/octet-stream");
            form.Add(fileContent, "File", vm.File.FileName);

            var resp = await client.PostAsync("api/documents/upload", form);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"Upload failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";
                return View(vm);
            }

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            int documentId;

            if (doc.RootElement.TryGetProperty("documentId", out var p1))
                documentId = p1.GetInt32();
            else if (doc.RootElement.TryGetProperty("id", out var p2))
                documentId = p2.GetInt32();
            else
                throw new Exception("Upload OK, but response does not contain documentId/id");

            TempData["Success"] = "Uploaded.";
            return RedirectToAction("Details", new { id = documentId });

        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // DETAILS
    // /Documents/Details/7
    // =========================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/documents/{id}");
            if (!resp.IsSuccessStatusCode)
                return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<DocumentDetailsViewModel>(json, JsonOpts);

            if (vm == null) return NotFound();

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // APPROVE
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.PostAsync($"api/documents/{id}/approve", null);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"Approve failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";
            }
            else TempData["Success"] = "Approved.";

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // REJECT
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string reason)
    {
        try
        {
            var client = Api();

            var payload = new { reason };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await client.PostAsync($"api/documents/{id}/reject", content);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"Reject failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";
            }
            else TempData["Success"] = "Rejected.";

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpGet("Documents/Study")]
    public async Task<IActionResult> Study(int studyApplicationId)

    {
        try
        {
            var client = Api(); // ten helper z JWT

            var resp = await client.GetAsync($"api/documents/studyapplication/{studyApplicationId}");
            if (!resp.IsSuccessStatusCode) throw new Exception("Failed to load documents");

            var json = await resp.Content.ReadAsStringAsync();

            var list = JsonSerializer.Deserialize<List<DocumentListViewModel>>(
                json, JsonOpts) ?? new();

            ViewBag.StudyApplicationId = studyApplicationId;
            return View(list);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // LIST: VisaApplication docs
    // GET: /Documents/Visa?visaApplicationId=1
    // =========================
    [HttpGet("Documents/Visa")]
    public async Task<IActionResult> Visa(int visaApplicationId)

    {
        try
        {
            var client = Api();

            var resp = await client.GetAsync($"api/documents/visaapplication/{visaApplicationId}");
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Failed to load documents from API");

            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<DocumentListViewModel>>(json, JsonOpts) ?? new();

            ViewBag.VisaApplicationId = visaApplicationId;
            return View(list);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // UPLOAD: Visa (GET)
    // GET: /Documents/UploadVisa?visaApplicationId=1
    // =========================
    [HttpGet]
    public IActionResult UploadVisa(int visaApplicationId)
    {
        try
        {
            _ = Api();

            return View(new DocumentUploadVisaViewModel
            {
                VisaApplicationId = visaApplicationId,
                Title = "Passport"
            });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // UPLOAD: Visa (POST)
    // POST: /Documents/UploadVisa
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadVisa(DocumentUploadVisaViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var client = Api();

            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(vm.Title ?? ""), "Title");
            if (!string.IsNullOrWhiteSpace(vm.Description))
                form.Add(new StringContent(vm.Description), "Description");

            form.Add(new StringContent(vm.VisaApplicationId.ToString()), "VisaApplicationId");

            using var fileStream = vm.File.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(vm.File.ContentType);

            form.Add(fileContent, "File", vm.File.FileName);

            var resp = await client.PostAsync("api/documents/upload", form);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                ViewBag.Error = $"Upload failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";
                return View(vm);
            }

            TempData["Success"] = "Document uploaded.";
            return RedirectToAction(nameof(Visa), new { visaApplicationId = vm.VisaApplicationId });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

}
