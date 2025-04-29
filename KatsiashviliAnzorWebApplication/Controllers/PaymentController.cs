using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using KatsiashviliAnzorWebApplication.Enum;
using Microsoft.AspNetCore.Mvc;


namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }


        [HttpPost("process")]
        public IActionResult ProcessPayment(PaymentDto paymentDto)
        {
            if (paymentDto == null)
            {
                return BadRequest("payment data is required");
            }

            var order = _orderService.GetOrderById(paymentDto.OrderId);
            if (order == null)
            {
                return BadRequest($"order with id {paymentDto.OrderId} not found");
            }

            if (order.Status == OrderStatus.Paid)
            {
                return BadRequest("cannot process payment because order status is already Paid");
            }
        
            // Main method here >>
            bool isPaymentSuccessful = _paymentService.ProcessPayment(paymentDto);

            if (!isPaymentSuccessful)
            {
                return BadRequest("Payment failed");
            }


            return Ok(new { message = "payment successful. Order status updated." });

        }



    }
}
