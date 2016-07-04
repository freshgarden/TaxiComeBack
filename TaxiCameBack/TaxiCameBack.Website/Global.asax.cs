using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
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

        public override void Dispose()
        {
            _container.Dispose();
            base.Dispose();
        }
    }
}
