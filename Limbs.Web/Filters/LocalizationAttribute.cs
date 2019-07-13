using System.Web.Mvc;
using Limbs.Web.Helpers;

namespace Limbs.Web.Filters
{
    public class LocalizationAttribute : ActionFilterAttribute
    {
        private string _DefaultLanguage = Languages.es.ToString();

        public LocalizationAttribute(string defaultLanguage)
        {
            _DefaultLanguage = defaultLanguage;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string lang = (string)filterContext.RouteData.Values["lang"] ?? _DefaultLanguage;
            LanguageHelper.SetLanguage(lang);
        }
    }
}