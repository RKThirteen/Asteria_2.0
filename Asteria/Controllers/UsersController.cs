using Asteria.Data;
using Asteria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
            ApplicationUser user = db.Users.Find(id);

            List<ApplicationUser> FriendsList = new List<ApplicationUser>();
            if (user.Friends!=null)
            {
                FriendsList.AddRange(user.Friends);
            }
                
            ViewBag.F = FriendsList;
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
                List<ApplicationUser> Requests = new List<ApplicationUser>();
                Requests.AddRange(user.Pending);

                ViewBag.F = Requests;
                return View(user);
            }

        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string senderId, string receiverId)
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
                var receiver = db.Users.Where(us=>us.Id==receiverId).FirstOrDefault();
                var sender = db.Users.Where(us=>us.Id==senderId).FirstOrDefault();

                if (receiver.FriendRequests.Contains(sender) && receiver.FriendRequests != null)
                {
                    TempData["message"] = "You have already sent a request to this person";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Users/Show/" + receiverId);
                }
                else
                {
                    receiver.Pending.Add(sender);
                    sender.FriendRequests.Add(receiver);
                    
                    db.SaveChanges();
                    TempData["message"] = "Friend request sent!";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Users/Show/" + receiverId);
                }
            }
            
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(string senderId, string receiverId)
        {
            SetAccesRights();
            if (!User.IsInRole("Admin") && _userManager.GetUserId(User).ToString() != receiverId)
            {
                TempData["message"] = "Can't access this tab";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Posts/Index");
            }
            else
            {
                var receiver = db.Users.Find(receiverId);
                var sender = db.Users.Find(senderId);

                if (!receiver.FriendRequests.Contains(sender) && receiver.FriendRequests!=null)
                {
                    TempData["message"] = "You have no friend request from this person";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Users/Show/" + receiverId);
                }
                else
                {
                    receiver.Pending.Remove(sender);
                    sender.FriendRequests.Remove(receiver);
                    receiver.Friends.Add(sender);
                    sender.Friends.Add(receiver);
                    await _userManager.UpdateAsync(receiver);
                    await _userManager.UpdateAsync(sender);
                    db.SaveChanges();
                    TempData["message"] = "Friend request accepted!";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Users/Show/" + senderId);
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
                var receiver = db.Users.Find(receiverId);
                var sender = db.Users.Find(senderId);

                if (!receiver.FriendRequests.Contains(sender) && receiver.FriendRequests != null)
                { 
                    TempData["message"] = "You have no friend request from this person";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Users/Show/" + receiverId);
                }
                else
                {
                    receiver.Pending.Remove(sender);
                    sender.FriendRequests.Remove(receiver);
                    db.SaveChanges();
                    TempData["message"] = "Friend request rejected!";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Users/Show/" + senderId);
                }
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> RemoveFromFriends(string senderId, string receiverId)
        {
            SetAccesRights();
            if (!User.IsInRole("Admin") && _userManager.GetUserId(User).ToString() != receiverId)
            {
                TempData["message"] = "Can't access this tab";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("/Posts/Index");
            }
            else
            {
                var receiver = db.Users.Find(receiverId);
                var sender = db.Users.Find(senderId);
                receiver.Friends.Remove(sender);
                sender.Friends.Remove(receiver);
                db.SaveChanges();
                TempData["message"] = "User removed from friend list";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Users/Show/" + receiverId);
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
