namespace KatsiashviliAnzorWebApplication.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public enum Status
        {
            PENDING,
            PAID,
            SHIPPED,
            DELIVERED
        }
        public decimal TotalAmount { get; set; }
        public object ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
