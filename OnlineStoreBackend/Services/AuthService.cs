using System.Text.RegularExpressions;

namespace OnlineStoreBackend.Services
{
    public class AuthService
    {
        public static bool IsValidPassword(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$");
        }

        public static bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}
