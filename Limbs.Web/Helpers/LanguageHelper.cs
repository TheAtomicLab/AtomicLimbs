using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Limbs.Web.Helpers
{
    public static class LanguageHelper
    {
        public static void SetLanguage(string language)
        {
            //NumberFormatInfo numberInfo = CultureInfo.CreateSpecificCulture("nl-NL").NumberFormat;
            CultureInfo info = new CultureInfo(language);
            info.NumberFormat.NumberDecimalSeparator = ".";
            //info.NumberFormat = numberInfo;
            //info.DateTimeFormat.DateSeparator = "/";
            //info.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentUICulture = info;
            Thread.CurrentThread.CurrentCulture = info;
        }


    }
}