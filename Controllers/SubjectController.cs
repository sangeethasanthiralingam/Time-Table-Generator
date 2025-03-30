using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var subject = _context.Subjects.Find(id);
            if (subject == null) return NotFound();
            return Ok(subject);
        }

        [HttpPost]
        public IActionResult Create(Subject subject)
        {
            if (subject == null) return BadRequest("Subject cannot be null.");
            _context.Subjects.Add(subject);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = subject.Id }, subject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Subject subject)
        {
            if (subject == null) return BadRequest("Subject cannot be null.");
            if (id != subject.Id) return BadRequest();
            _context.Subjects.Update(subject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var subject = _context.Subjects.Find(id);
            if (subject == null) return NotFound();
            _context.Subjects.Remove(subject);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
