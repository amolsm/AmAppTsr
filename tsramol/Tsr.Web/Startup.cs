using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tsr.Web.Startup))]
namespace Tsr.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
