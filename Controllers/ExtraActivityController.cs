using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            return Ok(new ResponseResult<object>(extraActivities));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var extraActivity = _context.ExtraActivities.Find(id);
            if (extraActivity == null)
                return NotFound(new ResponseResult<object>(new[] { "Extra activity not found." }));

            return Ok(new ResponseResult<object>(extraActivity));
        }

        [HttpPost]
        public IActionResult Create(IEnumerable<ExtraActivity> extraActivities)
        {
            if (extraActivities == null || !extraActivities.Any())
                return BadRequest(new ResponseResult<object>(new[] { "ExtraActivities cannot be null or empty." }));

            _context.ExtraActivities.AddRange(extraActivities);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>(extraActivities));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ExtraActivity extraActivity)
        {
            if (extraActivity == null || id != extraActivity.Id)
                return BadRequest(new ResponseResult<object>(new[] { "Invalid request or ID mismatch." }));

            _context.ExtraActivities.Update(extraActivity);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>(extraActivity));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var extraActivity = _context.ExtraActivities.Find(id);
            if (extraActivity == null)
                return NotFound(new ResponseResult<object>(new[] { "Extra activity not found." }));

            _context.ExtraActivities.Remove(extraActivity);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>("Extra activity deleted successfully."));
        }
    }
}
