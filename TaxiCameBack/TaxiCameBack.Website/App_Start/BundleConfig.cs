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
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                                    "~/Scripts/knockout-{version}.js",
                        "~/Scripts/knockout.validation.js"));

            //Register Css
            bundles.Add(new StyleBundle("~/custom/css").Include("~/Content/custom/custom-style.css"));
            bundles.Add(new StyleBundle("~/custom/map").Include("~/Content/custom/maps.css"));
            bundles.Add(new StyleBundle("~/jquery-ui/css").Include(
                "~/Scripts/jquery-ui/jquery-ui.min.css",
                "~/Scripts/jquery-ui/jquery-ui.theme.min.css"
                ));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css", "~/Content/Site.css"));
        }
    }
}
