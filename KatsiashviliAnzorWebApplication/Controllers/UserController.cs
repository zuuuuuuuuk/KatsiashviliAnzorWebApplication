﻿using KatsiashviliAnzorWebApplication.Services.Abstraction;
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


        
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UpdateUserDto user)
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

            if (user.Role != null)
            {
                if (user.Role == 0)
                {
                    us.Role = UserRole.User;
                }
                else if (user.Role == 2)
                {
                    us.Role = UserRole.Admin;
                }
            }
            _userService.UpdateUser(us);

            return Ok( new { message = "user updated" });
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


        // DeliveryEndpoints



        // GET api/user/{userId}/deliveryAddresses
        [HttpGet("{userId}/deliveryAddresses")]
        public IActionResult GetDeliveryAddresses(int userId)
        {
            try
            {
                var addresses = _userService.GetDeliveryAddressesByUserId(userId);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/user/{userId}/deliveryAddresses
        [HttpPost("{userId}/deliveryAddresses")]
        public IActionResult AddDeliveryAddress(int userId, [FromBody] UpdateUserDelAddressDto dto)
        {
            
            try 
            {
                _userService.AddDeliveryAddress(userId, dto.Address);
                return Ok(new { message = "Address added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/user/{userId}/deliveryAddresses/{addressId}

        [HttpPut("{userId}/deliveryAddresses/{addressId}")]
        public IActionResult UpdateDeliveryAddress(int userId, int addressId, [FromBody] UpdateUserDelAddressDto dto)
        {
            try
            {
                _userService.UpdateDeliveryAddress(userId, addressId, dto.Address, dto.isDefault);
                return Ok(new { message = "Address updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/user/{userId}/deliveryAddresses/{addressId}
        [HttpDelete("{userId}/deliveryAddresses/{addressId}")]
        public IActionResult DeleteDeliveryAddress(int userId, int addressId)
        {
            try    
            {
                _userService.DeleteDeliveryAddress(userId, addressId);
                return Ok(new { message = "Address deleted successfully." });
            }
            catch (Exception ex)
            {
                
                return BadRequest(ex.Message);
            }
        }


    }
}
