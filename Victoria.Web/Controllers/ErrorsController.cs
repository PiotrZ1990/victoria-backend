using Microsoft.AspNetCore.Mvc;

namespace Victoria.Web.Controllers;

public class ErrorsController : Controller
{
    [HttpGet]
    public IActionResult Forbidden()
    {
        Response.StatusCode = 403;
        return View();
    }

    [HttpGet]
    public IActionResult UnauthorizedAccess()
    {
        Response.StatusCode = 401;
        return View();
    }
}
