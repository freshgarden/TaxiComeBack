using System.Web.Mvc;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public PartialViewResult MainAdminNav()
        {
            return PartialView();
        }
    }
}