namespace KatsiashviliAnzorWebApplication.Dto
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
