using MajstorFinder.WebAPI.DTOs;
using MajstorFinder.WebAPI.Helpers;
using MajstorFinder.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MajstorFinder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly MajstoriDbContext _context;
        private readonly IConfiguration _config;

        public UserController(MajstoriDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (_context.AppUsers.Any(u => u.Email == dto.Email || u.Username == dto.Username))
                return BadRequest("User already exists.");

            PasswordHasher.Create(dto.Password, out var hash, out var salt, out var iterations);

            var user = new AppUser
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Iterations = iterations,
                CreatedAt = DateTime.Now
            };

            _context.AppUsers.Add(user);
            _context.SaveChanges();

            return Ok(new { user.Id, user.Username, user.Email });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.AppUsers.SingleOrDefault(u => u.Email == dto.Email);
            if (user == null) return Unauthorized();

            var ok = PasswordHasher.Verify(dto.Password, user.PasswordHash, user.PasswordSalt, user.Iterations);
            if (!ok) return Unauthorized();

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        private string GenerateToken(AppUser user)
        {
            var key = _config["Jwt:Key"]!;
            var jwt = _config.GetSection("Jwt");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(jwt["ExpiresInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

