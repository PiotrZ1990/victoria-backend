using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Victoria.Web.Models.Crm;

namespace Victoria.Web.Controllers.Crm;

public class LeadsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LeadsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // =========================================
    // HELPERS
    // =========================================
    private HttpClient CreateApiClientWithJwt()
    {
        var token = HttpContext.Session.GetString("JWT");

        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException("Brak JWT w sesji. Zaloguj się ponownie.");

        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // =========================================
    // INDEX (LIST)
    // GET: /Leads
    // =========================================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = CreateApiClientWithJwt();

            var response = await client.GetAsync("api/leads");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Auth");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var leads = JsonSerializer.Deserialize<List<LeadViewModel>>(json, JsonOptions) ?? new();

            // sortowanie po dacie po stronie web (szybko)
            leads = leads.OrderByDescending(x => x.CreatedAt).ToList();

            return View(leads);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // DETAILS
    // GET: /Leads/Details/5
    // =========================================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = CreateApiClientWithJwt();

            var response = await client.GetAsync($"api/leads/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Auth");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var lead = JsonSerializer.Deserialize<LeadViewModel>(json, JsonOptions);

            if (lead == null)
                return NotFound();

            return View(lead);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // DELETE
    // POST: /Leads/Delete/5
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var client = CreateApiClientWithJwt();

            var response = await client.DeleteAsync($"api/leads/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Auth");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Delete failed: {(int)response.StatusCode} {response.ReasonPhrase}";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Lead deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }


    // =========================================
    // CREATE (FORM)
    // GET: /Leads/Create
    // =========================================
    [HttpGet]
    public IActionResult Create()
    {
        // pusty formularz
        return View();
    }

    // =========================================
    // CREATE (SUBMIT)
    // POST: /Leads/Create
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeadCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var client = CreateApiClientWithJwt();

            var payload = new
            {
                fullName = vm.FullName,
                email = vm.Email,
                phoneNumber = vm.PhoneNumber,
                source = vm.Source,
                interestedCountry = vm.InterestedCountry,
                interestedService = vm.InterestedService
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("api/leads", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Auth");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Błąd API: {(int)response.StatusCode} {response.ReasonPhrase}";
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // EDIT (FORM)
    // GET: /Leads/Edit/5
    // =========================================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = CreateApiClientWithJwt();

            var response = await client.GetAsync($"api/leads/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Auth");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var lead = JsonSerializer.Deserialize<LeadViewModel>(json, JsonOptions);

            if (lead == null)
                return NotFound();

            // map do view model edycji
            var vm = new LeadEditViewModel
            {
                Id = lead.Id,
                FullName = lead.FullName,
                Email = lead.Email,
                PhoneNumber = lead.PhoneNumber,
                Source = lead.Source,
                InterestedCountry = lead.InterestedCountry,
                InterestedService = lead.InterestedService,
                Status = lead.Status
            };

            return View(vm);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // =========================================
    // EDIT (SUBMIT)
    // POST: /Leads/Edit/5
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LeadEditViewModel vm)
    {
        if (id != vm.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var client = CreateApiClientWithJwt();

            // ⚠️ API musi mieć endpoint PUT/PATCH dla leadów.
            // Jeżeli go jeszcze nie masz w backendzie, to teraz zrobimy go jako następny krok.
            var payload = new
            {
                fullName = vm.FullName,
                email = vm.Email,
                phoneNumber = vm.PhoneNumber,
                source = vm.Source,
                interestedCountry = vm.InterestedCountry,
                interestedService = vm.InterestedService,
                status = vm.Status
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync($"api/leads/{id}", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Auth");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Błąd API: {(int)response.StatusCode} {response.ReasonPhrase}";
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
    // =========================================
    // CONVERT LEAD → CASE FILE
    // POST: /Leads/ConvertToCaseFile/{id}
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConvertToCaseFile(int id)
    {
        try
        {
            var client = CreateApiClientWithJwt();

            var payload = new
            {
                leadId = id,
                internalNotes = "Converted from CRM (Web)"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("api/casefiles/from-lead", content);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Auth");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to convert lead to case file.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Lead converted to Case File successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

}
