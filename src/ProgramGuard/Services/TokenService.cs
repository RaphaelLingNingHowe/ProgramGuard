using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProgramGuard.Config;
using ProgramGuard.Interfaces.Service;
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
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"] ?? throw new InvalidOperationException("JWT:SigningKey is not configured")));
            _userManager = userManager;
        }
        public async Task<string> CreateTokenAsync(AppUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var userId = user.Id ?? throw new InvalidOperationException("User ID is not provided.");
            var userName = user.UserName ?? throw new InvalidOperationException("User Name is not provided.");
            var visiblePrivilege = user.PrivilegeRule?.Visible ?? 0;
            var operatePrivilege = user.PrivilegeRule?.Operate ?? 0;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId),
                new Claim("visiblePrivilege", $"{(uint)visiblePrivilege}"),
                new Claim("operatePrivilege", $"{(uint)operatePrivilege}"),
            };

            var creds = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);
            var identity = new ClaimsIdentity(claims, "JWT");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddMinutes(AppSettings.ExpiresInMinutes),
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
