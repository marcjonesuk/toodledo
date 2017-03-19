using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class DateTimeFormatter
    {
        public static string Format(DateTime dt)
        {
            var now = DateTime.UtcNow;
            if (now - dt < TimeSpan.FromHours(1))
            {
                var minutes = (int)Math.Round((now - dt).TotalMinutes,0);
                if (minutes < 1)
                    return "less than a minute ago";
                if (minutes == 1)
                    return "about one minute ago";
                return $"{minutes:N0} minutes ago";
            }
            if (now - dt < TimeSpan.FromHours(12))
            {
                var hours = (now - dt).TotalHours;
                return $"{hours:N0} hours ago";
            }
            return dt.ToString("dd MMM yyyy") + " at " + dt.ToString("HH:mm");
        }
    }
}
