using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using TaxiCameBack.Website.Application.Security;

namespace TaxiCameBack.Website.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (string.IsNullOrEmpty(SessionPersister.Username))
            {
                if (filterContext.HttpContext.Request.Url != null)
                    filterContext.Result =
                        new RedirectToRouteResult(
                            new RouteValueDictionary(
                                new
                                {
                                    controller = "Account",
                                    action = "Login",
                                    area = "Admin",
                                    returnUrl =
                                        filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery,
                                            UriFormat.SafeUnescaped)
                                }));
                else
                    filterContext.Result = new RedirectResult("~/Admin/Account/Login");
            }
            else
            {
                if (SessionPersister.Roles.Length == 0)
                {
                    if (filterContext.HttpContext.Request.Url != null)
                        filterContext.Result =
                            new RedirectToRouteResult(
                                new RouteValueDictionary(
                                    new
                                    {
                                        controller = "Account",
                                        action = "Login",
                                        area = "Admin",
                                        returnUrl =
                                            filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery,
                                                UriFormat.SafeUnescaped)
                                    }));
                    else
                        filterContext.Result = new RedirectResult("~/Admin/Account/Login");
                }

                CustomPricipal cp = new CustomPricipal(SessionPersister.Roles.ToList());
                if (!cp.IsInRole(Roles))
                {
                    filterContext.Result = new RedirectResult("~/Admin/Errors/AcessDenied");
                }
            }
        }
    }
}