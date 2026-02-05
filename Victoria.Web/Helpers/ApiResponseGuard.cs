using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Victoria.Web.Helpers;

public static class ApiResponseGuard
{
    public static IActionResult? HandleAuth(HttpStatusCode code)
    {
        if (code == HttpStatusCode.Unauthorized)
            return new RedirectToActionResult("Login", "Auth", null);

        if (code == HttpStatusCode.Forbidden)
            return new RedirectToActionResult("Forbidden", "Errors", null);

        return null;
    }
}
