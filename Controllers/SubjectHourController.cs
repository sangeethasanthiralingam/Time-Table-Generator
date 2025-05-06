using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            return Ok(new ResponseResult<object>(subjectHours));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var subjectHour = _context.SubjectHours.Find(id);
            if (subjectHour == null) 
                return NotFound(new ResponseResult<object>(new[] { "SubjectHour not found." }));
            
            return Ok(new ResponseResult<object>(subjectHour));
        }

        [HttpPost]
        public IActionResult Create(IEnumerable<CreateSubjectHourRequest> requests)
        {
            if (requests == null || !requests.Any())
                return BadRequest(new ResponseResult<object>(new[] { "SubjectHours cannot be null or empty." }));

            foreach (var request in requests)
            {
                var subjectHourEntity = new SubjectHour()
                {
                    SubjectId = request.SubjectId,
                    HoursInWeek = request.HoursInWeek,
                    HoursInDay = request.HoursInDay
                };

                if (!_context.Subjects.Any(s => s.Id == subjectHourEntity.SubjectId))
                    return NotFound(new ResponseResult<object>(new[] { $"Subject with ID {subjectHourEntity.SubjectId} not found." }));

                _context.SubjectHours.Add(subjectHourEntity);
            }

            _context.SaveChanges();
            return Ok(new ResponseResult<object>("Successfully created."));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, SubjectHour subjectHour)
        {
            if (subjectHour == null) 
                return BadRequest(new ResponseResult<object>(new[] { "SubjectHour cannot be null." }));
            
            if (id != subjectHour.Id) 
                return BadRequest(new ResponseResult<object>(new[] { "ID mismatch." }));

            _context.SubjectHours.Update(subjectHour);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var subjectHour = _context.SubjectHours.Find(id);
            if (subjectHour == null) 
                return NotFound(new ResponseResult<object>(new[] { "SubjectHour not found." }));
            
            _context.SubjectHours.Remove(subjectHour);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
