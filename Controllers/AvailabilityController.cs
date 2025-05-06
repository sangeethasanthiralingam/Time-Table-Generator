using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Microsoft.AspNetCore.Authorization;

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
            return Ok(availabilities);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var availability = _context.Availabilities.Find(id);
            if (availability == null) return NotFound();
            return Ok(availability);
        }

        [HttpPost]
        public IActionResult Create(Availability availability)
        {
            if (availability == null) return BadRequest("Availability cannot be null.");
            _context.Availabilities.Add(availability);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = availability.Id }, availability);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Availability availability)
        {
            if (availability == null) return BadRequest("Availability cannot be null.");
            if (id != availability.Id) return BadRequest();
            _context.Availabilities.Update(availability);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var availability = _context.Availabilities.Find(id);
            if (availability == null) return NotFound();
            _context.Availabilities.Remove(availability);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
