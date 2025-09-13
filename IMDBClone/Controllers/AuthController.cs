using IMDBClone.API.Models;
using IMDBClone.API.Services;
using IMDBClone.API.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IMDBClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(UserService userService, IOptions<JwtSettings> jwtOptions)
        {
            _userService = userService;
            _jwtSettings = jwtOptions.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var exists = await _userService.GetByUsernameAsync(req.Username);
            if (exists != null) return Conflict(new { message = "Username already exists" });

            var user = new User
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Roles = new List<string> { "User" },
                CreatedAt = DateTime.UtcNow
            };

            await _userService.CreateAsync(user);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, new { user.Id, user.Username, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userService.ValidateCredentialsAsync(req.Username, req.Password);
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });

            var token = GenerateToken(user);
            return Ok(new { token, expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes) });
        }

        private string GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username)
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

    public record RegisterRequest(string Username , string Email , string Password);
    public record LoginRequest(string Username ,  string Password);


}
