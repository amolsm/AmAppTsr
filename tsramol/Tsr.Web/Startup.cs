using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Tsr.Web.Models;

[assembly: OwinStartupAttribute(typeof(Tsr.Web.Startup))]
namespace Tsr.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User     
            //if (!roleManager.RoleExists("Admin"))
            //{

            //    // first we create Admin rool    
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Admin";
            //    roleManager.Create(role);

            //Here we create a Admin super user who will maintain the website                   

            //var user = new ApplicationUser();
            //user.UserName = "amol";
            //user.Email = "amol@gmail.com";

            //string userPWD = "Amol@12345";

            //var chkUser = UserManager.Create(user, userPWD);

            ////Add default User to Role Admin    
            //if (chkUser.Succeeded)
            //{
            //    var result1 = UserManager.AddToRole(user.Id, "Admin");

            //}
            //}

            // creating Creating Manager role     
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

            }
            if (!roleManager.RoleExists("Admission"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admission";
                roleManager.Create(role);

            }

            // creating Creating Tester role     
            if (!roleManager.RoleExists("Fees"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Fees";
                roleManager.Create(role);

            }
            if (!roleManager.RoleExists("Fees Worli"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Fees";
                roleManager.Create(role);

            }
            // creating Creating Tester role     
            if (!roleManager.RoleExists("Printing"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Printing";
                roleManager.Create(role);

            }
        }
    }
}
