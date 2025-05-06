using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class ClassTeacherController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClassTeacherController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var classTeachers = _context.ClassTeachers.ToList();
            return Ok(classTeachers);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var classTeacher = _context.ClassTeachers.Find(id);
            if (classTeacher == null) return NotFound();
            return Ok(classTeacher);
        }

        [HttpPost]
        public IActionResult Create(CreateClassTeacherRequest request)
        {
            var classTeacherEntity = new ClassTeacher()
            {
                TeacherId = request.TeacherId,
                ClassId = request.ClassId,
                BatchId = request.BatchId,
            };
            _context.ClassTeachers.Add(classTeacherEntity);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = classTeacherEntity.Id }, classTeacherEntity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ClassTeacher classTeacher)
        {
            if (id != classTeacher.Id) return BadRequest();
            _context.ClassTeachers.Update(classTeacher);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var classTeacher = _context.ClassTeachers.Find(id);
            if (classTeacher == null) return NotFound();
            _context.ClassTeachers.Remove(classTeacher);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
