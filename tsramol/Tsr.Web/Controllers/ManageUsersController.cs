using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}