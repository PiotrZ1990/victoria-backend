using Microsoft.AspNetCore.Mvc;

namespace Victoria.Web.Controllers.Crm
{
    public class CaseFilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
