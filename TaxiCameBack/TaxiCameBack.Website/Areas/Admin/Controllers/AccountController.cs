using System.Web.Mvc;
using TaxiCameBack.Website.Areas.Admin.Models;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {

            }
            else
            {
                ModelState.AddModelError("", "There was an error login");
            }

            return RedirectToAction("Index", "Schedule", new { area = "Admin" });
        }

        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Register(RegisterViewModel registerViewModel)
        {
            return View();
        }
    }
}