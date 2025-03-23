using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Dto
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public int CategoryId { get; set; }
        public int Stock {  get; set; }
        public ICollection<Image>? Images { get; set; }

    }
}
