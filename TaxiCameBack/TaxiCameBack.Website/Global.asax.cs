using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Castle.Windsor;
using TaxiCameBack.Website.Dependency;

namespace TaxiCameBack.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly IWindsorContainer _container;

        public MvcApplication()
        {
            _container = new WindsorContainer().Install(new DependencyConventions());
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var controllerFactory = new WindsorControllerFactory(_container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastError = Server.GetLastError();
            // Don't flag missing pages or changed urls, as just clogs up the log
            if (!lastError.Message.Contains("was not found or does not implement IController"))
            {
                Response.Redirect("Errors/Error404");
            }
        }

        public override void Dispose()
        {
            _container.Dispose();
            base.Dispose();
        }
    }
}
