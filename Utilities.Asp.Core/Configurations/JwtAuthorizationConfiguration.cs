using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Utilities.Asp.Core.Configurations
{
    /// <summary>
    /// Provide pre-configuration for JWT.
    /// </summary>
    public static class JwtAuthorizationConfiguration
    {
        /// <summary>
        /// Set required authentication and bearer for JWT, this method need to be call on ConfigureServices on Startup.cs
        /// </summary>
        /// <param name="services">instance of IServiceCollection</param>
        /// <param name="configuration">configuration read from appsettings.json with 'JwtIssuer','JwtAudience' and 'JwtKey' properties.</param>
        /// <exception cref="ArgumentNullException"/>
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            var jwtIssuer = configuration["JwtIssuer"];
            var jwtAudience = configuration["JwtAudience"];
            var jwtKey = configuration["JwtKey"];
            AddJwtAuthentication(services, jwtIssuer, jwtAudience, jwtKey);
        }

        /// <summary>
        /// Set required authentication and bearer for JWT, this method need to be call on ConfigureServices on Startup.cs
        /// </summary>
        /// <param name="services">instance of IServiceCollection</param>
        /// <param name="validIssuer">issuer of JWT</param>
        /// <param name="validAudience">audience of JWT</param>
        /// <param name="jwtKey">key of JWT</param>
        /// <exception cref="ArgumentNullException"/>
        public static void AddJwtAuthentication(this IServiceCollection services, string validIssuer, string validAudience, string jwtKey)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (string.IsNullOrWhiteSpace(validIssuer)) throw new ArgumentNullException(nameof(validIssuer));
            if (string.IsNullOrWhiteSpace(validAudience)) throw new ArgumentNullException(nameof(validAudience));
            if (string.IsNullOrWhiteSpace(jwtKey)) throw new ArgumentNullException(nameof(jwtKey));
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
        public static void UseJwtAuthentication(this IApplicationBuilder application)
        {
            application.UseAuthentication();
        }
    }
}