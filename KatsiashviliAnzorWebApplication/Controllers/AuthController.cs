using KatsiashviliAnzorWebApplication.Services.Abstraction;
using KatsiashviliAnzorWebApplication.Models.Auth;
using KatsiashviliAnzorWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IUserService _userService;



        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _authService.Authenticate(model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Email or password is incorrect"} );
            }

            var token = _authService.GenerateJwtToken(user);

            return Ok(new
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                Token = token
            });
        }




        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            var users = _userService.GetAllUsers();

            // preventing duplicate registration

            if (users.Any(x => x.Email == model.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = _authService.HashPassword(model.Password),
                Role = Enum.UserRole.User,
                CreatedAt = DateTime.UtcNow,
            };


            _userService.AddUser(user);

            return Ok(new { message = "Registration successful" });


        }





    }
}
