using System;
using System.Collections.Generic;

namespace unicron.Extensions
{
    public static class DateExtensions {
        public static IEnumerable<DateTime> Range(this DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }

}