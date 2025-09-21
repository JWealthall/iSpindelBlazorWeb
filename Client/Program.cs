using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using iSpindelBlazorWeb.Client.Components;
using iSpindelBlazorWeb.Client.HttpRepository;
using Microsoft.AspNetCore.Components.Routing;

namespace iSpindelBlazorWeb.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IDataHttpRepository, DataHttpRepository>();
            builder.Services.AddSingleton<PageHistoryState>();
            
            await builder.Build().RunAsync();
        }
        
        //public static Type FindRoute(Router router, string path) {
        //    var assm = typeof(Router).Assembly;
        //    var routes = typeof(Router).GetProperty("Routes", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router);
        //    var type = assm.GetTypes().FirstOrDefault(t => t.Name == "RouteContext");
        //    var context = Activator.CreateInstance(type, new[] { path });
        //    routes.GetType().GetMethod("Route", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(routes, new[] { context });
        //    return type.GetProperty("Handler").GetValue(context) as Type;
        //}
    }
}
