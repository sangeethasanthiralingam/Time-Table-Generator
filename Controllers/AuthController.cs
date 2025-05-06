using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Time_Table_Generator.Helpers;
using Time_Table_Generator.Models;
using Microsoft.AspNetCore.Authorization; 

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [AllowAnonymous]  
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public ActionResult<User> Register([FromBody] RegisterRequest request)
        {
            if (request == null || request.Displayname == null || request.Password == null)
            {
                return BadRequest("Missing required fields.");
            }

            var newUser = new User
            {
                FirstName = request.FirstName ?? "",
                LastName = request.LastName ?? "",
                Displayname = request.Displayname,
                Phone = request.Phone,
                Address = request.Address,
                Email = request.Email,
                Password = PasswordHelper.HashPassword(request.Password),
                Role = request.Role ?? UserRole.User,
                UserType = request.UserType ?? UserType.Student,
                Status = UserStatus.Active,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Save user to database here
            // dbContext.Users.Add(newUser);
            // dbContext.SaveChanges();

            return Ok(newUser);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required.");
            }

            // Simulated user lookup
            var user = new User
            {
                Email = request.Email,
                Password = PasswordHelper.HashPassword(request.Password),
                Displayname = "Test User",
                Phone = "0000000000", // placeholder
                Address = "N/A",      // placeholder
                UserType = UserType.Student,
                Role = UserRole.User
            };

            var secret = _configuration.GetValue<string>("Jwt:Key");

            if (string.IsNullOrWhiteSpace(secret))
            {
                return StatusCode(500, "JWT secret key is missing.");
            }

            var token = JwtHelper.GenerateToken(user, secret);

            return Ok(new { token });
        }
    }
}
