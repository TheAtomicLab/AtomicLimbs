using System.Web.Optimization;

namespace Limbs.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/estilo01.css",
                "~/Content/estilo02.css",
                "~/Content/general_style.css",
                "~/Content/estiloatomchoose.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                "~/Content/jquery-ui.min.css"));
        }
    }
}