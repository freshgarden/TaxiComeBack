using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TaxiCameBack.Website.Startup))]
namespace TaxiCameBack.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
