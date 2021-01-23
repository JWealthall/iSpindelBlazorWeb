using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iSpindelBlazorWeb.Client
{
    public static class Extensions
    {
        public static string ToJava(this bool b)
        {
            return b ? "true" : "false";
        }

        public static DateTime Trim(this DateTime date, long ticks) { return new DateTime(date.Ticks - (date.Ticks % ticks), date.Kind); }
        public static DateTime TrimToSecond(this DateTime date) { return date.Trim(TimeSpan.TicksPerSecond); }
        public static DateTime TrimToSecond(this DateTime date, int seconds) { return date.Trim(TimeSpan.TicksPerSecond * seconds); }
        public static DateTime TrimToMinute(this DateTime date) { return date.Trim(TimeSpan.TicksPerMinute); }
        public static DateTime TrimToMinute(this DateTime date, int minutes) { return date.Trim(TimeSpan.TicksPerMinute * minutes); }
        public static DateTime TrimToHour(this DateTime date) { return date.Trim(TimeSpan.TicksPerHour); }
        public static DateTime TrimToHour(this DateTime date, int minutes) { return date.Trim(TimeSpan.TicksPerHour * minutes); }
    }
}
