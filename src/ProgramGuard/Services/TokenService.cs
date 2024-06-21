using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace ProgramGuard.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly UserManager<AppUser> _userManager;
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _config = config;
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
            _userManager = userManager;
        }
        public async Task<string> CreateTokenAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName)
            };

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var creds = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            // 创建 ClaimsIdentity 并将声明添加到其中
            var identity = new ClaimsIdentity(claims, "JWT");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity, // 设置 Subject 为包含声明的 ClaimsIdentity
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

    }
}
