using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Victoria.Web.Controllers;

public class AuthController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");

        var payload = new
        {
            email,
            password
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("api/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Nieprawidłowe dane logowania";
            return View();
        }

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var token = doc.RootElement.GetProperty("token").GetString();

        HttpContext.Session.SetString("JWT", token!);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JWT");
        return RedirectToAction("Login");
    }
}
