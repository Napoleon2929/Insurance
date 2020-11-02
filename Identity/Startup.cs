using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Shared.Identity;
using Identity.Data;
using Shared.Common.Extensions;

namespace Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationAssembly = typeof(Startup).Assembly.GetName().Name;
            var connectionString = Configuration["ConnectionString"];

            Action<DbContextOptionsBuilder> dbContextOptionsBuilder =
                builder => builder.UseMySql(connectionString, m =>
                {
                    m.MigrationsAssembly(migrationAssembly);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    m.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });

            services.AddDbContext<ApplicationDbContext>(dbContextOptionsBuilder);

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/Login";
                config.LogoutPath = "/Account/Logout";
            });

            IdentityModelEventSource.ShowPII = true;

            services.AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options => options.ConfigureDbContext = dbContextOptionsBuilder)
                .AddOperationalStore(options => options.ConfigureDbContext = dbContextOptionsBuilder)
                .AddAspNetIdentity<ApplicationUser>();

            services.AddCustomSwagger("v1", "IdentityServer API");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SeedData.Migrate(app);
            SeedData.InitializeDatabase(app, Configuration);

            app.UseCookiePolicy();

            app.UseForwardedHeaders();
            app.UseStaticFiles();
            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
}
