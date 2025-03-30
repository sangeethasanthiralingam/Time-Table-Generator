using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherSubjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeacherSubjectController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var teacherSubjects = _context.TeacherSubjects.ToList();
            return Ok(teacherSubjects);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var teacherSubject = _context.TeacherSubjects.Find(id);
            if (teacherSubject == null) return NotFound();
            return Ok(teacherSubject);
        }

        [HttpPost]
        public IActionResult Create(IEnumerable<TeacherSubject> teacherSubjects)
        {
            if (teacherSubjects == null || !teacherSubjects.Any()) 
                return BadRequest("TeacherSubjects cannot be null or empty.");

            foreach (var teacherSubject in teacherSubjects)
            {
                if (!_context.Teachers.Any(t => t.Id == teacherSubject.TeacherId))
                    return NotFound($"Teacher with ID {teacherSubject.TeacherId} not found.");
                if (!_context.Subjects.Any(s => s.Id == teacherSubject.SubjectId))
                    return NotFound($"Subject with ID {teacherSubject.SubjectId} not found.");

                _context.TeacherSubjects.Add(teacherSubject);
            }

            _context.SaveChanges();
            return Ok(teacherSubjects);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, TeacherSubject teacherSubject)
        {
            if (teacherSubject == null) return BadRequest("TeacherSubject cannot be null.");
            if (id != teacherSubject.Id) return BadRequest("ID mismatch.");
            if (!_context.Teachers.Any(t => t.Id == teacherSubject.TeacherId)) 
                return NotFound($"Teacher with ID {teacherSubject.TeacherId} not found.");
            if (!_context.Subjects.Any(s => s.Id == teacherSubject.SubjectId)) 
                return NotFound($"Subject with ID {teacherSubject.SubjectId} not found.");

            _context.TeacherSubjects.Update(teacherSubject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var teacherSubject = _context.TeacherSubjects.Find(id);
            if (teacherSubject == null) return NotFound($"TeacherSubject with ID {id} not found.");

            _context.TeacherSubjects.Remove(teacherSubject);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
