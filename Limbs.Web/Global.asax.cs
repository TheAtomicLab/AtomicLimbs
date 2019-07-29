using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Limbs.Web.Common.Extensions;
using Limbs.Web.Helpers;

namespace Limbs.Web
{
    public class MvcApplication : HttpApplication
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

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string lang = Languages.en.ToString();
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["lang"]))
            {
                lang = HttpContext.Current.Request["lang"] as String;
                HttpCookie langCookie = new HttpCookie("Language", lang);
                HttpContext.Current.Response.Cookies.Add(langCookie);
            }
            else if (HttpContext.Current.Request.Cookies["Language"] != null)
            {
                lang = HttpContext.Current.Request.Cookies["Language"].Value as String;
            }
            else if (Request.UserLanguages != null)
            {
                lang = Request.UserLanguages[0];
            }
            LanguageHelper.SetLanguage(lang);
        }

    }
}
