using AuthJWTLesson.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthJWTLesson.Services
{
    public class TokenValidationService
    {
        private readonly string key;
        private readonly IConfiguration configuration;

        public TokenValidationService(IOptions<SecretOptions> key, IConfiguration configuration)
        {
            this.key = key.Value.JWTSecret;
            this.configuration = configuration;
        }

        public string Validate(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claimPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters { 
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidAudience = configuration.GetSection("Validations").GetSection("ValidAudience").Value,
                ValidIssuer = configuration.GetSection("Validations").GetSection("ValidIssuer").Value,
                ValidateLifetime = true
            }, out _);

            var claims = "";

            foreach(var claim in claimPrincipal.Claims)
            {
                claims += claim.Type + ": " + claim.Value + "; ";
            }

            return claims;
        }
    }
}
