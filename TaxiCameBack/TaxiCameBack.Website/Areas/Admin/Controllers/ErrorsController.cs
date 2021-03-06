﻿using System.Web;
using System.Web.Mvc;
using TaxiCameBack.Website.Areas.Admin.Models;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public class ErrorsController : BaseController
    {
        public ActionResult NotFound(string url)
        {
            var originalUri = url ?? Request.QueryString["aspxerrorpath"] ?? Request.Url.OriginalString;
            var controllerName = (string)RouteData.Values["controller"];
            var actionName = (string)RouteData.Values["action"];
            var model = new NotFoundModel(new HttpException(404, "Failed to find page"), controllerName, actionName)
            {
                RequestedUrl = originalUri,
                ReferrerUrl = Request.UrlReferrer == null ? "" : Request.UrlReferrer.OriginalString
            };
            Response.StatusCode = 404;
            return View("NotFound", model);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            var name = GetViewName(ControllerContext, $"~/Views/Errors/{actionName}.cshtml",
                                  "~/Views/Error/Error.cshtml",
                                  "~/Views/Error/General.cshtml",
                                  "~/Views/Shared/Error.cshtml");
            var controllerName = (string)RouteData.Values["controller"];
            var model = new HandleErrorInfo(Server.GetLastError(), controllerName, actionName);
            var result = new ViewResult
            {
                ViewName = name,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
            };
            Response.StatusCode = 501;
            result.ExecuteResult(ControllerContext);
        }
        protected string GetViewName(ControllerContext context, params string[] names)
        {
            foreach (var name in names)
            {
                var result = ViewEngines.Engines.FindView(ControllerContext, name, null);
                if (result.View != null)
                    return name;
            }
            return null;
        }


        // GET: Admin/Errors
        public ActionResult Error500()
        {
            return View();
        }        

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult AcessDenied()
        {
            return View();
        }
    }
}