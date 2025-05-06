using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;
using Time_Table_Generator.Helpers;
using System.Collections.Generic;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [AllowAnonymous]  // Allow anonymous access for login and register endpoints
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        // Constructor for dependency injection
        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // Register method
        [HttpPost("register")]
        public ActionResult<User> Register([FromBody] RegisterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Displayname) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new ResponseResult<object>(new[] { "Missing required fields." }));
            }

            var newUser = new User
            {
                FirstName = request.FirstName ?? "",
                LastName = request.LastName ?? "",
                Displayname = request.Displayname,
                Phone = request.Phone,
                Address = request.Address,
                Email = request.Email,
                Password = Helpers.PasswordHelper.HashPassword(request.Password),
                Role = request.Role ?? UserRole.User,
                UserType = request.UserType ?? UserType.Student,
                Status = UserStatus.Active,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Save user to database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Create teacher or student record based on user role
            if (newUser.UserType == UserType.Teacher)
            {
                var teacher = new Teacher
                {
                    UserId = newUser.Id
                };
                _context.Teachers.Add(teacher);
                _context.SaveChanges();
            }
            else if (newUser.UserType == UserType.Student)
            {
                var student = new Student
                {
                    UserId = newUser.Id,
                    // Set default values for required fields
                    RollNumber = request.RollNumber ?? string.Empty,
                    RegistrationNumber = request.RegistrationNumber ?? string.Empty,
                    // BatchId will need to be provided or set to a default value if required
                    BatchId = request.BatchId ?? 0 // Assuming 0 is an acceptable default
                };
                _context.Students.Add(student);
                _context.SaveChanges();
            }

            // Respond with the newly created user
            var response = new ResponseResult<object>(newUser);
            return Ok(response);
        }

        // Login method
        [HttpPost("login")]
        public ActionResult<ResponseResult<object>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new ResponseResult<object>(new[] { "Email and password are required." }));
            }

            // Check if the user exists in the database
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null || !Helpers.PasswordHelper.VerifyPassword(request.Password, user.Password))
            {
                return Unauthorized(new ResponseResult<object>(new[] { "Invalid email or password." }));
            }

            // Get JWT secret from configuration
            var secret = _configuration.GetValue<string>("Jwt:Key");
            if (string.IsNullOrWhiteSpace(secret))
            {
                return StatusCode(500, new ResponseResult<object>(new[] { "JWT secret key is missing." }));
            }

            // Generate JWT token
            var token = JwtHelper.GenerateToken(user ?? throw new ArgumentNullException(nameof(user)), secret);

            // Create success response with token and user info
            var loginResult = new
            {
                Token = token,
                Displayname = user.Displayname,
                Email = user.Email,
                Role = user.Role.ToString(),
                UserType = user.UserType.ToString()
            };

            return Ok(new ResponseResult<object>(loginResult));
        }
    }
}