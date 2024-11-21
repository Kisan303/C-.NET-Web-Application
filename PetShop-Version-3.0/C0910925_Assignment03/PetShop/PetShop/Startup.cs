using Microsoft.Owin;
using Owin;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


[assembly: OwinStartupAttribute(typeof(PetShop.Startup))]
namespace PetShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);



        }
    }
}
