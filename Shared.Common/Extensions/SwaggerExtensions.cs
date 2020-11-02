using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Common.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, string version, string name)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = name,
                    Version = version
                });
                options.EnableAnnotations();
            });

            return services;
        }

        public static IServiceCollection AddCustomSwaggerWithOAuth<T>(this IServiceCollection services,
            string version, string name, OpenApiSecurityScheme scheme)
            where T : IOperationFilter
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = name,
                    Version = version
                });
                options.EnableAnnotations();
                options.OperationFilter<T>();
                options.AddSecurityDefinition("oauth2", scheme);
            });

            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, string name)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", name);
            });

            return app;
        }

        public static IApplicationBuilder UseCustomSwaggerWithOAuth(this IApplicationBuilder app, string name)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", name);
                c.OAuthClientId("SwaggerId");
                c.OAuthAppName("Swagger UI");
            });

            return app;
        }
    }
}
