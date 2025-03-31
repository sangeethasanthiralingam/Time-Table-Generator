using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeacherController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var teachers = _context.Teachers.ToList();
            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var teacher = _context.Teachers.Find(id);
            if (teacher == null) return NotFound();
            return Ok(teacher);
        }

        [HttpPost]
        public IActionResult Create(CreateTeacherRequest request)
        {
            if (request == null) return BadRequest("Teacher cannot be null.");

            var teacherEntity = new Teacher()
            {
                UserId = request.UserId,
            };
            _context.Teachers.Add(teacherEntity);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = teacherEntity.Id }, teacherEntity);
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, Teacher teacher)
        {
            if (teacher == null) return BadRequest("Teacher cannot be null.");
            if (id != teacher.Id) return BadRequest();
            _context.Teachers.Update(teacher);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var teacher = _context.Teachers.Find(id);
            if (teacher == null) return NotFound();
            _context.Teachers.Remove(teacher);
            _context.SaveChanges();
            return NoContent();
        }

        /*[HttpGet("classes-and-subjects/{id?}")]
        public IActionResult GetClassesAndSubjects(int? id)
        {
            if (id.HasValue)
            {
                var teacher = _context.Teachers
                    .Where(t => t.Id == id.Value)
                    .Select(t => new 
                    {
                        t.Id,
                        t.Name,
                        Classes = t.Classes.Select(c => c.Name),
                        Subjects = t.Subjects.Select(s => s.Name)
                    })
                    .FirstOrDefault();

                if (teacher == null) return NotFound();
                return Ok(teacher);
            }
            else
            {
                var teachers = _context.Teachers
                    .Select(t => new 
                    {
                        t.Id,
                        t.Name,
                        Classes = t.Classes.Select(c => c.Name),
                        Subjects = t.Subjects.Select(s => s.Name)
                    })
                    .ToList();

                return Ok(teachers);
            }
        }*/
    }
}
