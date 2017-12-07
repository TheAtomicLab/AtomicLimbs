using System.Web.Mvc;
using Limbs.Web.Common.Extensions;

namespace Limbs.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
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
}
