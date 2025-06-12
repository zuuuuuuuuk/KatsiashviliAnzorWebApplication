namespace KatsiashviliAnzorWebApplication.Models
{
    public class PromoCode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
        public int? OwnerUserId { get; set; }
        public bool IsUsed { get; set; } = false;
        public bool IsGlobal { get; set; } = true;
    }
}
