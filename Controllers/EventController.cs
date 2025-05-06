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
            return Ok(events);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var eventModel = _context.Events.Find(id);
            if (eventModel == null) return NotFound();
            return Ok(eventModel);
        }

        [HttpPost]
        public IActionResult Create(IEnumerable<EventModel> eventModels)
        {
            if (eventModels == null || !eventModels.Any())
                return BadRequest("Events cannot be null or empty.");

            _context.Events.AddRange(eventModels);
            _context.SaveChanges();
            return Ok(eventModels);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, EventModel eventModel)
        {
            if (id != eventModel.Id) return BadRequest();
            _context.Events.Update(eventModel);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var eventModel = _context.Events.Find(id);
            if (eventModel == null) return NotFound();
            _context.Events.Remove(eventModel);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
