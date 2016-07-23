using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Services.Logging;
using TaxiCameBack.Services.Settings;
using TaxiCameBack.Website.Application;
using TaxiCameBack.Website.Dependency;

namespace TaxiCameBack.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly IWindsorContainer _container;

        public ISettingsService SettingsService => ServiceFactory.Get<ISettingsService>();
        public ILoggingService LoggingService => ServiceFactory.Get<ILoggingService>();

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

            // If the same carry on as normal
            var logging = _container.Resolve<ILoggingService>();
            logging.Initialise(ConfigUtils.GetAppSettingInt32("LogFileMaxSizeBytes", 10000));
            logging.Error("START APP");
        }

        //        protected void Application_Error(object sender, EventArgs e)
        //        {
        //            var lastError = Server.GetLastError();
        //            // Don't flag missing pages or changed urls, as just clogs up the log
        //            if (!lastError.Message.Contains("was not found or does not implement IController"))
        //            {
        //                Response.RedirectToRoute(new RouteValueDictionary(
        //                    new RouteValueDictionary(
        //                                new
        //                                {
        //                                    controller = "Errors",
        //                                    action = "Error404",
        //                                    area = "Admin"
        //                                })
        //                                ));
        //            }
        //        }

        public override void Dispose()
        {
            _container.Dispose();
            base.Dispose();
        }
    }
}
