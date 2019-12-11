using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace Utilities.Asp.Core.Configurations
{
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Add Swagger generate options to IServiceCollection, this method must be called on ConfigureServices method in order to take effects.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        public static void AddSwaggerServices(this IServiceCollection services, IDictionary<string, Info> configs)
        {
            services.AddSwaggerGen(context =>
            {
                foreach (var config in configs)
                {
                    context.SwaggerDoc(config.Key, config.Value);
                }
            });
        }

        /// <summary>
        /// Add Swagger services to HTTP request pipeline, this method must be called on Configure method in order to take effects.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="routePrefix"></param>
        public static void UseSwaggerRequest(this IApplicationBuilder application, string name, string url, string routePrefix = "")
        {
            application.UseSwagger();
            application.UseSwaggerUI(context =>
            {
                context.SwaggerEndpoint(url, name);
                context.RoutePrefix = routePrefix;
            });
        }

        /// <summary>
        /// Add Swagger services to HTTP request pipeline, this method must be called on Configure method in order to take effects.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="action"></param>
        public static void UseSwaggerRequest(this IApplicationBuilder application, Action<SwaggerUIOptions> action)
        {
            application.UseSwagger();
            application.UseSwaggerUI(action);
        }
    }
}