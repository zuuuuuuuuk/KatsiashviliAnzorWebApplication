using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Services.Abstraction;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        public PaymentService(AppDbContext context)
        {
            _context = context;
        }



    }
}
