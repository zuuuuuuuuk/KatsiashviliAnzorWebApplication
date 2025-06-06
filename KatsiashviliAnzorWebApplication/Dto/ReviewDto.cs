namespace KatsiashviliAnzorWebApplication.Dto
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public double Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime CreatedAt { get; set; }

        public ReviewUserDto User { get; set; }
    }
}