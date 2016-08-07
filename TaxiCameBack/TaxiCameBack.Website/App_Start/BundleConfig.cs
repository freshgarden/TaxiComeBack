using System.Web.Optimization;

namespace TaxiCameBack.Website
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Register Script

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.10.2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-2.2.3").Include(
                        "~/Scripts/plugins/jQuery/jquery-2.2.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/js/jquery-ui-1.9.2").Include(
                        "~/Scripts/jquery-ui-1.9.2.custom.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-migrate").Include("~/Scripts/jquery-migrate.min"));

            //Backend js
            bundles.Add(new ScriptBundle("~/admin/js").Include(
                "~/Scripts/plugins/jQuery/jquery-2.2.3.min.js",
                "~/Content/bootstrap/js/bootstrap.min.js"
                ));
            bundles.Add(new ScriptBundle("~/app/js").Include(
                "~/Content/dist/js/app.min.js"
                ));

            bundles.Add(new ScriptBundle("~/js/icheck").Include(
                "~/Scripts/plugins/iCheck/icheck.min.js"
                ));

            bundles.Add(new ScriptBundle("~/js/calendar").Include(
                "~/Scripts/moment.min.js",
                 "~/Scripts/plugins/fullcalendar/fullcalendar.min.js",
                 "~/Scripts/view_calendar.js",
                 "~/Scripts/plugins/fullcalendar/lang/vi.js"
                 ));

            //Register Css
            //bundles.Add(new StyleBundle("~/custom/css").Include("~/Content/custom/custom-style.css"));
            //bundles.Add(new StyleBundle("~/custom/map").Include("~/Content/custom/maps.css"));
            bundles.Add(new StyleBundle("~/jquery-ui/css").Include(
                "~/Scripts/jquery-ui/jquery-ui.min.css", new CssRewriteUrlTransform()
                ).Include("~/Scripts/jquery-ui/jquery-ui.theme.min.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css", "~/Content/Site.css"));

            //backend css
            bundles.Add(new StyleBundle("~/admin/css")
                .Include("~/Content/bootstrap/css/bootstrap.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/font-awesome-4.6.3/css/font-awesome.min.css", new CssRewriteUrlTransform())
                .Include(
                "~/Content/ionicons-2.0.1/css/ionicons.min.css",
                "~/Content/dist/css/AdminLTE.min.css",
                "~/Content/custom/admin.css"
                ));
            bundles.Add(new StyleBundle("~/css/skins").Include(
                "~/Content/skins/_all-skins.min.css"
                ));
            bundles.Add(new StyleBundle("~/css/icheck").Include(
                "~/Scripts/plugins/iCheck/square/blue.css", new CssRewriteUrlTransform()
                ));
            bundles.Add(new StyleBundle("~/css/calendar").Include(
                "~/Scripts/plugins/fullcalendar/fullcalendar.min.css",
                "~/Content/view_calendar.css"
                ));

            //Reg Front-End CSS
            bundles.Add(new StyleBundle("~/css/font-awesome").Include(
                "~/Content/frontend-css/font-awesome.css",
                 new CssRewriteUrlTransform()
                ));

            bundles.Add(new StyleBundle("~/css/icomoon").Include(
                "~/Content/frontend-css/icomoon.css",
                new CssRewriteUrlTransform()
                ));

            bundles.Add(new StyleBundle("~/css/fancybox").Include(
                "~/Content/frontend-css/jquery.fancybox-1.3.4.css"
                ));

            bundles.Add(new StyleBundle("~/css/revslider").Include(
                "~/Content/frontend-css/revslider.css"
                ));

            bundles.Add(new StyleBundle("~/css/style").Include(
                "~/Content/frontend-css/style.css"
                ));

            bundles.Add(new StyleBundle("~/css/responsive").Include(
               "~/Content/frontend-css/responsive.css"
               ));

            bundles.Add(new ScriptBundle("~/schedule/createedit").Include(
                "~/Scripts/ScheduleCreateEdit.js",
                "~/Scripts/aftp.js",
                "~/Scripts/Schedule.js"));

            bundles.Add(new ScriptBundle("~/js/signalr").Include(
                "~/Scripts/jquery.signalR-2.2.1.min.js"));

            //Reg Front-end JS
            bundles.Add(new ScriptBundle("~/front-end-js/jquery").Include(
                "~/Scripts/frontend-js/jquery-1.9.1.min.js",
                "~/Scripts/frontend-js/jquery.themepunch.plugins.min.js",
                "~/Scripts/frontend-js/jquery.themepunch.revolution.min.js"
                ));
            bundles.Add(new ScriptBundle("~/front-end-js/bootstrap").Include(
                "~/Scripts/frontend-js/bootstrap.min.js"
                ));
            bundles.Add(new ScriptBundle("~/front-end-js/form-style").Include(
                "~/Scripts/frontend-js/form_style.js"
                ));
            bundles.Add(new ScriptBundle("~/front-end-js/custom").Include(
                "~/Scripts/frontend-js/custom.js"
                ));
            bundles.Add(new ScriptBundle("~/front-end-js/placeholder").Include(
                "~/Scripts/frontend-js/jquery.placeholder.min.js"
                ));
            bundles.Add(new ScriptBundle("~/front-end-js/fancybox").Include(
                "~/Scripts/frontend-js/jquery.fancybox-1.3.4.js"
                ));
            bundles.Add(new ScriptBundle("~/front-end-js/gmap").Include(
                "~/Scripts/frontend-js/jquery.gmap.min.js"
                ));
            bundles.Add(new ScriptBundle("~/front-end-js/superfish").Include(
                "~/Scripts/frontend-js/superfish.js"
                ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
