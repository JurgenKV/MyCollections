using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCollections.Models;

namespace MyCollections
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
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            

            services.AddIdentity<User, IdentityRole>(options =>
                    {
                        options.Password.RequiredLength = 1;   // ìèíèìàëüíàÿ äëèíà
                        options.Password.RequireNonAlphanumeric = false;   // òðåáóþòñÿ ëè íå àëôàâèòíî-öèôðîâûå ñèìâîëû
                        options.Password.RequireLowercase = false; // òðåáóþòñÿ ëè ñèìâîëû â íèæíåì ðåãèñòðå
                        options.Password.RequireUppercase = false; // òðåáóþòñÿ ëè ñèìâîëû â âåðõíåì ðåãèñòðå
                        options.Password.RequireDigit = false; // òðåáóþòñÿ ëè öèôðû
                        
                        options.User.AllowedUserNameCharacters =
                            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
                        options.User.RequireUniqueEmail = false;
                       
                    }

                )
                .AddEntityFrameworkStores<ApplicationContext>();
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/User/Login");
                options.Cookie.Name = "Cookie";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(720);
                options.LoginPath = new PathString("/User/Login");
                options.LogoutPath = new PathString("/User/Login");// !!!!!!!!!!

                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                options.ValidationInterval = TimeSpan.Zero;
                
            });
            services.AddControllersWithViews();

            //services.AddDbContext<ItemContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
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
                app.UseExceptionHandler("/User/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseRouting();


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=User}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default1",
                    pattern: "{controller=Collection}/{action=ItemProfile}/{name?}");
               
            });
        }
    }
}
