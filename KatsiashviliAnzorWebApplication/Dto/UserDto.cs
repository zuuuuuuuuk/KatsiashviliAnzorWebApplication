using KatsiashviliAnzorWebApplication.Enum;
using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Dto
{
    public class UserDto
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
