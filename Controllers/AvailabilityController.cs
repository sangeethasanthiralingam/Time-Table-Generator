using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Microsoft.AspNetCore.Authorization;
using Time_Table_Generator.Models.Request;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AvailabilityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AvailabilityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var availabilities = _context.Availabilities.ToList();
            var response = new ResponseResult<object>(availabilities);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var availability = _context.Availabilities.Find(id);
            if (availability == null)
                return NotFound(new ResponseResult<object>(new[] { "Availability not found." }));

            var response = new ResponseResult<object>(availability);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Availability availability)
        {
            if (availability == null)
                return BadRequest(new ResponseResult<object>(new[] { "Availability cannot be null." }));

            _context.Availabilities.Add(availability);
            _context.SaveChanges();

            var response = new ResponseResult<object>(availability);
            return CreatedAtAction(nameof(GetById), new { id = availability.Id }, response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Availability availability)
        {
            if (availability == null)
                return BadRequest(new ResponseResult<object>(new[] { "Availability cannot be null." }));

            if (id != availability.Id)
                return BadRequest(new ResponseResult<object>(new[] { "ID mismatch." }));

            _context.Availabilities.Update(availability);
            _context.SaveChanges();

            var response = new ResponseResult<object>(availability);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var availability = _context.Availabilities.Find(id);
            if (availability == null)
                return NotFound(new ResponseResult<object>(new[] { "Availability not found." }));

            _context.Availabilities.Remove(availability);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>("Deleted successfully."));
        }
    }
}
