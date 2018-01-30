using System;
using System.Web.Mvc;
using System.Web.Routing;
using Limbs.Web.Common.Extensions;

namespace Limbs.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
            filters.Add(new HandleAntiforgeryTokenErrorAttribute
            {
                ExceptionType = typeof(HttpAntiForgeryException)
            });
        }
    }

    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.Exception.Log(filterContext.HttpContext.ApplicationInstance.Context, ExceptionAction.SendMailAndEnqueue);

            base.OnException(filterContext);
        }
    }

    public class HandleAntiforgeryTokenErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Account", _t = DateTime.UtcNow.Millisecond }));
        }
    }
}
