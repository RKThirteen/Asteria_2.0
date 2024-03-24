using Asteria.Data;
using Asteria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Asteria.Controllers
{
    public class MessagesController : Controller
    {

        private readonly ApplicationDbContext db;
        
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public MessagesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
         }
        public IActionResult Chat(string id)
        {
            //var messages = db.Messages.Where(m => );
            return View();
        }

        public async Task<IActionResult> SendMessage(string id)
        {
            return View();
        }

        public async Task<IActionResult> DeleteMessage(string id)
        {
            return View();
        }
    }
}
