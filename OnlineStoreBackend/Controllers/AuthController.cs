using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Dto;
using OnlineStoreBackend.Models;

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
                return Ok(user);
            }

        }
    }
}
