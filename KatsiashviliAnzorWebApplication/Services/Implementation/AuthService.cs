using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;


        public AuthService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        // Authenticate user by checking username and password
        public User Authenticate(string eMail, string password)
        {
            var user = _userService.GetAllUsers().FirstOrDefault(x => x.Email == eMail);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        // Generate JWT token for authenticated users
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token valid for 7 days
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Verify password hash
        public bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;
        }
    }
}
