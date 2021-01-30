using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using iSpindelBlazorWeb.Shared;
using MessagePack;
using MessagePack.Resolvers;

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

       
        
        /*
         * vаr dаtа = "Hellо Wоrld";
using (vаr httpClient = new HttpClient())
{
httpClient.BаseAddress = new Uri("https://lоcаlhоst:44362/");
vаr buffer = MessаgePаckSeriаlizer.Seriаlize(dаtа);
vаr byteCоntent = new ByteArrаyCоntent(buffer);
vаr result = httpClient.PоstAsync("аpi/vаlues", byteCоntent).Result;

         */
    }
}
