using Asteria.Data;
using Asteria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Ganss.Xss;

namespace Asteria.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PostsController(
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

            var search = "";
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {

                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                var dateTimeSearch = DateTime.Parse(search);
                List<int> postsID = db.Posts.Where(
                                     bm => bm.Title.Contains(search) || bm.Description.Contains(search)
                                     || DateTime.Compare(bm.Date, dateTimeSearch) > 0
                                      ).Select(bm => bm.Id).ToList();

                var posts = db.Posts.Where(post => postsID.Contains(post.Id))
                              .Include("User")
                              .OrderBy(p => p.Title);
                ViewBag.Posts = posts;
                return View();
            }
            ViewBag.SearchString = search;
            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Posts/Index/?search="
                + search;

            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Posts/Index";
            }
            return View();
        }

        public IActionResult Show(int id)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.messageType = TempData["messageType"].ToString();
            }
            Post post = db.Posts.Include("User")
                                .Include("Comments")
                                .Include("Comments.User")
                                .Where(bok => bok.Id == id)
                                .First();

            ViewBag.collections = db.Collections.Where(b => b.UserId == _userManager.GetUserId(User))
                                .ToList();

            SetAccessRights();


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult AddCollection([FromForm] PostCollection postCollection)
        {
            if (ModelState.IsValid)
            {
                if (db.PostCollections
                   .Where(ab => ab.PostId == postCollection.PostId)
                   .Where(ab => ab.CollectionId == postCollection.CollectionId)
                   .Count() > 0)
                {
                    TempData["message"] = "Post is already in that category";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    db.PostCollections.Add(postCollection);
                    db.SaveChanges();

                    TempData["message"] = "Post added to selected collection";
                    TempData["messageType"] = "alert-success";
                }

            }
            else
            {
                TempData["message"] = "Couldn't add post";
                TempData["messageType"] = "alert-danger";
            }
            return Redirect("/Posts/Show/" + postCollection.PostId);
        }


        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show([FromForm] Comment comm)
        {
            comm.Date = DateTime.Now;
            comm.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comm);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comm.PostId);
            }
            else
            {
                Post post = db.Posts.Include("User")
                                .Include("Comments")
                                .Include("Comments.User")
                                .Where(p => p.Id == comm.PostId)
                                .First();


                SetAccessRights();

                return View(post);
            }

        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Post bm = new Post();

            bm.Categ = GetUserCategories();

            return View(bm);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Post post)
        {
            var sanitizer = new HtmlSanitizer();
            if (ModelState.IsValid)
            {

                post.UserId = _userManager.GetUserId(User);
                post.Date = DateTime.Now;
                post.Likes = 0;
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "Added Post!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                post.Categ = GetUserCategories();
                return View(post);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Post post = db.Posts.Where(bok => bok.Id == id).First();

            post.Categ = GetUserCategories();
            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                return View(post);
            else
            {
                TempData["message"] = "You can't modify a post that isn't yours!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Post requestPost)
        {
            Post post = db.Posts.Find(id);

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {

                    post.Title = requestPost.Title;
                    post.Description = requestPost.Description;
                    post.Content = requestPost.Content;
                    db.SaveChanges();
                    TempData["message"] = "Post Modified!";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }

                else
                {
                    requestPost.Categ = GetUserCategories();
                    return RedirectToAction("Edit", id);

                }
            }

            else
            {
                TempData["message"] = "You can't modify a post that isn't yours!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Include("Comments")
                                         .Where(bok => bok.Id == id)
                                         .First();

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (post.Comments.Count > 0)
                {
                    foreach (var comment in post.Comments)
                    {
                        db.Comments.Remove(comment);
                    }
                }

                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["message"] = "Post Deleted";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["message"] = "You can't delete a post that isn't yours!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }


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

        [NonAction]
        public IEnumerable<SelectListItem> GetUserCategories()
        {

            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Collections
                             where cat.UserId == _userManager.GetUserId(User)
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CollectionName.ToString()
                });
            }
            return selectList;
        }
    }
}
