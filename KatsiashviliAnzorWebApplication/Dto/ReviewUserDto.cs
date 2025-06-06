using KatsiashviliAnzorWebApplication.Enum;

namespace KatsiashviliAnzorWebApplication.Dto
{
    public class ReviewUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }  
        public string LastName { get; set; }
        public UserRole Role { get; set; }
    }
}
