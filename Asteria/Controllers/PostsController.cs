using Microsoft.AspNetCore.Mvc;

namespace Asteria.Controllers
{
    public class PostsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
