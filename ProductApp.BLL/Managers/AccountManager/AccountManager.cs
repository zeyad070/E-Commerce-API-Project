using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductApp.Common.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductApp.BLL
{
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly AdminSettings _adminSettings;

        public AccountManager(
            UserManager<IdentityUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            IOptions<AdminSettings> adminSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _adminSettings = adminSettings.Value;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDTO model, CancellationToken cancellationToken = default)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(_adminSettings.SecretCode) && model.AdminCode == _adminSettings.SecretCode)
                    await _userManager.AddToRoleAsync(user, "Admin");
                else
                    await _userManager.AddToRoleAsync(user, "User");
            }
            return result;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO model, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return null;

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return new AuthResponseDTO
            {
                UserId = user.Id,
                Token = token,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles,
                Expiration = DateTime.UtcNow.AddDays(_jwtSettings.ExpireDays)
            };
        }

        private string GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwtSettings.ExpireDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
