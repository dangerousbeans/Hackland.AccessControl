using Hackland.AccessControl.Data;
using Hackland.AccessControl.Shared;
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
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-NZ");

            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            //services.AddHttpsRedirection(options => options.HttpsPort = 443);
            services.Configure<ApplicationConfigurationModel>(Configuration.GetSection("Application"));

            services.AddDbContext<DataContext>(options =>
            {

                string connectionString;
                if (Settings.IsRunningInDocker)
                {
                    connectionString = Configuration.GetConnectionString("ProductionMySQLConnection");
                    options.UseMySql(connectionString, mySqlOptions => mySqlOptions.ServerVersion(new Version(6, 7, 17), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql));
                }
                else
                {
                    if (Settings.UseSqlServer)
                    {
                        connectionString = Configuration.GetConnectionString("DevelopmentSQLServerConnection");
                        options.UseSqlServer(connectionString);
                    }
                    else
                    {
                        connectionString = Configuration.GetConnectionString("DevelopmentMySQLConnection");
                        options.UseMySql(connectionString, mySqlOptions => mySqlOptions.ServerVersion(new Version(6, 7, 17), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql));
                    }
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

            //if (env.IsProduction())
            //{
            //    app.UseHttpsRedirection();
            //}

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
