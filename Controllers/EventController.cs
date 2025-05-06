using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var events = _context.Events.ToList();
            return Ok(new ResponseResult<object>(events));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var eventModel = _context.Events.Find(id);
            if (eventModel == null)
                return NotFound(new ResponseResult<object>(new[] { "Event not found." }));

            return Ok(new ResponseResult<object>(eventModel));
        }

        [HttpPost]
        public IActionResult Create(IEnumerable<EventModel> eventModels)
        {
            if (eventModels == null || !eventModels.Any())
                return BadRequest(new ResponseResult<object>(new[] { "Events cannot be null or empty." }));

            _context.Events.AddRange(eventModels);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>(eventModels));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, EventModel eventModel)
        {
            if (eventModel == null || id != eventModel.Id)
                return BadRequest(new ResponseResult<object>(new[] { "Invalid event or ID mismatch." }));

            _context.Events.Update(eventModel);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>(eventModel));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var eventModel = _context.Events.Find(id);
            if (eventModel == null)
                return NotFound(new ResponseResult<object>(new[] { "Event not found." }));

            _context.Events.Remove(eventModel);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>("Event deleted successfully."));
        }
    }
}
