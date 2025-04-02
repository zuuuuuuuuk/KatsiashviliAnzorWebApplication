namespace KatsiashviliAnzorWebApplication.Dto
{
    public class PaymentDto
    {
        public int OrderId { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string Cvv {  get; set; }
        public decimal Amount { get; set; }
    }
}
