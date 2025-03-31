namespace KatsiashviliAnzorWebApplication.Models
{
    public class Sale
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public decimal DiscountValue { get; set; }
        public string Description { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Product>? ProductsOnThisSale { get; set; }
    }
}
