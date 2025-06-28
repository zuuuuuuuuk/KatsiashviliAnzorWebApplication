using KatsiashviliAnzorWebApplication.Data;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IPromoCodeService _promoCodeService;

        public OrderController(IOrderService orderService, IUserService userService, IProductService productService, IPromoCodeService promoCodeService)
        {
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
            _promoCodeService = promoCodeService;
        }

        // Get all orders

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrders();
            if (orders == null)
            {
                return BadRequest("there are no orders");
            }
            ;
            return Ok(orders);
        }

        // Get specific order

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return Ok(new { message = "user has no orders" });
            }
            return Ok(order);
        }


        [HttpGet("get-orders/{userId}")]
        public IActionResult GetOrdersByUserId(int userId)
        {
            var orders = _orderService.GetOrdersByUserId(userId);
            if (orders == null)
            {
                return Ok(new {message = "user has no orders"});
            }
            else
            {
                return Ok(orders);
            }
        }


        // Create new order

        [HttpPost]
        public IActionResult AddOrder(OrderDto order)
        {


            var user = _userService.GetUserById(order.UserId);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var ord = new Order()
            {
                UserId = order.UserId,
                PromoCode = order.PromoCode,
                OrderItems = new List<OrderItem>(),
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 0
            };

            

            _orderService.AddOrder(ord);


            decimal totalAmount = 0;


            if (order.OrderItems == null)
            {
                return BadRequest("orderitems are null");
            }

            foreach (var OrderItem in order.OrderItems)
            {
                var product = _productService.GetProductById(OrderItem.ProductId);
                if (product == null)
                {
                    return BadRequest("product is null");
                }

                decimal discountedPrice = product.DiscountedPrice;


                var orderItem = new OrderItem()
                {
                    OrderId = ord.Id,
                    ProductId = OrderItem.ProductId,
                    Quantity = OrderItem.Quantity,
                    Price = product.OriginalPrice,
                    DiscountedPrice = discountedPrice                    
                };

                if (discountedPrice > 0)
                {
                    totalAmount += orderItem.Quantity * orderItem.DiscountedPrice;
                } else
                {
                    totalAmount += product.OriginalPrice * OrderItem.Quantity;
                }

                
                ord.OrderItems.Add(orderItem);
            }

            if (!string.IsNullOrWhiteSpace(order.PromoCode))
            {
                var promoCode = _promoCodeService.getPromoCodeByCode(order.PromoCode.ToLower());
        if (promoCode == null)
                {
                    return BadRequest("existing promocode was not found");
                }
                // updates final discountValue for order depending on it's previous sale discounts
        
        if (!promoCode.IsGlobal)
                {
                    if (promoCode.OwnerUserId != user.Id)
                        return BadRequest("promo code is not yours");
                }
                if (promoCode.IsUsed)
                    return BadRequest("promo code is already used");

                decimal discountAmount = (promoCode.DiscountValue / 100) * totalAmount;
                promoCode.IsUsed = true;
                _promoCodeService.UpdatePromoCode(promoCode);
                totalAmount -= discountAmount;
            } else
            {
                ord.PromoCode = null;
            }

                ord.TotalAmount = totalAmount;
            _orderService.UpdateOrder(ord);

            
            
            return Ok(ord);
        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}/status")]
        public IActionResult UpdateOrder(int id, OrderStatus status)
        {

            var existingOrder = _orderService.GetOrderById(id);

            if (existingOrder == null)
            {
                return BadRequest("order with that id doesnt exist");
            }
            existingOrder.Status = status;
            _orderService.UpdateOrder(existingOrder);


            return Ok($"Order status with id {id} has been updated successfully");
        }




        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
           var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return BadRequest("can not find order with that id");
            }
            _orderService.DeleteOrder(id);
            return Ok( new { message = $"Order with id {id} has been deleted successfully" });
        }
    }
}
