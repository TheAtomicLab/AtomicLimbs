using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Limbs.Web.Common.Extensions;

namespace Limbs.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var exception  = Server.GetLastError();

                exception.Log(Context, ExceptionAction.SendMailAndEnqueue);
            }
            catch (Exception ex)
            {
                (new SystemException("FATAL", ex)).Log(Context, ExceptionAction.SendMailAndEnqueue);
                throw;
            }
        }
    }
}
