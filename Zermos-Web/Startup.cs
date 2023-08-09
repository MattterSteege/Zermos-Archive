using System;
using Infrastructure;
using Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Zermos_Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<ZermosContext>();
            services.AddScoped<Users>();
            
            services.AddProgressiveWebApp();

            var cookieExpiration = TimeSpan.FromDays(60);

            services.AddAuthentication("EmailScheme") // Sets the default scheme to cookies
                .AddCookie("EmailScheme", options =>
                {
                    // options.AccessDeniedPath = "/Account/Denied";
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Logout";
                    options.ExpireTimeSpan = cookieExpiration;
                    options.Cookie.MaxAge = cookieExpiration;
                    options.Cookie.IsEssential = true;
                });
            
            //sets the data protection keys to be stored in the /dataprotection folder (in the docker volume dataprotection)
            services.AddDataProtection().PersistKeysToFileSystem(new System.IO.DirectoryInfo("/dataprotection"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error/Error");

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    context.Response.Redirect("/Error/404");
                }
            });

            app.UseForwardedHeaders();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Main}/{action=Index}/{id?}");
            });
        }
    }
}