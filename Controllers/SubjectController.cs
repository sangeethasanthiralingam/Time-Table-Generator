using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var subjects = _context.Subjects.ToList();
            return Ok(new ResponseResult<object>(subjects));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var subject = _context.Subjects.Find(id);
            if (subject == null)
                return NotFound(new ResponseResult<object>(new[] { "Subject not found." }));

            return Ok(new ResponseResult<object>(subject));
        }

        [HttpPost]
        public IActionResult Create(CreateSubjectRequest request)
        {
            if (request == null)
                return BadRequest(new ResponseResult<object>(new[] { "Subject cannot be null." }));

            var subjectEntity = new Subject()
            {
                Name = request.Name,
            };

            _context.Subjects.Add(subjectEntity);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = subjectEntity.Id }, new ResponseResult<object>(subjectEntity));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Subject subject)
        {
            if (subject == null || id != subject.Id)
                return BadRequest(new ResponseResult<object>(new[] { "Invalid subject data or ID mismatch." }));

            _context.Subjects.Update(subject);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>(subject));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var subject = _context.Subjects.Find(id);
            if (subject == null)
                return NotFound(new ResponseResult<object>(new[] { "Subject not found." }));

            _context.Subjects.Remove(subject);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>("Subject deleted successfully."));
        }
    }
}
