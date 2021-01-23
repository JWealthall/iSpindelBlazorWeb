using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using iSpindelBlazorWeb.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace iSpindelBlazorWeb.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var serverType = 0;
            var readOnly = false;
            if (Configuration != null)
            {
                if (!int.TryParse(Configuration["ServerType"], out serverType)) serverType = 0;
                if (!bool.TryParse(Configuration["ReadOnly"], out readOnly)) readOnly = false;
            }
            LogDbService.ReadOnly = readOnly;
            if (serverType == 1)
                services.AddDbContext<LogDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("iSpindelSqlite")));
            else
                services.AddDbContext<LogDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("iSpindelSqlServer")));

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddTransient<LogDbService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
