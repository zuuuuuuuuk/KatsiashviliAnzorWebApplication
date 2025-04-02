using KatsiashviliAnzorWebApplication.Dto;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface IPaymentService
    {
        bool ProcessPayment(PaymentDto paymentDto);
    }
}
