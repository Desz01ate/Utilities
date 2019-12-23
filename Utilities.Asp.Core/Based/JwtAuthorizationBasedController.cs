using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utilities.Asp.Core.Models;

namespace Utilities.Asp.Core.Based
{
    [ApiController]
    public abstract class JwtAuthorizationBasedController : ControllerBase
    {
        protected string _jwtIssuer { get; set; }
        protected string _jwtAudience { get; set; }
        protected string _jwtKey { get; set; }
        protected double _jwtExpiresMinute { get; set; }

        public JwtAuthorizationBasedController(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            var jwtIssuer = configuration["JwtIssuer"];
            var jwtAudience = configuration["JwtAudience"];
            var jwtKey = configuration["JwtKey"];
            var jwtExpiresMinute = Convert.ToDouble(configuration["JwtExpireMinutes"]);
            this._jwtIssuer = jwtIssuer;
            this._jwtAudience = jwtAudience;
            this._jwtKey = jwtKey;
            this._jwtExpiresMinute = jwtExpiresMinute;
        }

        /// <summary>
        /// Get method for request JWT with given user id.
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
        protected abstract bool VerifyAuthentication(string id);
    }
}