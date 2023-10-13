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
        private readonly IHttpContextAccessor _httpContextAccessor;

        private string SecretToken = string.Empty;
        private string Issuer = string.Empty;
        private string Audience = string.Empty;
        public AuthController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            SecretToken = configuration.GetSection("AppSettings:Token").Value!;
            Issuer = configuration.GetSection("AppSettings:Issuer").Value!;
            Audience = configuration.GetSection("AppSettings:Audience").Value!;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("registration")]
        public async Task<ActionResult> Registration(RegistrationDto request)
        {
            using (DataContext db = new DataContext())
            {
                User user = new User();
                var userExist = db.Users.FirstOrDefault(u => u.Email.Contains(request.Email));
                if (userExist != null)
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
                if (request.Username.Length < 3 & request.Username.Length < 30)
                {
                    return BadRequest(new { message = "Username incorrect(Minimum length 3, maximum length 30)" });
                }
                else
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    user.Email = request.Email;
                    user.PasswordHash = passwordHash;
                    user.Name = request.Username;
                    db.Users.Add(user);
                    db.SaveChanges();
                    var token = AuthService.CreateToken(user, SecretToken, Issuer, Audience);
                    return Ok(new { name = user.Name, role = user.Role, token = token });
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
                    return Ok(new { name = user.Name, role = user.Role, avatar = user.Avatar, token = token });
                }
            }
        }

        [HttpPost("changename")]
        [Authorize]
        public async Task<ActionResult> ChangeName(ChangeNameDto request)
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
                user.Name = request.Name;
                db.SaveChanges();
                return Ok(new { message = "Username changed" });
            }
        }

        [HttpPost("changeemail")]
        [Authorize]
        public async Task<ActionResult> ChangeEmail(ChangeEmailDto request)
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
                if (request.NewPass != request.NewPassRepeat)
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
        [HttpPost("uploadavatar")]
        [Authorize]
        public async Task<ActionResult> UploadAvatar(IFormFile file)
        {
            if (file != null && file.Length > 0)
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
                    var oldFileName = Path.GetFileName(user.Avatar);
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatar", oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    string fileExtension = Path.GetExtension(file.FileName);
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatar", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var request = _httpContextAccessor.HttpContext.Request;
                    var serverUrl = $"{request.Scheme}://{request.Host.Value}";
                    user.Avatar = serverUrl + $"/avatar/{fileName}";
                    db.SaveChanges();
                    return Ok(new { message = "Avatar uploaded", avatar = user.Avatar });
                }
            }
            else
            {
                return BadRequest(new { message = "Something went wrong avatar is not loaded" });
            }
        }
        [HttpPost("getusers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUsers(GetUsers request)
        {
            using (DataContext db = new DataContext())
            {
                var users = db.Users
                    .OrderBy(u => u.Id)
                    .Skip(request.First)
                    .Take(request.Last)
                    .ToList();
                return Ok(users);
            }
        }
        [HttpPost("changerole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeRole(ChangeRoleDto request)
        {
            using (DataContext db = new DataContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == request.Id);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }
                if (request.Role == "User" || request.Role == "Seller" || request.Role == "Admin")
                {
                    user.Role = request.Role;
                    db.SaveChanges();
                    return Ok(new { message = "Role changed" });
                }
                return BadRequest(new { message = "Incorrect role" });
            }
        }
    }
}