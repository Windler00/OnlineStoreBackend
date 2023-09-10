using Microsoft.IdentityModel.Tokens;
using OnlineStoreBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public static string CreateToken(User user, string SecretToken, string Issuer, string Audience)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretToken));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
