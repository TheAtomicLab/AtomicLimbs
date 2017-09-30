using System;

namespace Limbs.Web.Extensions
{
    public static class DateExtensions
    {
        public static string ToFriendlyDateString(this DateTime date)
        {
            // 1.
            // Get time span elapsed since the date.
            var s = DateTime.Now.Subtract(date);

            // 2.
            // Get total number of days elapsed.
            var dayDiff = (int)s.TotalDays;

            // 3.
            // Get total number of seconds elapsed.
            var secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0 || dayDiff >= 31)
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
                    return $"hace {Math.Floor((double) secDiff / 60)} minutos";
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
                    return $"hace {Math.Floor((double) secDiff / 3600)} horas";
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
            return dayDiff < 31 ? $"hace {Math.Ceiling((double) dayDiff / 7)} semanas" : null;
        }
    }
}