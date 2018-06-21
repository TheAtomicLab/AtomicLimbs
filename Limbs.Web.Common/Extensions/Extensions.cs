using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Limbs.Web.Common.Extensions
{
    public static class Extensions
    {
        public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var description = metadata.Description;

            return MvcHtmlString.Create(description);
        }

        public static string ToDescription(this Enum value)
        {
            var da = (DescriptionAttribute[])value.GetType().GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }

        public static string EmailMask(this String email)
        {
            string pattern = @"(.{1}).+@.+(.{2}(?:\..{2,3}){1,1})";
            String replace = "$1****@****$2";
            string result = Regex.Replace(email, pattern, replace);
            
            return result;

        }

        public static string ToFriendlyDateString(this DateTime date)
        {
            // 1.
            // Get time span elapsed since the date.
            var s = DateTime.UtcNow.Subtract(date);

            // 2.
            // Get total number of days elapsed.
            var dayDiff = (int)s.TotalDays;

            // 3.
            // Get total number of seconds elapsed.
            var secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0)
            {
                return null;
            }

            // 5.
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "recién";
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "hace 1 minuto";
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return $"hace {Math.Floor((double)secDiff / 60)} minutos";
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "hace 1 hora";
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return $"hace {Math.Floor((double)secDiff / 3600)} horas";
                }
            }
            else if (dayDiff == 1)
            {
                return "ayer";
            }
            // 6.
            // Handle previous days.
            if (dayDiff < 7)
            {
                return $"hace {dayDiff} dias";
            }

            //7
            // Get total number of month elapsed.
            var monthDiff = Math.Abs((DateTime.UtcNow.Month - date.Month) + 12 * (DateTime.UtcNow.Year - date.Year));

            if (dayDiff >= 31)
            {
                if (monthDiff < 2)
                {
                    return "hace 1 mes";
                }
                else
                {
                    return String.Format("hace {0} meses", monthDiff);
                }
            }

            //
            return dayDiff < 31 ? $"hace {Math.Ceiling((double)dayDiff / 7)} semanas" : null;
        }

        public static string ToFriendlyDistanceString(this double distance)
        {
            //distance in meters
            return distance > 1000 ? $"{(distance / 1000):F}km" : $"{distance:F}m";
        }


    }
}