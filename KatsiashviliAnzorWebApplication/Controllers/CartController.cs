using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Enum;
using KatsiashviliAnzorWebApplication.Services.Implementation;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IUserService userService, IProductService productService, IOrderService orderService)
        {
            _cartService = cartService;
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
        }

        // Get all carts

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

        // Create new cart

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

            var existingCart = _cartService.GetCartByUserId(cartDto.UserId);
            if (existingCart != null)
            {
                return BadRequest($"User with id {cartDto.UserId} already has an active cart");
            }


            var cart = new Cart()
            {
                UserId = cartDto.UserId,
                CartItems = new List<CartItem>(),
                User = user,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
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
                        product.Stock -= item.Quantity;
                        _productService.UpdateProduct(product);
                        cart.CartItems.Add(cartItem);
                    }
                }
                _cartService.UpdateCart(cart);
            }


            return Ok("cart created successfully");
        }

        // Update cart

        [HttpPut("{id}")]
        public IActionResult UpdateCart(int id, CartDto cartDto)
        {
            if (cartDto == null)
            {
                return BadRequest("Cart data is null.");
            }

            var existingCart = _cartService.GetCartById(id);
            if (existingCart == null)
            {
                return NotFound($"Cart with ID {id} was not found.");
            }

            if (cartDto.UserId > 0)
            {
                existingCart.UserId = cartDto.UserId;
            }

            // Restore Stock for Old Items (return stock to products)
            foreach (var oldItem in existingCart.CartItems.ToList())
            {
                var oldProduct = _productService.GetProductById(oldItem.ProductId);
                if (oldProduct != null)
                {
                    oldProduct.Stock += oldItem.Quantity; // Restoring stock
                    _productService.UpdateProduct(oldProduct);
                }
            }

            // Clear existing cart items before adding new ones
            existingCart.CartItems.Clear();

            // Add New Items & Deduct Stock
            if (cartDto.CartItems != null && cartDto.CartItems.Any())
            {
                foreach (var newItem in cartDto.CartItems)
                {
                    var newProduct = _productService.GetProductById(newItem.ProductId);
                    if (newProduct != null)
                    {
                        // Ensure stock is available before adding the new item
                        if (newProduct.Stock < newItem.Quantity)
                        {
                            return BadRequest($"Not enough stock for product ID {newProduct.Id}");
                        }

                        // Deduct new stock amount
                        newProduct.Stock -= newItem.Quantity;
                        _productService.UpdateProduct(newProduct);

                        var cartItem = new CartItem()
                        {
                            CartId = existingCart.Id,
                            ProductId = newProduct.Id,
                            Quantity = newItem.Quantity
                        };
                        existingCart.CartItems.Add(cartItem);
                    }
                }
            }

            // Updates Cart
            _cartService.UpdateCart(existingCart);

            return Ok("Cart updated successfully.");
        }


        // Add products to existing cart with productIds

        [HttpPut("add-products/{id}")]
        public IActionResult AddProductsToCart(int id, List<int> productIds)
        {
            var cart = _cartService.GetCartById(id);
            if (cart == null)
            {
                return NotFound("Cart not found");
            }

            foreach (var productId in productIds)
            {
                var product = _productService.GetProductById(productId);
                if (product == null || product.Stock < 1)
                {
                    return BadRequest($"Product with ID {productId} is not available");
                }

                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += 1; // Increase quantity if already in cart
                }
                else
                {
                    cart.CartItems.Add(new CartItem { CartId = cart.Id, ProductId = productId, Quantity = 1 });
                }

                product.Stock -= 1; // Reduce stock for each product added
                _productService.UpdateProduct(product);
            }

            _cartService.UpdateCart(cart);
            return Ok("Products added successfully");
        }



        // Remove products from cart with productIds

        [HttpPut("remove-products/{id}")]
        public IActionResult DeleteProductsFromCart(int id, List<int> productIds)
        {
            var cart = _cartService.GetCartById(id);
            if (cart == null)
            {
                return NotFound("Cart not found");
            }

            foreach (var productId in productIds)
            {
                var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (item == null)
                {
                    continue; // Skip if item not found in cart
                }

                var product = _productService.GetProductById(productId);
                if (product == null)
                {
                    continue; // Skip if product not found
                }

                product.Stock += item.Quantity; // Restore stock
                _productService.UpdateProduct(product);
                cart.CartItems.Remove(item);
            }

            _cartService.UpdateCart(cart);
            return Ok("Products removed successfully");
        }





        // optional but necessary for dev
        [HttpDelete("remove-and-UpdateProductStockNumber/{cartId}")]
        public IActionResult RemoveCartWithStockRestoration(int cartId)
        {
            var cart = _cartService.GetCartById(cartId);
            if (cart == null)
            {
                return NotFound("Cart not found");
            }
            

            // Restore stock for each item in the cart
            foreach (var item in cart.CartItems)
            {
                var product = _productService.GetProductById(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                    _productService.UpdateProduct(product);
                }
            }

            // Remove the cart
            _cartService.DeleteCart(cartId);

            return Ok("Cart removed and product stocks restored.");
        }


        // Unnecessary in many cases

        [HttpDelete("{id}")]
        public IActionResult DeleteCart(int id)
        {
            var cart = _cartService.GetCartById(id);
            if (cart == null)
            {
                return BadRequest($"cart with id {id} was not found");
            }
            _cartService.DeleteCart(id);
            return Ok($"cart with id {id} was deleted successfully");
        }



    }
}
