using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users.ToList();
            return Ok(new ResponseResult<object>(users));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound(new ResponseResult<object>(new[] { $"User with ID {id} not found." }));

            return Ok(new ResponseResult<object>(user));
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (user == null)
                return BadRequest(new ResponseResult<object>(new[] { "User cannot be null." }));

            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new ResponseResult<object>(user));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, User user)
        {
            if (user == null)
                return BadRequest(new ResponseResult<object>(new[] { "User cannot be null." }));

            if (id != user.Id)
                return BadRequest(new ResponseResult<object>(new[] { "ID mismatch." }));

            _context.Users.Update(user);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound(new ResponseResult<object>(new[] { $"User with ID {id} not found." }));

            _context.Users.Remove(user);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
