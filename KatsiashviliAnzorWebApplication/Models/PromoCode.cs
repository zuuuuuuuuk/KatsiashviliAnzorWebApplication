namespace KatsiashviliAnzorWebApplication.Models
{
    public class PromoCode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
    }
}
