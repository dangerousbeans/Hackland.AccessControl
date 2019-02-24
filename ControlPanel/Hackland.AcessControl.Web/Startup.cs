using Hackland.AccessControl.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;

namespace Hackland.AccessControl.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-NZ");

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private bool IsRunningInDocker
        {
            get
            {
                return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpsRedirection(options => options.HttpsPort = 443);

            services.AddDbContext<DataContext>(options =>
            {
                if (IsRunningInDocker)
                {
                    options.UseMySQL(Configuration.GetConnectionString("ProductionMySQLConnection"));
                }
                else
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DevelopmentSQLServerConnection"));
                }
            });

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"App_Data\Keys\"))
                .SetApplicationName("Hackland.AccessControl")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(60));

            services.AddDefaultIdentity<User>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (env.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            InitializeDatabase(app);
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<Data.DataContext>().Database.Migrate();
            }
        }
    }
}
