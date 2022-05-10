using IdentityPro.Data;
using IdentityPro.Helper;
using IdentityPro.Models.Entites;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro
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
            services.AddDbContext<DataBaseContext>(p => p.UseSqlServer("data source =.; initial catalog = IdentityDb;  integrated security = true; MultipleActiveResultSets=true "));
            services.AddControllersWithViews();

            //claim
          //  services.AddScoped<IUserClaimsPrincipalFactory<User>, AddMyClaims>();

            //Autoriztion
            services.AddAuthorization(option =>
            {
                option.AddPolicy("Buyer", policy =>
                {
                    policy.RequireClaim("Buyer");
                });

                option.AddPolicy("Cradit", policy =>
                {
                    policy.Requirements.Add(new UserCreditRequerment(1000));
                });

                option.AddPolicy("IsBlogForUser", policy =>
                {
                    policy.Requirements.Add(new BlogRequirement());
                });
            });

            ////////SettingIdntity
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<DataBaseContext>()
                .AddDefaultTokenProviders()
                .AddPasswordValidator<MyPasswordValidator>();

            services.Configure<IdentityOptions>(p=>{
                p.User.RequireUniqueEmail = true;

            });

            //policy
            services.AddScoped<IClaimsTransformation, AddClaim>();
            services.AddSingleton<IAuthorizationHandler, UserCreditHandler>();
            services.AddSingleton<IAuthorizationHandler, IsBlogForUserAuthorizationHandler>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
              );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");     
            });;
        }
    }
}
