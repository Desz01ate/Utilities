using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        //protected readonly Dictionary
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
        /// Post method for request new JWT with expired access token and refresh token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual Response<JwtTokenEntity> RefreshToken([FromBody] JwtTokenEntity token)
        {
            var principal = GetPrincipalFromExpiredToken(token.AccessToken);
            var username = principal.Identity.Name;
            var savedRefreshToken = RetrieveRefreshTokenFromPreferredDataSource(username);
            if (savedRefreshToken != token.RefreshToken)
                throw new SecurityTokenException($"Invalid refresh token.");
            var accessToken = GenerateAccessToken(username, out var expires);
            var refreshToken = GenerateRefreshToken();
            SaveRefreshTokenToPreferredDataSource(username, refreshToken);
            return new Response<JwtTokenEntity>(true, null, new JwtTokenEntity
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ValidUntil = expires
            });
        }
        /// <summary>
        /// Get method for request JWT with given user id.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Response of request in form of { success, message , data }</returns>
        [HttpGet]
        public virtual Response<JwtTokenEntity> Authenticate([FromQuery]string username, [FromQuery]string password)
        {
            var exists = VerifyAuthentication(username, password);
            if (!exists)
                return new Response<JwtTokenEntity>(false, NotFound().StatusCode.ToString(), null);
            var accessToken = GenerateAccessToken(username, out var expires);
            var refreshToken = GenerateRefreshToken();
            SaveRefreshTokenToPreferredDataSource(username, refreshToken);
            return new Response<JwtTokenEntity>(true, Ok().StatusCode.ToString(), new JwtTokenEntity()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ValidUntil = expires
            });
        }
        /// <summary>
        /// Delete method for revoke access token.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpDelete]
        public virtual Response Revoke([FromQuery]string data)
        {
            throw new NotImplementedException();
        }
        private string GenerateAccessToken(string id, out DateTime expires)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,id),
                    new Claim(JwtRegisteredClaimNames.Sub, id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtExpiresMinute));/*DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));*/

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }
        /// <summary>
        /// Generate pseudo-random refresh token to use alongside access token.
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateRefreshToken()
        {
            var entropy = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(entropy);
            return Convert.ToBase64String(entropy);
        }
        /// <summary>
        /// Retrieve jwt principal from expired token, useful for re-generate new token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokenValidationParameters"></param>
        /// <returns></returns>
        protected ClaimsPrincipal GetPrincipalFromExpiredToken(string token, TokenValidationParameters tokenValidationParameters = null)
        {
            tokenValidationParameters ??= new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException($"Token is invalid.");
            return principal;
        }
        /// <summary>
        /// Verify any request for JWT from Authenticate method by given id and return if the user is valid or not
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>boolean which indicate whether the user is allow to get the JWT or not</returns>
        protected abstract bool VerifyAuthentication(string username, string password);
        /// <summary>
        /// Retrieve refresh token correspond to username from preferred datasource.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        protected abstract string RetrieveRefreshTokenFromPreferredDataSource(string username);
        /// <summary>
        /// Save refresh token alongside username to any preferred datasource.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="refreshToken"></param>
        protected abstract void SaveRefreshTokenToPreferredDataSource(string username, string refreshToken);
    }
}