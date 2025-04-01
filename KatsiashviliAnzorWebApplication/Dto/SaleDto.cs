using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Dto
{
    public class SaleDto
    {
        public string Name { get; set; }
        public decimal DiscountValue { get; set; }
        public string Description { get; set; }
        public string? StartsAt { get; set; }
        public string? EndsAt { get; set; }
        public bool IsActive { get; set; }
        public List<int>? ProductIdsOnThisSale { get; set; }
    }
}
