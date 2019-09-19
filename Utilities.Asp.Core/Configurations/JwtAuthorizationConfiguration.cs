using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Asp.Core.Configurations
{
    public static class JwtAuthorizationConfiguration
    {
        /// <summary>
        /// Set required authentication and bearer for JWT, this method need to be call on ConfigureServices on Startup.cs
        /// </summary>
        /// <param name="services">instance of IServiceCollection</param>
        /// <param name="configuration">configuration read from appsettings.json with 'JwtIssuer','JwtAudience' and 'JwtKey' properties.</param>
        public static void SetPreconfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtIssuer = configuration["JwtIssuer"];
            var jwtAudience = configuration["JwtAudience"];
            var jwtKey = configuration["JwtKey"];
            SetPreconfiguration(services, jwtIssuer, jwtAudience, jwtKey);
        }
        /// <summary>
        /// Set required authentication and bearer for JWT, this method need to be call on ConfigureServices on Startup.cs
        /// </summary>
        /// <param name="services">instance of IServiceCollection</param>
        /// <param name="validIssuer">issuer of JWT</param>
        /// <param name="validAudience">audience of JWT</param>
        /// <param name="jwtKey">key of JWT</param>
        public static void SetPreconfiguration(this IServiceCollection services, string validIssuer, string validAudience, string jwtKey)
        {
            services.
                AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).
                AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = validIssuer,
                        ValidAudience = validAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
        }
        /// <summary>
        /// (optional) You can use app.UserAuthentication(); in Startup.Configure.
        /// </summary>
        /// <param name="application"></param>
        public static void SetAppUseAuthentication(IApplicationBuilder application)
        {
            application.UseAuthentication();
        }
    }
}
