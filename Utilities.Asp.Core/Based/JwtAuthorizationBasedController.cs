using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utilities.Asp.Core.Models;

namespace Utilities.Asp.Core.Based
{
    [ApiController]
    public abstract class JwtAuthorizationBasedController : ControllerBase
    {
        protected static string _jwtIssuer { get; set; }
        protected static string _jwtAudience { get; set; }
        protected static string _jwtKey { get; set; }
        protected static double _jwtExpiresMinute { get; set; }
        protected readonly IConfiguration _configuration;
        public JwtAuthorizationBasedController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Get method for request JWT with given user id
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Response of request in form of { success, message , data }</returns>
        [HttpGet]
        public virtual Response Authenticate([FromQuery]string id)
        {
            var exists = VerifyAuthentication(id);
            if (!exists)
                return new Response(false, NotFound().StatusCode.ToString(), string.Empty);
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtExpiresMinute));/*DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));*/

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            var jwtTokenHandler = new JwtSecurityTokenHandler().WriteToken(token);
            return new Response(true, Ok().StatusCode.ToString(), jwtTokenHandler);
        }
        /// <summary>
        /// Verify any request for JWT from Authenticate method by given id and return if the user is valid or not
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>boolean which indicate whether the user is allow to get the JWT or not</returns>
        protected virtual bool VerifyAuthentication(string id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Set required authentication and bearer for JWT, this method need to be call on ConfigureServices on Startup.cs
        /// </summary>
        /// <param name="services">instance of IServiceCollection</param>
        /// <param name="configuration">configuration read from appsettings.json with 'JwtIssuer','JwtAudience' and 'JwtKey' properties.</param>
        public static void SetPreconfiguration(ref IServiceCollection services, IConfiguration configuration)
        {
            var jwtIssuer = configuration["JwtIssuer"];
            var jwtAudience = configuration["JwtAudience"];
            var jwtKey = configuration["JwtKey"];
            var jwtExpiresMinute = Convert.ToDouble(configuration["JwtExpireMinutes"]);
            SetPreconfiguration(ref services, jwtIssuer, jwtAudience, jwtKey, jwtExpiresMinute);
        }
        /// <summary>
        /// Set required authentication and bearer for JWT, this method need to be call on ConfigureServices on Startup.cs
        /// </summary>
        /// <param name="services">instance of IServiceCollection</param>
        /// <param name="validIssuer">issuer of JWT</param>
        /// <param name="validAudience">audience of JWT</param>
        /// <param name="jwtKey">key of JWT</param>
        public static void SetPreconfiguration(ref IServiceCollection services, string validIssuer, string validAudience, string jwtKey, double jwtExpiresMinute)
        {
            _jwtIssuer = validIssuer;
            _jwtAudience = validAudience;
            _jwtKey = jwtKey;
            _jwtExpiresMinute = jwtExpiresMinute;
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
        public static void SetAppUseAuthentication(ref IApplicationBuilder application)
        {
            application.UseAuthentication();
        }
    }
}
