using System.Web.Mvc;

namespace Limbs.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Admin";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Panel", action = "Index", id = UrlParameter.Optional },
                new[] { "Limbs.Web.Areas.Admin.Controllers" }
            );
        }
    }
}