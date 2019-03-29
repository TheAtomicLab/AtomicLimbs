using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Limbs.Web.Common.Extensions;

namespace Limbs.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var configuration = new Migrations.Configuration();
            var migrator = new System.Data.Entity.Migrations.DbMigrator(configuration);
            migrator.Update();
            
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

                var action = ExceptionAction.SendMailAndEnqueue;

                if (typeof(HttpException) == exception.GetType())
                {
                    var url = Context.Request.Url.ToString();
                    if (url.Contains("activity") || url.Contains("show") || url.Contains("members") || url.Contains("ogShow") || url.Contains("wp-admin"))
                    {
                        action = ExceptionAction.Enqueue;
                    }
                }

                exception.Log(Context, action);
            }
            catch (Exception ex)
            {
                (new SystemException("FATAL", ex)).Log(Context, ExceptionAction.SendMailAndEnqueue);
                throw;
            }
        }
    }
}
