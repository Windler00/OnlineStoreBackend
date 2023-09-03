using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Dto;
using OnlineStoreBackend.Dto.Auth;
using OnlineStoreBackend.Models;
using OnlineStoreBackend.Services;
using System.Security.Claims;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/auth")]
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
                var userExist = db.Users.FirstOrDefault(u => u.Email.Contains(request.Email));
                if(userExist != null)
                {
                    return BadRequest(new { message = "User already exist" });
                }
                if (!AuthService.IsValidEmail(request.Email))
                {
                    return BadRequest(new { message = "Email incorrect" });
                }
                if (!AuthService.IsValidPassword(request.Password))
                {
                    return BadRequest(new { message = "Password incorrect(Minimum 1 uppercase, letter 1 symbol, minimum length 8 symbols)" });
                }
                if (request.Email != request.EmailConfirmation)
                {
                    return BadRequest(new { message = "Email and email confirmation is not same" });
                }
                if (request.Password != request.PasswordConfirmation)
                {
                    return BadRequest(new { message = "Password and password confirmation is not same" });
                }
                else
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    user.Email = request.Email;
                    user.PasswordHash = passwordHash;
                    db.Users.Add(user);
                    db.SaveChanges();
                    var token = AuthService.CreateToken(user, SecretToken, Issuer, Audience);
                    return Ok(new { token = token });
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
                    return BadRequest(new { message = "Email is incorrect" });
                }
                if (!AuthService.IsValidPassword(request.Password))
                {
                    return BadRequest(new { message = "Password incorrect(Minimum 1 uppercase, letter 1 symbol, minimum length 8 symbols)" });
                }
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return BadRequest(new { message = "Wrong password" });
                }
                else
                {
                    var token = AuthService.CreateToken(user, SecretToken, Issuer, Audience);
                    return Ok(new { token = token });
                }
            }
        }

        [HttpPost("changename")]
        [Authorize]
        public async Task<ActionResult> ChangeName (ChangeNameDto request)
        {
            using (DataContext db = new DataContext())
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var user = db.Users.FirstOrDefault(u =>  u.Email.Contains(email));
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }
                user.Name = request.Name;
                db.SaveChanges();
                return Ok(new { message = "Username changed" });
            }
        }

        [HttpPost("changeemail")]
        [Authorize]
        public async Task<ActionResult> ChangeEmail (ChangeEmailDto request)
        {
            using (DataContext db = new DataContext())
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var user = db.Users.FirstOrDefault(u => u.Email.Contains(email));
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }
                if (!AuthService.IsValidEmail(request.Email))
                {
                    return BadRequest(new { message = "Email incorrect" });
                }
                user.Email = request.Email;
                db.SaveChanges();
                return Ok(new { message = "Email changed" });
            }
        }
        [HttpPost("changepass")]
        [Authorize]
        public async Task<ActionResult> ChangePass(ChangePassDto request)
        {
            using (DataContext db = new DataContext())
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;
                var user = db.Users.FirstOrDefault(u => u.Email.Contains(email));
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }
                if(request.NewPass != request.NewPassRepeat)
                {
                    return BadRequest(new { message = "Password and repeat password not same" });
                }
                if (!AuthService.IsValidPassword(request.NewPass))
                {
                    return BadRequest(new { message = "Password incorrect" });
                }
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPass);
                user.PasswordHash = passwordHash;
                db.SaveChanges();
                return Ok(new { message = "Password changed" });
            }
        }
    }
}