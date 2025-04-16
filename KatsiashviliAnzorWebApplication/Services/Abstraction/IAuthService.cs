using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        bool VerifyPassword(string password, string passwordHash);
        string HashPassword(string password);
        User Authenticate(string eMail, string password);
    }
}
