using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Victoria.Web.Helpers;
using Victoria.Web.Models.Admin;

namespace Victoria.Web.Controllers;

public class AdminUsersController : Controller
{
    private readonly IHttpClientFactory _http;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AdminUsersController(IHttpClientFactory http)
    {
        _http = http;
    }

    private HttpClient Api()
    {
        var client = _http.CreateClient("BackendApi").WithJwt(HttpContext.Session);
        return client;
    }

    // GET: /AdminUsers
    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JWT")))
            return RedirectToAction("Login", "Auth");

        var resp = await Api().GetAsync("api/admin/users");
        if (!resp.IsSuccessStatusCode)
            return Content($"Backend error: {(int)resp.StatusCode}\n{await resp.Content.ReadAsStringAsync()}");

        var json = await resp.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List<AdminUserListVm>>(json, JsonOpts) ?? new();

        return View(list);
    }

    // GET: /AdminUsers/Edit/{id}
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JWT")))
            return RedirectToAction("Login", "Auth");

        var userResp = await Api().GetAsync($"api/admin/users/{id}");
        if (!userResp.IsSuccessStatusCode)
            return Content($"Backend error: {(int)userResp.StatusCode}\n{await userResp.Content.ReadAsStringAsync()}");

        var rolesResp = await Api().GetAsync("api/admin/users/roles");
        if (!rolesResp.IsSuccessStatusCode)
            return Content($"Backend error: {(int)rolesResp.StatusCode}\n{await rolesResp.Content.ReadAsStringAsync()}");

        var userJson = await userResp.Content.ReadAsStringAsync();
        var rolesJson = await rolesResp.Content.ReadAsStringAsync();

        var user = JsonSerializer.Deserialize<AdminUserListVm>(userJson, JsonOpts)!;
        var allRoles = JsonSerializer.Deserialize<List<string>>(rolesJson, JsonOpts) ?? new();

        var vm = new AdminUserEditVm
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            StudentId = user.StudentId,
            AllRoles = allRoles,
            SelectedRoles = user.Roles
        };

        return View(vm);
    }

    // POST: /AdminUsers/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AdminUserEditVm vm)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JWT")))
            return RedirectToAction("Login", "Auth");

        vm.SelectedRoles ??= new List<string>();

        var payload = new
        {
            roles = vm.SelectedRoles
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var resp = await Api().PutAsync($"api/admin/users/{vm.Id}/roles", content);
        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.Error = await resp.Content.ReadAsStringAsync();

            // doładuj role żeby widok nie był pusty
            var rolesResp = await Api().GetAsync("api/admin/users/roles");
            var rolesJson = await rolesResp.Content.ReadAsStringAsync();
            vm.AllRoles = JsonSerializer.Deserialize<List<string>>(rolesJson, JsonOpts) ?? new();

            return View(vm);
        }

        TempData["Success"] = "Roles updated ✅";
        return RedirectToAction(nameof(Index));
    }
}
