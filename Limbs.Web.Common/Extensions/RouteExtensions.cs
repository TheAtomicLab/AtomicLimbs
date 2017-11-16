using System.Web;
using System.Web.Routing;

namespace Limbs.Web.Common.Extensions
{
    public static class RouteExtensions
    {
        public static void Redirect(this RouteCollection routes, string url, string redirectUrl)
        {
            routes.Add(new Route(url, new RedirectRouteHandler(redirectUrl)));
        }
    }

    public class RedirectRouteHandler : IRouteHandler
    {
        private string _redirectUrl;

        public RedirectRouteHandler(string redirectUrl)
        {
            _redirectUrl = redirectUrl;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (_redirectUrl.StartsWith("~/"))
            {
                string virtualPath = _redirectUrl.Substring(2);
                var route = new Route(virtualPath, null);
                var vpd = route.GetVirtualPath(requestContext, requestContext.RouteData.Values);
                if (vpd != null)
                {
                    _redirectUrl = "~/" + vpd.VirtualPath;
                }
            }

            if (requestContext.HttpContext.Request.Url != null)
            {
                if (_redirectUrl.Split('?').Length == 2)
                {
                    _redirectUrl = _redirectUrl.Split('?')[0];
                }

                if (requestContext.HttpContext.Request.Url.PathAndQuery.Split('?').Length > 1)
                    _redirectUrl += "?" + requestContext.HttpContext.Request.Url.PathAndQuery.Split('?')[1];
            }

            return new RedirectHandler(_redirectUrl, false);
        }
    }

    public class RedirectHandler : IHttpHandler
    {
        private readonly string _redirectUrl;

        public RedirectHandler(string redirectUrl, bool isReusable)
        {
            _redirectUrl = redirectUrl;
            IsReusable = isReusable;
        }

        public bool IsReusable { get; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Status = "301 Moved Permanently";
            context.Response.StatusCode = 301;
            context.Response.AddHeader("Location", _redirectUrl);
        }
    }
}
