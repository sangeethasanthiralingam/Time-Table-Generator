using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectHourController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectHourController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var subjectHours = _context.SubjectHours.ToList();
            return Ok(subjectHours);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var subjectHour = _context.SubjectHours.Find(id);
            if (subjectHour == null) return NotFound();
            return Ok(subjectHour);
        }

        [HttpPost]
        public IActionResult Create(IEnumerable<CreateSubjectHourRequest> requests)
        {
            if (requests == null || !requests.Any())
                return BadRequest("SubjectHours cannot be null or empty.");

            foreach (var request in requests)
            {
                var subjectHourEntity = new SubjectHour()
                {
                    SubjectId = request.SubjectId,
                    HoursInWeek = request.HoursInWeek,
                    HoursInDay = request.HoursInDay
                };

                if (!_context.Subjects.Any(s => s.Id == subjectHourEntity.SubjectId))
                    return NotFound($"Subject with ID {subjectHourEntity.SubjectId} not found.");

                _context.SubjectHours.Add(subjectHourEntity);
            }

            _context.SaveChanges();
            return Ok("Successfully created.");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, SubjectHour subjectHour)
        {
            if (id != subjectHour.Id) return BadRequest();
            _context.SubjectHours.Update(subjectHour);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var subjectHour = _context.SubjectHours.Find(id);
            if (subjectHour == null) return NotFound();
            _context.SubjectHours.Remove(subjectHour);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
