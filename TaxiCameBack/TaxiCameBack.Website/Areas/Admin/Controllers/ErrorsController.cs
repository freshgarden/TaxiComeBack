using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public class ErrorsController : BaseController
    {
        // GET: Admin/Errors
        public ActionResult Error500()
        {
            return View();
        }        

        public ActionResult Error404()
        {
            return View();
        }
    }
}