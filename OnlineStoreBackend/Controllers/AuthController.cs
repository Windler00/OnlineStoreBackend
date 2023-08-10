using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Dto;
using OnlineStoreBackend.Models;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private string SecretToken = string.Empty;
        private string Issuer = string.Empty;
        private string Audience = string.Empty;
        public AuthController(IConfiguration configuration)
        {
            SecretToken = configuration.GetSection("AppSettings:Token").Value!;
            Issuer = configuration.GetSection("AppSettings:Issuer").Value!;
            Audience = configuration.GetSection("AppSettings:Audience").Value!;
        }

        [HttpPost("registration")]
        public async Task<ActionResult> Registration(RegistrationDto request)
        {
            using (DataContext db = new DataContext())
            {
                User user = new User();
                if (!AuthService.IsValidEmail(request.Email))
                {
                    return BadRequest("Email incorrect");
                }
                if (!AuthService.IsValidPassword(request.Password))
                {
                    return BadRequest("Password incorrect(Minimum 1 uppercase, letter 1 symbol, minimum length 8 symbols)");
                }
                if (request.Email != request.EmailConfirmation)
                {
                    return BadRequest("Email and email confirmation is not same");
                }
                if (request.Password != request.PasswordConfirmation)
                {
                    return BadRequest("Password and password confirmation is not same");
                }
                else
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    user.Email = request.Email;
                    user.PasswordHash = passwordHash;
                    db.Users.Add(user);
                    db.SaveChanges();
                    var token = AuthService.CreateToken(user, SecretToken, Issuer, Audience);
                    return Ok(token);
                }
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto request)
        {
            using (DataContext db = new DataContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email.Contains(request.Email));
                if (!AuthService.IsValidEmail(request.Email))
                {
                    return BadRequest("Email is incorrect");
                }
                if (!AuthService.IsValidPassword(request.Password))
                {
                    return BadRequest("Password incorrect(Minimum 1 uppercase, letter 1 symbol, minimum length 8 symbols)");
                }
                if (request.Password != request.PasswordConfirmation)
                {
                    return BadRequest("Passwords not same");
                }
                if (user == null)
                {
                    return BadRequest("User not found");
                }
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return BadRequest("Wrong password");
                }
                else
                {
                    var token = AuthService.CreateToken(user, SecretToken, Issuer, Audience);
                    return Ok(token);
                }
            }
        }
    }
}
