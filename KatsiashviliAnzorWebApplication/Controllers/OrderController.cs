using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Enum;
namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public OrderController(IOrderService orderService, IUserService userService, IProductService productService)
        {
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrders();
            if (orders == null)
            {
                return BadRequest("there are no orders");
            }
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderService.GetOrderById(id);
            return Ok(order);
        }

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
                TotalAmount = 0,
                OrderItems = new List<OrderItem>(),
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Paid,
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


                var orderItem = new OrderItem()
                {
                    OrderId = ord.Id,
                    ProductId = OrderItem.ProductId,
                    Quantity = OrderItem.Quantity,
                    Price = product.OriginalPrice
                };



                totalAmount += orderItem.Quantity * orderItem.Price;
                ord.OrderItems.Add(orderItem);
            }

            ord.TotalAmount = totalAmount;
            _orderService.UpdateOrder(ord);


            return Ok($"{user.LastName} has successfully created an order");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, OrderDto updatedOrder)
        {
            
            var existingOrder = _orderService.GetOrderById(id);
            if (existingOrder == null)
            {
                return BadRequest("order doesn't exist");
            }

          

            
            existingOrder.ShippingAddress = updatedOrder.ShippingAddress;
            existingOrder.PaymentMethod = updatedOrder.PaymentMethod;
           


            existingOrder.OrderItems.Clear();

            decimal totalAmount = 0;

          

            foreach (var orderItem in updatedOrder.OrderItems)
            {
                var product = _productService.GetProductById(orderItem.ProductId);
                if (product == null)
                {
                    return BadRequest("product is null");
                }

                var ordItem = new OrderItem()
                {
                    OrderId = existingOrder.Id, 
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    Price = product.OriginalPrice
                };

                existingOrder.OrderItems.Add(ordItem);
                totalAmount += ordItem.Quantity * ordItem.Price;
            }

            existingOrder.TotalAmount = totalAmount;

            _orderService.UpdateOrder(existingOrder);


            return Ok($"Order with id {id} has been updated successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            _orderService.DeleteOrder(id);
            return Ok($"Order with id {id} has been deleted successfully");
        }
    }
}
