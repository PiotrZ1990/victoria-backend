using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Cms;

namespace Victoria.Web.Controllers.Cms;

public class TeachersController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public TeachersController(IHttpClientFactory httpClientFactory)
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
    // HELPER: upload photo if provided
    // returns filePath (e.g. uploads/2026/01/xxx.png) or null
    // =========================
    private async Task<string?> UploadTeacherPhotoIfAny(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return null;

        var client = Api();

        using var form = new MultipartFormDataContent();

        await using var fileStream = file.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

        // backend expects: [FromForm] IFormFile file
        form.Add(fileContent, "file", file.FileName);

        var resp = await client.PostAsync("api/fileresources/upload-image", form);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Photo upload failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}");
        }

        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("filePath", out var p))
            return p.GetString();

        return null;
    }

    // =========================
    // LIST
    // =========================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync("api/teachers");
            if (!resp.IsSuccessStatusCode) throw new Exception("Failed");

            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<TeacherListViewModel>>(json, JsonOpts) ?? new();
            return View(list);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================
    // DETAILS
    // =========================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync($"api/teachers/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<TeacherDetailsViewModel>(json, JsonOpts);
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
    public IActionResult Create()
    {
        try
        {
            _ = Api();
            return View(new TeacherCreateViewModel());
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
    public async Task<IActionResult> Create(TeacherCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            // 1) upload photo (optional)
            var uploadedPath = await UploadTeacherPhotoIfAny(vm.PhotoFile);
            if (!string.IsNullOrWhiteSpace(uploadedPath))
                vm.PhotoUrl = uploadedPath;

            // 2) create teacher
            var client = Api();

            var payload = new
            {
                fullName = vm.FullName,
                title = vm.Title,
                email = vm.Email,
                phone = vm.Phone,
                bio = vm.Bio,
                photoUrl = vm.PhotoUrl,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("api/teachers", content);

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
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(vm);
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
            var resp = await client.GetAsync($"api/teachers/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<TeacherDetailsViewModel>(json, JsonOpts);
            if (dto == null) return NotFound();

            var vm = new TeacherEditViewModel
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Title = dto.Title,
                Email = dto.Email,
                Phone = dto.Phone,
                Bio = dto.Bio,
                PhotoUrl = dto.PhotoUrl,
                IsActive = dto.IsActive
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
    public async Task<IActionResult> Edit(int id, TeacherEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            // 1) upload new photo (optional)
            var uploadedPath = await UploadTeacherPhotoIfAny(vm.PhotoFile);
            if (!string.IsNullOrWhiteSpace(uploadedPath))
                vm.PhotoUrl = uploadedPath;

            // 2) update
            var client = Api();

            var payload = new
            {
                fullName = vm.FullName,
                title = vm.Title,
                email = vm.Email,
                phone = vm.Phone,
                bio = vm.Bio,
                photoUrl = vm.PhotoUrl,
                isActive = vm.IsActive
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PutAsync($"api/teachers/{id}", content);

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
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(vm);
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
            var resp = await client.DeleteAsync($"api/teachers/{id}");

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = $"Delete failed: {(int)resp.StatusCode} {resp.StatusCode}. {body}";
            }
            else TempData["Success"] = "Deleted";

            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
