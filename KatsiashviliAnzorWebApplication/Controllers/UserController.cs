using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Enum;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public UserController(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
        }


        [HttpGet]

        public IActionResult GetUsers()
        {
            var user = _userService.GetAllUsers();
            if (user == null)
            {
                return BadRequest("there are no users");
            }
            return Ok(user);
        }

        [HttpGet("{id}")]

        public IActionResult GetUserById(int id)
        {
            var user = _userService.GetUserById(id);
            return Ok(user);
        }

        [HttpPost]

        public IActionResult AddUser(UserDto user)
        {
            if (user == null)
            {
                return BadRequest("user is null");
            }

            User u = new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.User,
            };
            _userService.AddUser(u);

            return Ok("User successfully added");
        }



        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UserDto user)
        {

            User us = _userService.GetUserById(id);
            if (us == null)
            {
                return BadRequest("user is null");
            }

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                us.FirstName = user.FirstName;

            if (!string.IsNullOrWhiteSpace(user.LastName))
                us.LastName = user.LastName;

            if (!string.IsNullOrWhiteSpace(user.Email))
                us.Email = user.Email;

            if (!string.IsNullOrWhiteSpace(user.Password))
                us.Password = user.Password;

            _userService.UpdateUser(us);

            return Ok("user updated");
        }

        [HttpGet("{id}/orders")]
        public IActionResult GetOrdersByUserId(int id)
        {
        User user = _userService.GetUserById(id);
            if (user == null)
            {
                return BadRequest("User with that Id does not exist");
            }
            else if (user.Orders == null)
            {
                return BadRequest("User got no orders");
            }
            else
            {
                List<Order> orders = _orderService.GetOrdersByUserId(id);
                if (orders == null) 
                {
                    return BadRequest("orders are null");
                }

                return Ok(orders);
            }
            }

        [HttpDelete("{id}")]

        public IActionResult DeleteUser(int id)
        {
            _userService.DeleteUser(id);
            return Ok("User has been deleted");
        }

    }
}
