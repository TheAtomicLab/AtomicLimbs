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

            var targetRoute = routes.MapRoute("", "Home/Index", new { controller = "Home", action = "Index" });
            var donar = routes.MapRoute("", "StaticContent/Donar", new { controller = "StaticContent", action = "Donar" });
            var faq = routes.MapRoute("", "StaticContent/Faq", new { controller = "StaticContent", action = "Faq" });
            var libre = routes.MapRoute("", "StaticContent/Libre", new { controller = "StaticContent", action = "Libre" });
            var manoton = routes.MapRoute("", "StaticContent/Manoton", new { controller = "StaticContent", action = "Manoton" });
            var embajadores = routes.MapRoute("", "StaticContent/Embajadores", new { controller = "StaticContent", action = "Embajadores" });
            var animales = routes.MapRoute("", "StaticContent/Animales", new { controller = "StaticContent", action = "Animales" });
            var lista = routes.MapRoute("", "Orders/PublicOrders", new { controller = "Orders", action = "PublicOrders", area = "" }, new[] { "Limbs.Web.Controllers" });

            routes.Redirect(r => r.MapRoute("", "faq")).To(faq);
            routes.Redirect(r => r.MapRoute("", "dar")).To(donar);
            routes.Redirect(r => r.MapRoute("", "donar")).To(donar);
            routes.Redirect(r => r.MapRoute("", "libre")).To(libre);
            routes.Redirect(r => r.MapRoute("", "manoton")).To(manoton);
            routes.Redirect(r => r.MapRoute("", "pedir")).To(targetRoute);
            routes.Redirect(r => r.MapRoute("", "pedir-una-mano")).To(targetRoute);
            routes.Redirect(r => r.MapRoute("", "embajador-atomico")).To(embajadores);
            routes.Redirect(r => r.MapRoute("", "SerEmbajador")).To(embajadores);
            routes.Redirect(r => r.MapRoute("", "SerVoluntario")).To(embajadores);
            routes.Redirect(r => r.MapRoute("", "donations/limbs")).To(donar);
            routes.Redirect(r => r.MapRoute("", "animales")).To(animales);
            routes.Redirect(r => r.MapRoute("", "ListaDeEspera")).To(lista);

            routes.MapRoute(
             name: "Localized",
             url: "{lang}/{controller}/{action}/{id}",
             constraints: new { lang = @"(\w{2})|(\w{2}-\w{2})" },
             defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

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
