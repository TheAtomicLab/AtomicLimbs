using System.Web.Mvc;
using System.Web.Routing;
using Limbs.Web.Common.Extensions;

namespace Limbs.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Redirect("libre", "/");
            routes.Redirect("donations/limbs", "/");
            routes.Redirect("faq", "/");
            routes.Redirect("manoton", "/");
            routes.Redirect("dar", "/");
            routes.Redirect("embajador-atomico", "/");
            routes.Redirect("pedir-una-mano", "/");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] { "Limbs.Web.Controllers" }
            );

            routes.MapRoute(
                "404-PageNotFound",
                "{*url}",
                new { controller = "StaticContent", action = "PageNotFound" }
            );

        }
    }
}
