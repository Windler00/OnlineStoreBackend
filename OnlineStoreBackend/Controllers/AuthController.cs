using Microsoft.AspNetCore.Http;
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
        [HttpPost("registration")]
        public async Task<ActionResult> Registration(RegistrationDto request)
        {
            User user = new User();
            if(!AuthService.IsValidEmail(request.Email))
            {
                return BadRequest("Email incorrect");
            }
            if (!AuthService.IsValidPassword(request.Password))
            {
                return BadRequest("Password incorrect(Minimum 1 uppercase, letter 1 symbol, minimum length 8 symbols)");
            }
            if(request.Email != request.EmailConfirmation)
            {
                return BadRequest("Email and email confirmation is not same");
            }
            if(request.Password != request.PasswordConfirmation)
            {
                return BadRequest("Password and password confirmation is not same");
            }
            else
            {
                user.Email = request.Email;
                user.PasswordHash = request.Password;
                DataContext.Users.Add(user);
                return Ok(user);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto request)
        {
            var user = DataContext.Users.Find(u => u.Email.Contains(request.Email));
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
            if (user.PasswordHash != request.Password)
            {
                return BadRequest("Wrong password");
            }
            else
            {
                return Ok(user);
            }
        }
    }
}
