using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            return Ok(new ResponseResult<object>(holidays));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var holiday = _context.Holidays.Find(id);
            if (holiday == null)
                return NotFound(new ResponseResult<object>(new[] { "Holiday not found." }));

            return Ok(new ResponseResult<object>(holiday));
        }

        [HttpPost]
        public IActionResult Create(Holiday holiday)
        {
            if (holiday == null)
                return BadRequest(new ResponseResult<object>(new[] { "Holiday cannot be null." }));

            _context.Holidays.Add(holiday);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = holiday.Id }, new ResponseResult<object>(holiday));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Holiday holiday)
        {
            if (holiday == null || id != holiday.Id)
                return BadRequest(new ResponseResult<object>(new[] { "Invalid request or ID mismatch." }));

            _context.Holidays.Update(holiday);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>(holiday));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var holiday = _context.Holidays.Find(id);
            if (holiday == null)
                return NotFound(new ResponseResult<object>(new[] { "Holiday not found." }));

            _context.Holidays.Remove(holiday);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>("Holiday deleted successfully."));
        }
    }
}
