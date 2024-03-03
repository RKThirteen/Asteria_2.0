using Asteria.Data;
using Asteria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Asteria.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CollectionsController(
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
        public IActionResult Index()
        {

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.messageType = TempData["messageType"].ToString();
            }

            if (User.IsInRole("Admin"))
            {
                var categories = from category in db.Collections.Include("User")
                                 orderby category.CollectionName
                                 select category;
                ViewBag.Categories = categories;

            }
            else if (User.IsInRole("User"))
            {
                var categories = from category in db.Collections.Include("User")
                                 .Where(col => col.UserId == _userManager.GetUserId(User))
                                 orderby category.CollectionName
                                 select category;
                ViewBag.Categories = categories;

            }
            return View();

        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            SetAccessRights();
            Collection category = db.Collections.Find(id);
            if (User.IsInRole("User"))
            {
                var categories = db.Collections
                                .Include("PostCollections.Post.User")
                                .Include("User")
                                .Where(b => b.Id == id)
                                .Where(b => b.UserId == _userManager.GetUserId(User))
                                .FirstOrDefault();

                if (categories == null)
                {
                    TempData["message"] = "You don't have such a collection";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Posts");
                }

                return View(categories);
            }
            else
            if (User.IsInRole("Admin"))
            {
                var categories = db.Collections
                                .Include("PostCollections.Post.User")
                                .Include("User")
                                .Where(b => b.Id == id)
                                .FirstOrDefault();

                if (categories == null)
                {
                    TempData["message"] = "Resource doesn't exist";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Posts");
                }

                return View(categories);
            }

            else
            {
                TempData["message"] = "You must be logged in to see this page";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Posts/Index");
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult New(Collection col)
        {
            if (ModelState.IsValid)
            {
                col.UserId = _userManager.GetUserId(User);
                db.Collections.Add(col);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adaugata";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                return View(col);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Collection collection = db.Collections.Find(id);
            if (collection.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(collection);
            }
            else
            {
                TempData["message"] = "Categoria pe care ati incercat sa o editati nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Collection requestCollection)
        {
            Collection collection = db.Collections.Find(id);
            if (collection.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {

                    collection.CollectionName = requestCollection.CollectionName;
                    collection.Description = requestCollection.Description;
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost modificata!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCollection);
                }
            }
            else
            {
                TempData["message"] = "Categoria pe care ati incercat sa o editati nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]

        public IActionResult Delete(int id)
        {
            Collection collection = db.Collections.Include("PostCollections")
                                         .Where(cat => cat.Id == id)
                                         .First();
            if (collection.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (collection.PostCollections.Count > 0)
                {
                    foreach (var bmcat in collection.PostCollections)
                        db.PostCollections.Remove(bmcat);
                }
                db.Collections.Remove(collection);
                TempData["message"] = "Collection deleted successfully";
                TempData["messageType"] = "alert-success";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Can't delete someone else's category";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult RemoveFromCollection(int postId, int collectionId)
        {
            PostCollection pcl = db.PostCollections
                                  .Where(ab => ab.PostId == postId)
                                  .Where(ab => ab.CollectionId == collectionId)
                                  .First();

            var collections = db.Collections
                                .Include("PostCollections.Post.User")
                                .Include("User")
                                .Where(b => b.Id == collectionId)
                                .Where(b => b.UserId == _userManager.GetUserId(User))
                                .FirstOrDefault();

            if (User.IsInRole("Admin") || collections.UserId == _userManager.GetUserId(User))
            {

                db.PostCollections.Remove(pcl);
                TempData["message"] = "Post removed from category";
                TempData["messageType"] = "alert-success";
                db.SaveChanges();

            }
            else
            {
                TempData["message"] = "Can't delete this post from this category";
                TempData["messageType"] = "alert-danger";
            }
            return Redirect("/Categories/Show/" + collectionId);

        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}
