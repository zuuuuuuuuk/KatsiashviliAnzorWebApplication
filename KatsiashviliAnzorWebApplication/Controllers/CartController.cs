using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IUserService userService, IProductService productService)
        {
            _cartService = cartService;
            _userService = userService;
            _productService = productService;
        }


        [HttpGet]
        public IActionResult GetAllCarts() 
        {
            var carts = _cartService.GetAllCarts();
            if (carts == null)
            {
                return NotFound("carts not found");
            }
            return Ok(carts);
        }

        [HttpGet("id")]
        public IActionResult GetCartById(int id)
        {
            var cart = _cartService.GetCartById(id);
            return Ok(cart);
        }


        [HttpPost("create")]
        public IActionResult CreateNewCart(CartDto cartDto)
        {
            if (cartDto == null)
            {
                return BadRequest("Cart is null");
            }
            var user = _userService.GetUserById(cartDto.UserId);
            if (user == null)
            {
                return BadRequest($"user with userId: {cartDto.UserId} not found");
            }
            var cart = new Cart()
            {
                UserId = cartDto.UserId,
                CartItems = new List<CartItem>(),
                User = user,
                CreatedAt = DateTime.UtcNow,
            };
            _cartService.CreateNewCart(cart);

            if (cartDto.CartItems != null && cartDto.CartItems.Any())
            {
                foreach (var item in cartDto.CartItems)
                {
                    var product = _productService.GetProductById(item.ProductId);
                    if (product != null)
                    {
                        var cartItem = new CartItem()
                        {
                            CartId = cart.Id,
                            ProductId = product.Id,
                            Quantity = item.Quantity
                        };
                        cart.CartItems.Add(cartItem);
                    }
                }
                _cartService.UpdateCart(cart);
            }

            return Ok("cart created successfully");
        }

    }
}
