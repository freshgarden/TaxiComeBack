using System.Web.Mvc;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        public PartialViewResult MainAdminNav()
        {
            return PartialView();
        }
    }
}