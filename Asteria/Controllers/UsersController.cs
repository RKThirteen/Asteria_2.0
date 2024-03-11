using Asteria.Data;
using Asteria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing.Text;

namespace Asteria.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var users = from user in db.Users
                            orderby user.UserName
                            select user;
                ViewBag.UsersList = users;
                return View();
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul la resursa aceasta";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("/Posts/Index");
            }


        }

        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            SetAccesRights();
            if (User.IsInRole("Admin"))
            {
                var roles = await _userManager.GetRolesAsync(user);

                ViewBag.Roles = roles;
            }
            else
            {
                ViewBag.Roles = null;
            }
            var userposts = db.Users
                          .Include("Collections")
                          .Include("Collections.PostCollections")
                          .Include("Collections.PostCollections.Post.User")
                          .Where(u => u.Id == id)
                          .FirstOrDefault();

            List<Post> postList = new List<Post>();


            foreach (var categ in userposts.Collections)
                foreach (var bmcat in categ.PostCollections)
                    if (!postList.Contains(bmcat.Post))
                        postList.Add(bmcat.Post);


            ViewBag.UserPosts = postList;

            return View(userposts);
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Edit(string id)
        {
            if (User.IsInRole("Admin") || _userManager.GetUserId(User).ToString() == id)
            {
                ApplicationUser user = db.Users.Find(id);

                user.AllRoles = GetAllRoles();

                var roleNames = await _userManager.GetRolesAsync(user);

                var currentUserRole = _roleManager.Roles
                                                  .Where(r => roleNames.Contains(r.Name))
                                                  .Select(r => r.Id)
                                                  .First();
                ViewBag.UserRole = currentUserRole;

                return View(user);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul la resursa aceasta";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("/Bookmarks/Index");
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();

            if (User.IsInRole("Admin") || _userManager.GetUserId(User).ToString() == id)
            {
                if (ModelState.IsValid)
                {
                    user.Email = newData.Email;
                    user.FirstName = newData.FirstName;
                    user.LastName = newData.LastName;
                    user.PhoneNumber = newData.PhoneNumber;
                    user.Nickname = newData.Nickname;

                    if (Request.Form.Files.Count > 0)
                    {
                        IFormFile file = Request.Form.Files.FirstOrDefault();
                        using (var dataStream = new MemoryStream())
                        {
                            await file.CopyToAsync(dataStream);
                            user.ProfilePic = dataStream.ToArray();
                        }
                        await _userManager.UpdateAsync(user);
                    }


                    if (User.IsInRole("Admin"))
                    {
                        var roles = db.Roles.ToList();

                        foreach (var role in roles)
                        {
                            await _userManager.RemoveFromRoleAsync(user, role.Name);
                        }
                        var roleName = await _roleManager.FindByIdAsync(newRole);
                        await _userManager.AddToRoleAsync(user, roleName.ToString());
                    }
                    db.SaveChanges();
                    TempData["message"] = "Utilizator editat";
                    TempData["messageType"] = "alert-success";
                    if (User.IsInRole("Admin"))
                        return Redirect("/Users/Index");
                    else
                        return Redirect("/Users/Show/" + id);

                }
                else
                {

                    newData.AllRoles = GetAllRoles();

                    var roleNames = await _userManager.GetRolesAsync(newData);

                    var currentUserRole = _roleManager.Roles
                                                      .Where(r => roleNames.Contains(r.Name))
                                                      .Select(r => r.Id)
                                                      .First();
                    ViewBag.UserRole = currentUserRole;

                    return View(newData);
                }

            }
            else
            {
                TempData["message"] = "Nu aveti dreptul la resursa aceasta";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("/Bookmarks/Index");
            }


        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(string id)
        {
            if (User.IsInRole("Admin") && _userManager.GetUserId(User).ToString() != id)
            {
                var user = db.Users
                        .Include("Bookmarks")
                        .Include("Comments")
                        .Include("Categories")
                        .Where(u => u.Id == id)
                        .First();

                if (user.Comments.Count > 0)
                {
                    foreach (var comment in user.Comments)
                    {
                        db.Comments.Remove(comment);
                    }
                }

                if (user.Posts.Count > 0)
                {
                    foreach (var bookmark in user.Posts)
                    {
                        db.Posts.Remove(bookmark);
                    }
                }

                if (user.Collections.Count > 0)
                {
                    foreach (var category in user.Collections)
                    {
                        db.Collections.Remove(category);
                    }
                }

                db.ApplicationUsers.Remove(user);

                db.SaveChanges();
                TempData["message"] = "Utilizator sters";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Index");
            }
            else if (User.IsInRole("Admin") && _userManager.GetUserId(User).ToString() == id)
            {
                TempData["message"] = "Nu cred ca ai vrea sa te stergi singur";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul la resursa aceasta";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("/Bookmarks/Index");
            }

        }
        public async Task<ActionResult> FriendList(string id)
        {
            SetAccesRights();
            ApplicationUser user = db.Users.Find(id);
            var friends = db.Friends
                               .Include("Friendship")
                               .Where(fr => fr.FriendshipId == id)
                               .OrderBy(fr => fr.DateAccepted);
            ViewBag.Fr = friends;
            return View(user);
        }

        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> FriendRequests(string id)
        {

            SetAccesRights();
            if (!User.IsInRole("Admin") && _userManager.GetUserId(User).ToString() != id)
            {
                TempData["message"] = "Can't access this tab";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("/Posts/Index");
            }
            else
            {
                var user = db.Users.Find(id);
                var requests = db.FriendRequests
                               .Include("Sender") 
                               .Where(fr => fr.ReceiverId == id)
                               .OrderBy(fr => fr.DateSent);
                ViewBag.F = requests;
                return View(user);
            }

        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest([FromForm] FriendRequest fr)
        {
            SetAccesRights();
            if (!User.IsInRole("Admin") && _userManager.GetUserId(User).ToString() != fr.SenderId)
            {
                TempData["message"] = "Can't send friend request";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Posts/Index");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (db.FriendRequests
                       .Where(ab => ab.SenderId == fr.SenderId)
                       .Where(ab => ab.ReceiverId == fr.ReceiverId)
                       .Count() > 0)
                    {
                        TempData["message"] = "You have already sent a friend request to that person";
                        TempData["messageType"] = "alert-danger";
                        return Redirect("/Users/Show/" + fr.SenderId);
                    }
                    else
                    {
                        fr.DateSent = DateTime.Now;
                        db.FriendRequests.Add(fr);
                        db.SaveChanges();

                        TempData["message"] = "Friend request send";
                        TempData["messageType"] = "alert-success";
                        return Redirect("/Users/Show/" + fr.SenderId);
                    }

                }
                else
                {
                    //shouldn't get here
                    TempData["message"] = "Something went wrong with sending friend requests";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Users/Show/" + fr.SenderId);
                }
                
            }
            
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest([FromForm] Friend f)
        {
            SetAccesRights();
            if (!User.IsInRole("Admin") && _userManager.GetUserId(User).ToString() != f.UserId)
            {
                TempData["message"] = "Can't access this tab";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Posts/Index");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (db.Friends
                       .Where(ab => ab.UserId == f.UserId)
                       .Where(ab => ab.FriendshipId == f.FriendshipId)
                       .Count() > 0)
                    {
                        TempData["message"] = "You have already send a friend request to that person";
                        TempData["messageType"] = "alert-danger";
                        return Redirect("/Users/FriendRequests/" + f.UserId);
                    }
                    else
                    {
                        FriendRequest fr = db.FriendRequests
                                         .Where(fr => fr.SenderId == f.FriendshipId)
                                         .Where(fr => fr.ReceiverId == f.UserId)
                                         .FirstOrDefault();
                        f.DateAccepted = DateTime.Now;
                        db.Friends.Add(f);
                        db.FriendRequests.Remove(fr);
                        db.SaveChanges();

                        TempData["message"] = "Friend request send";
                        TempData["messageType"] = "alert-success";
                        return Redirect("/Users/FriendRequests/" + f.UserId);
                    }

                }
                else
                {
                    //shouldn't get here
                    TempData["message"] = "Something went wrong with sending friend requests";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Users/FriendRequests/" + f.UserId);
                }
            }

        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> RejectFriendRequest(string senderId, string receiverId)
        {
            SetAccesRights();
            if (!User.IsInRole("Admin") && _userManager.GetUserId(User).ToString() != senderId)
            {
                TempData["message"] = "Can't access this tab";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Posts/Index");
            }
            else
            {
                FriendRequest fr = db.FriendRequests
                                         .Where(fr => fr.SenderId == senderId)
                                         .Where(fr => fr.ReceiverId == receiverId)
                                         .FirstOrDefault();
                db.FriendRequests.Remove(fr);
                db.SaveChanges();
                TempData["message"] = "Friend request rejected";
                TempData["messageType"] = "alert-success";
                return Redirect("/Users/FriendRequests/"+receiverId);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> RemoveFromFriends(string userId, string friendshipId)
        {
            SetAccesRights();
            if (!User.IsInRole("Admin") && _userManager.GetUserId(User).ToString() != userId)
            {
                TempData["message"] = "Can't access this tab";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("/Posts/Index");
            }
            else
            {
                Friend friendship = db.Friends
                                  .Where(f => f.FriendshipId == friendshipId)
                                  .Where(f => f.UserId == userId)
                                  .FirstOrDefault();
                db.Remove(friendship);
                db.SaveChanges();
                TempData["message"] = "User removed from friend list";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Users/Show/" + userId);
            }

        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        public virtual void SetAccesRights()
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
