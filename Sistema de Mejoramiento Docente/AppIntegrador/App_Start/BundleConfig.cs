using System.Web;
using System.Web.Optimization;

namespace AppIntegrador
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymin").Include(
                        "~/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css",
                      "~/Content/Navbar/navbar.css",
                      "~/Content/PlanesDeMejora/mainPlanesDeMejora.css",
                      "~/Content/PlanesDeMejora/planesDeMejoraStructureStyles.css"));

            bundles.Add(new StyleBundle("~/Content/bs3").Include(
          "~/Content/bootstrap.css"));
            bundles.Add(new StyleBundle("~/Content/bs4.3.1").Include(
                "~/Content/Visualizacion/bootstrap.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/bs4.3.1").Include(
          "~/Scripts/bootstrap4.3.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery3.3.1").Include(
            "~/Scripts/jquery-3.3.1.slim.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/popper1.14.7").Include(
            "~/Scripts/popper.min.js"));

            bundles.Add(new StyleBundle("~/Content/users&profiles").Include(
            "~/Content/UsuariosYPerfiles/login.css",
            "~/Content/UsuariosYPerfiles/Perfiles.css",
            "~/Content/Plugins/animate.css"));

            // Custom bundles
            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                      "~/Scripts/Plugins/jquery.dirrty.js",
                      "~/Scripts/Plugins/bootstrap-notify.js",
                      "~/Scripts/Plugins/sweetalert2.all.js",
                      "~/Scripts/UsuariosYPerfiles/login.js",
                      "~/Scripts/UsuariosYPerfiles/dirrtyUsers.js",
                      "~/Scripts/UsuariosYPerfiles/alertsHandler.js",
                      "~/Scripts/UsuariosYPerfiles/bootstrap-select.js",
                      "~/Scripts/UsuariosYPerfiles/bootstrap-select-min.js"
                      ));

        }
    }
}
