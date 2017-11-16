using System.Web.Mvc;
using System.Web.Routing;
using RouteMagic;

namespace Limbs.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            var targetRoute = routes.MapRoute("new", "Home/{action}", new { controller = "Home", action = "Index" });
            routes.Redirect(r => r.MapRoute("faq", "faq")).To(targetRoute);
            routes.Redirect(r => r.MapRoute("dar", "dar")).To(targetRoute);
            routes.Redirect(r => r.MapRoute("libre", "libre")).To(targetRoute);
            routes.Redirect(r => r.MapRoute("manoton", "manoton")).To(targetRoute);
            routes.Redirect(r => r.MapRoute("pedir-una-mano", "pedir-una-mano")).To(targetRoute);
            routes.Redirect(r => r.MapRoute("embajador-atomico", "embajador-atomico")).To(targetRoute);
            routes.Redirect(r => r.MapRoute("donations/limbs", "donations/limbs")).To(targetRoute);
            
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional},
                new[] { "Limbs.Web.Controllers" }
            ).SetRouteName("Default");
            
            routes.MapRoute(
                "404-PageNotFound",
                "{*url}",
                new { controller = "StaticContent", action = "PageNotFound" }
            );

        }
    }
}
