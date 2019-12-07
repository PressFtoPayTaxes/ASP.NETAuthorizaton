using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthJWTLesson.DataAccess;
using AuthJWTLesson.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthJWTLesson.Services
{
    public class AuthService
    {
        private readonly DataAccess.AppContext context;
        private readonly IConfiguration configuration;
        private readonly string jwtSecret;

        public AuthService(DataAccess.AppContext context, IOptions<SecretOptions> secretOptions, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
            jwtSecret = secretOptions.Value.JWTSecret;
        }

        public async Task<string> Authenticate(string login, string password)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(user => user.Password == password && user.Username == login);

            if(existingUser == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = configuration.GetSection("Validations").GetSection("ValidAudience").Value,
                Issuer = configuration.GetSection("Validations").GetSection("ValidIssuer").Value,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, login)
                }),
                Expires = DateTime.UtcNow.AddYears(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}