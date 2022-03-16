using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MissionAssignment7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissionAssignment7
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration temp)
        {
            Configuration = temp;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<BookstoreContext>(options =>
            {
                options.UseSqlite(Configuration["ConnectionStrings:BookDBConnection"]);
            });

            // Add Identity Connection
            services.AddDbContext<AppIdentityDBContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("IdentityConnection"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDBContext>();

            // Each Http request gets each repository objects
            services.AddScoped<IBookstoreRepository, EFBookstoreRepository>();
            services.AddScoped<IPurchaseRepository, EFPurchaseRepository>();

            services.AddRazorPages();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddScoped<Basket>(x => SessionBasket.GetBasket(x));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddServerSideBlazor(); // enables to use blazer
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            // Corresponds to the wwwroot
            app.UseStaticFiles();
            app.UseSession(); // session can store int, string, byte but we want to store basket so twe are going to create json file
            app.UseRouting();

            // Enable authentication and authorization  
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("typePage",
                    "{category}/{pageNum}",
                    new { Controller = "Home", action = "Index" }
                );

                endpoints.MapControllerRoute(
                    name: "Paging",
                    pattern: "{pageNum}",
                    defaults: new { Controller = "Home", action = "Index", pageNum = 1 }
                );

                endpoints.MapControllerRoute("type",
                    "{category}",
                    new { Controller = "Home", action = "Index", pageNum = 1 }
                );

                endpoints.MapDefaultControllerRoute();

                endpoints.MapRazorPages();

                endpoints.MapBlazorHub(); // refer to as a middleware
                endpoints.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index"); // If you don't get anything from 1st, go to 2nd
            });

            IdentitySeedData.EnsurePopulated(app);
        }
    }
}

