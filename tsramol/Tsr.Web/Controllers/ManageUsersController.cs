using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tsr.Core.Entities;
using Tsr.Core.Models;
using Tsr.Infra;
using Tsr.Web.Models;

namespace Tsr.Web.Controllers
{
    public class ManageUsersController : Controller
    {
        ApplicationDbContext db;
        AppContext ndb;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

#region Users
        public ActionResult UserLists()
        {
            db = new ApplicationDbContext();

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>
               (db));

            var users = db.Users.ToList();

            var viewModels = new List<UserlistViewModel>();

            foreach (var user in users)
            {
                var s = UserManager.GetRoles(user.Id);
                string rolename = string.Empty;
                try { rolename = s[0].ToString(); }
                catch (Exception) { rolename = "None"; }

                viewModels.Add(new UserlistViewModel { Id = user.Id, Name = user.UserName, Role = rolename });
            }
            return View(viewModels);
        }

        public ActionResult CreateUser()
        {
            ndb = new AppContext();
            db = new ApplicationDbContext();
            var emp = ndb.Employees
                        .Where(x => x.IsActive == true)
                        .ToList()
                        .Select(x => new { EmployeeId = x.EmployeeId, EmployeeName = string.Format("{0} {1}", x.EmployeeCode, x.FirstName) });
            ViewBag.Employees = new SelectList(emp, "EmployeeId", "EmployeeName");
            //ViewBag.Roles = new SelectList(db.Roles, "RoleId", "Role");
            ViewBag.Roles = new SelectList(db.Roles.ToList(), "Name", "Name");

            return PartialView("_CreateUser");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(CreateUserVM model)
        {
            ndb = new AppContext();
            db = new ApplicationDbContext();
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, EmployeeId = model.EmployeeId, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    await this.UserManager.AddToRoleAsync(user.Id, model.RoleId);
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return Json(new { success = true });
                }
                AddErrors(result);
            }

            var emp = ndb.Employees
                        .Where(x => x.IsActive == true)
                        .ToList()
                        .Select(x => new { EmployeeId = x.EmployeeId, EmployeeName = string.Format("{0} {1}", x.EmployeeCode, x.FirstName) });
            ViewBag.Employees = new SelectList(emp, "EmployeeId", "EmployeeName");
            ViewBag.Roles = new SelectList(db.Roles.ToList(), "Name", "Name");
            return PartialView("_CreateUser", model);
        }

        #endregion

        #region Roles
        public ActionResult RolesList()
        {
            db = new ApplicationDbContext();
            var roles = db.Roles.ToList();
            return View(roles);
        }

        public ActionResult CreateRole()
        {
            return PartialView("_RoleCreate");
        }
        [HttpPost]
        public ActionResult CreateRole(CreateRoleVM vm)
        {
            if (ModelState.IsValid)
            {
                db = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                if (!roleManager.RoleExists(vm.Roles))
                {
                    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                    role.Name = vm.Roles;
                    roleManager.Create(role);
                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError("Role Already exist", "Role Already exist");
                    return PartialView("_RoleCreate", vm);
                }
            }
            
            return PartialView("_RoleCreate",vm);
        }

        public ActionResult RoleMenu(string Id)
        {
            ndb = new AppContext(); 
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var obj = (from um in ndb.UserMenus
                       join sm in ndb.SubMenus on um.SubMenuId equals sm.Id
                       //join mm in ndb.MainMenus on um.MainMenuId equals mm.Id
                       where (um.RoleId == Id)
                       select sm.Id).ToArray();

            //var docs = ndb.SubMenus.Where(x => x.IsActive == true).ToList();
            var docs = (from sm in ndb.SubMenus
                        join mm in ndb.MainMenus on sm.MainMenuId equals mm.Id
                        where (sm.IsActive == true)
                        select new { Id = sm.Id, Name = mm.Name + " -> " + sm.Name }).ToList();
            ViewBag.SubMenu = new MultiSelectList(docs, "Id", "Name", obj);

            ViewBag.RoleId = Id;
            return PartialView("RoleMenu");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleMenu(IEnumerable<int> SubMenuListId, string id)
        {
            ndb = new AppContext();
            //int CourseId = Convert.ToInt32(id);

            ndb.UserMenus.RemoveRange(ndb.UserMenus.Where(x => x.RoleId == id));
            await ndb.SaveChangesAsync();

            foreach (var item in SubMenuListId)
            {
                var obj = new UserMenu { SubMenuId = item, RoleId = id, MainMenuId = ndb.SubMenus.Find(item).MainMenuId };
                ndb.UserMenus.Add(obj);
                await ndb.SaveChangesAsync();
            }
            //return PartialView("CourseDocuments");
            return Json(new { success = true });
        }

        #endregion

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}