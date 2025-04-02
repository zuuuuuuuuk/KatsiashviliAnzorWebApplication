using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using KatsiashviliAnzorWebApplication.Enum;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        
        private readonly AppDbContext _context;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        public PaymentService(AppDbContext context, IOrderService orderService, ICartService cartService)
        {
            _context = context;
            _orderService = orderService;
            _cartService = cartService;
        }

        public bool ProcessPayment(PaymentDto paymentDto)
        {
            if (string.IsNullOrWhiteSpace(paymentDto.CardNumber) || paymentDto.CardNumber.Length < 16)
            {
                return false;
            }


            //
            //Band Approval Here   
            //


            // if payment is approved >>>

            var order = _orderService.GetOrderById(paymentDto.OrderId);

            if (order == null)
            {
                return false;
            }

         
            order.Status = OrderStatus.Paid;
            order.PaymentInfo = $"CardNumber: {paymentDto.CardNumber}," +
                $" ExpirationDate: {paymentDto.ExpirationDate}," +
                $" Cvv: {paymentDto.Cvv}," +
                $"Amount: {paymentDto.Amount}";

            _orderService.UpdateOrder(order);



            var cart = _context.Carts.FirstOrDefault(cart => cart.UserId == order.UserId);
            if (cart != null)
            {
                _cartService.DeleteCart(cart.Id);
            }


            return true;
        }
    }
}
