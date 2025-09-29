using IMDBClone.API.Controllers;
using IMDBClone.API.Models;
using IMDBClone.API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IMDBClone.API.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserService userService, IOptions<JwtSettings> jwtOptions)
        {
            _userService = userService;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<User?> RegisterAsync(RegisterRequest req)
        {
            var exists = await _userService.GetByUsernameAsync(req.Username);
            if (exists != null) return null;

            var user = new User
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Roles = new List<string> { "User" },
                CreatedAt = DateTime.UtcNow
            };

            await _userService.CreateAsync(user);
            return user;
        }

        public async Task<string?> LoginAsync(LoginRequest req)
        {
            var user = await _userService.ValidateCredentialsAsync(req.Username, req.Password);
            if (user == null) return null;

            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub , user.Id),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.Username)
            };

            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: creds
        );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
