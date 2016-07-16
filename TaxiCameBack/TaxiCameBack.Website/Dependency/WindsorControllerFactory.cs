using System;
using System.Web;
using Castle.MicroKernel;
using System.Web.Mvc;
using System.Web.Routing;

namespace TaxiCameBack.Website.Dependency
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            _kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                var context = new RequestContext(
                        new HttpContextWrapper(System.Web.HttpContext.Current),
                        new RouteData());
                var urlHelper = new UrlHelper(context);
                var url = urlHelper.Action("NotFound", "Errors", new {area = "Admin", url = requestContext.HttpContext.Request.Path });
                if (url != null) HttpContext.Current.Response.Redirect(url);
                return null;
            }
            return (IController) _kernel.Resolve(controllerType);
        }
    }
}