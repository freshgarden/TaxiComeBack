﻿using System.Web.Mvc;
using System.Web.Routing;

namespace TaxiCameBack.Website.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Admin";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            RouteTable.Routes.LowercaseUrls = true;
            RouteTable.Routes.AppendTrailingSlash = true;
            
//            context.MapRoute(
//                "Schedule",
//                "Admin/Dang-Ky-Lich-Trinh/{action}/{id}",
//                new { controller = "Schedule", action = "Index", id = UrlParameter.Optional }
//                );
//
//            context.MapRoute(
//                "Account",
//                "Admin/Nguoi-Dung/{action}/{id}",
//                new { controller="Account", action = "Index", id = UrlParameter.Optional }
//            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}