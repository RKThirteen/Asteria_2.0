using Asteria.Data;
using Asteria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Asteria.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (User.IsInRole("Admin") || comm.UserId == _userManager.GetUserId(User))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                TempData["message"] = "Comentariu sters cu succes";
                TempData["messageType"] = "alert-success";
                return Redirect("/Posts/Show/" + comm.PostId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti comentariul altui user";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Posts/Show/" + comm.PostId);
            }

        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (User.IsInRole("Admin") || comm.UserId == _userManager.GetUserId(User))
            {
                ViewBag.Comment = comm;
                return View();
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul altui user";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Posts/Show/" + comm.PostId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);
            if (User.IsInRole("Admin") || comm.UserId == _userManager.GetUserId(User))
            {
                try
                {

                    comm.Content = requestComment.Content;

                    db.SaveChanges();

                    TempData["message"] = "Comentariu editat cu succes";
                    TempData["messageType"] = "alert-success";

                    return Redirect("/Posts/Show/" + comm.PostId);
                }
                catch (Exception e)
                {
                    return Redirect("/Posts/Show/" + comm.PostId);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul altui user";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Posts/Show/" + comm.PostId);
            }


        }
    }
}
