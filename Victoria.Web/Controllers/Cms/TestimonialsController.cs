using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Cms;

namespace Victoria.Web.Controllers.Cms;

public class TestimonialsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public TestimonialsController(IHttpClientFactory httpClientFactory)
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

    // GET: /Testimonials
    [HttpGet]
    public async Task<IActionResult> Index(string? sort = "created_desc")
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync("api/testimonials");
            if (!resp.IsSuccessStatusCode) throw new Exception("Failed");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<TestimonialListViewModel>>(json, JsonOpts) ?? new();

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

    // GET: /Testimonials/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync($"api/testimonials/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var vm = JsonSerializer.Deserialize<TestimonialDetailsViewModel>(json, JsonOpts);
            if (vm == null) return NotFound();

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // GET: /Testimonials/Create
    [HttpGet]
    public IActionResult Create()
    {
        try
        {
            _ = Api();
            return View(new TestimonialCreateViewModel());
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // POST: /Testimonials/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestimonialCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                authorName = vm.AuthorName,
                authorTitle = vm.AuthorTitle,
                country = vm.Country,
                content = vm.Content,
                rating = vm.Rating,
                isPublished = vm.IsPublished
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("api/testimonials", content);

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

    // GET: /Testimonials/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.GetAsync($"api/testimonials/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<TestimonialDetailsViewModel>(json, JsonOpts);
            if (dto == null) return NotFound();

            var vm = new TestimonialEditViewModel
            {
                Id = dto.Id,
                AuthorName = dto.AuthorName,
                AuthorTitle = dto.AuthorTitle,
                Country = dto.Country,
                Content = dto.Content,
                Rating = dto.Rating,
                IsPublished = dto.IsPublished
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // POST: /Testimonials/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TestimonialEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var client = Api();

            var payload = new
            {
                authorName = vm.AuthorName,
                authorTitle = vm.AuthorTitle,
                country = vm.Country,
                content = vm.Content,
                rating = vm.Rating,
                isPublished = vm.IsPublished
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PutAsync($"api/testimonials/{id}", content);

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

    // POST: /Testimonials/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var client = Api();
            var resp = await client.DeleteAsync($"api/testimonials/{id}");

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
