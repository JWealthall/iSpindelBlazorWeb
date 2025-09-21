using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using iSpindelBlazorWeb.Server.Data;
using iSpindelBlazorWeb.Shared;
using MessagePack;
using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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

            services.AddControllersWithViews().AddMvcOptions(option =>
                {
                    // Run with the default recommended resolver
                    //option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
                    //option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Options));

                    // Can be run with compression - will make a smaller sizer, but is slower to run
                    //option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options.WithCompression(MessagePackCompression.Lz4BlockArray)));
                    //option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Options.WithCompression(MessagePackCompression.Lz4BlockArray)));

                    // Use a custom resolver
                    option.OutputFormatters.Add(new MessagePackOutputFormatter(MsgPack.CustomFormatter));
                    option.InputFormatters.Add(new MessagePackInputFormatter(MsgPack.CustomFormatter));
                });
            services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();
            services.AddRazorPages();
            services.AddTransient<LogDbService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(WebApplication app, IWebHostEnvironment env)
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


            app.UseStaticFiles();

            app.UseRouting();

            app.UseAntiforgery();

            app.MapRazorPages();
            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);
        }
    }
}
