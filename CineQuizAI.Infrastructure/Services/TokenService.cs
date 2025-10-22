using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CineQuizAI.Application.Abstractions.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CineQuizAI.Infrastructure.Services
{
    // TODO: add more claims later
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config) => _config = config;

        public string CreateToken(Guid userId, string userName, IEnumerable<string>? roles = null)
        {
            var issuer = _config["Jwt:Issuer"] ?? "CineQuizAI";
            var audience = _config["Jwt:Audience"] ?? "CineQuizAI.Clients";
            var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Missing Jwt:Key");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, userName),
            };

            if (roles != null)
                foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(12), // TODO: tune lifetime
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
