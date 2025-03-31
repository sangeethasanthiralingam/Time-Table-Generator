using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtraActivityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExtraActivityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var extraActivities = _context.ExtraActivities.ToList();
            return Ok(extraActivities);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var extraActivity = _context.ExtraActivities.Find(id);
            if (extraActivity == null) return NotFound();
            return Ok(extraActivity);
        }

        [HttpPost]
        public IActionResult Create(IEnumerable<ExtraActivity> extraActivities)
        {
            if (extraActivities == null || !extraActivities.Any())
                return BadRequest("ExtraActivities cannot be null or empty.");

            _context.ExtraActivities.AddRange(extraActivities);
            _context.SaveChanges();
            return Ok(extraActivities);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ExtraActivity extraActivity)
        {
            if (id != extraActivity.Id) return BadRequest();
            _context.ExtraActivities.Update(extraActivity);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var extraActivity = _context.ExtraActivities.Find(id);
            if (extraActivity == null) return NotFound();
            _context.ExtraActivities.Remove(extraActivity);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
