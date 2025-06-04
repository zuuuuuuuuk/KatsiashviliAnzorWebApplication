using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Enum;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Policy = "AdminOnly")]
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
                PasswordHash = user.PasswordHash,
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.User,
            };
            _userService.AddUser(u);

            return Ok("User successfully added");
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UserDto user)
        {

            User us = _userService.GetUserById(id);
            if (us == null)
            {
                return BadRequest("user is null");
            }

            if (!string.IsNullOrWhiteSpace(user.FirstName) && user.FirstName != "string")
                us.FirstName = user.FirstName;

            if (!string.IsNullOrWhiteSpace(user.LastName) && user.LastName != "string")
                us.LastName = user.LastName;

            if (!string.IsNullOrWhiteSpace(user.Email) && user.Email != "string")
                us.Email = user.Email;


            if (!string.IsNullOrWhiteSpace(user.PasswordHash) && user.PasswordHash != "string")
                us.PasswordHash = user.PasswordHash;

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

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]

        public IActionResult DeleteUser(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return BadRequest("can not find user with that id");
            }
            _userService.DeleteUser(id);
            return Ok( new { message = "User has been deleted" });
        }


        [HttpPost("{userId}/favorites/{productId}")]
        public IActionResult ToggleFavoriteProduct(int userId, int productId)
        {
            var user = _userService.GetUserById(userId);
            if (user == null)
                return NotFound("User not found");

            if (user.FavoriteProductIds == null)
                user.FavoriteProductIds = new List<int>();

            if (!user.FavoriteProductIds.Contains(productId))
                user.FavoriteProductIds.Add(productId);
            else
                user.FavoriteProductIds.Remove(productId); // toggle logic

            _userService.UpdateUser(user); // persist changes

            return Ok(user.FavoriteProductIds); // send back updated list
        }

       

        [HttpGet("{userId}/favorites")]
        public IActionResult GetFavoriteProducts(int userId)
        {
            var user = _userService.GetUserById(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(user.FavoriteProductIds);
        }


    }
}
