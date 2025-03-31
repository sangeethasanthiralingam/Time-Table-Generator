using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolidayController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HolidayController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var holidays = _context.Holidays.ToList();
            return Ok(holidays);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var holiday = _context.Holidays.Find(id);
            if (holiday == null) return NotFound();
            return Ok(holiday);
        }

        [HttpPost]
        public IActionResult Create(Holiday holiday)
        {
            _context.Holidays.Add(holiday);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = holiday.Id }, holiday);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Holiday holiday)
        {
            if (id != holiday.Id) return BadRequest();
            _context.Holidays.Update(holiday);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var holiday = _context.Holidays.Find(id);
            if (holiday == null) return NotFound();
            _context.Holidays.Remove(holiday);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
