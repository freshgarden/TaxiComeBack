using System.Web;
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

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui/jquery-ui.min.js"));
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

            //Register Css
            bundles.Add(new StyleBundle("~/custom/css").Include("~/Content/custom/custom-style.css"));
            bundles.Add(new StyleBundle("~/custom/map").Include("~/Content/custom/maps.css"));
            bundles.Add(new StyleBundle("~/jquery-ui/css").Include(
                "~/Scripts/jquery-ui/jquery-ui.min.css",
                "~/Scripts/jquery-ui/jquery-ui.theme.min.css"
                ));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css", "~/Content/Site.css"));

            //backend css
            bundles.Add(new StyleBundle("~/admin/css").Include(
                "~/Content/bootstrap/css/bootstrap.min.css",
                "~/Content/font-awesome-4.6.3/css/font-awesome.min.css",                
                "~/Content/ionicons-2.0.1/css/ionicons.min.css",
                "~/Content/dist/css/AdminLTE.min.css"                
                ));
            bundles.Add(new StyleBundle("~/css/skins").Include(
                "~/Content/skins/_all-skins.min.css"
                ));
            bundles.Add(new StyleBundle("~/css/icheck").Include(
                "~/Scripts/plugins/iCheck/square/blue.css"
                ));
        }
    }
}
