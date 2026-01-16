using Microsoft.AspNetCore.Mvc;

namespace Victoria.Web.Controllers;

public class HomeController : Controller
{
    private bool IsLogged()
    {
        var jwt = HttpContext.Session.GetString("JWT");
        return !string.IsNullOrWhiteSpace(jwt);
    }

    // "/" - jeśli zalogowany -> Dashboard, jeśli nie -> Landing
    [HttpGet]
    public IActionResult Index()
    {
        if (IsLogged())
            return RedirectToAction(nameof(Dashboard));

        return View(); // Views/Home/Index.cshtml
    }

    // ✅ Dashboard dopiero po zalogowaniu
    [HttpGet]
    public IActionResult Dashboard()
    {
        if (!IsLogged())
            return RedirectToAction("Login", "Auth");

        return View(); // Views/Home/Dashboard.cshtml
    }

    // ✅ Procesy też tylko po zalogowaniu
    [HttpGet]
    public IActionResult Processes()
    {
        if (!IsLogged())
            return RedirectToAction("Login", "Auth");

        return View(); // Views/Home/Processes.cshtml
    }

    [HttpGet]
    public IActionResult Cruds()
    {
        if (!IsLogged())
            return RedirectToAction("Login", "Auth");

        return View();
    }

}
