using Microsoft.AspNetCore.Mvc;

namespace Asteria.Controllers
{
    public class CollectionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
