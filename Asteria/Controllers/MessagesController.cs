using Microsoft.AspNetCore.Mvc;

namespace Asteria.Controllers
{
    public class MessagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
