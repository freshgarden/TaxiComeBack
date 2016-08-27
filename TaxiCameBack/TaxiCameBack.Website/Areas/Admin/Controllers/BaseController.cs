using System;
using System.Web.Mvc;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            //If the request is AJAX return JSON else redirect user to Error view.
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                //Return JSON
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { error = true, message = "Sorry, an error occurred while processing your request." }
                };
            }
            else
            {
                //Redirect user to error page
                filterContext.ExceptionHandled = true;
                filterContext.Result = this.RedirectToAction("Error500", "Errors", new {area = "Admin"});
            }

            base.OnException(filterContext);
        }

        // GET: Base
        public BaseController() { }
    }
}