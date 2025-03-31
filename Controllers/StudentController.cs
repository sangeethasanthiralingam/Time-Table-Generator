using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var students = _context.Students.ToList();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpPost]
        public IActionResult Create(CreateStudentRequest request)
        {
            if (request == null) return BadRequest("Student cannot be null.");

            var studentEntity = new Student()
            {
                UserId = request.UserId,
                BatchId = request.BatchId,
                RollNumber = request.RollNumber,
                RegistrationNumber = request.RegistrationNumber,
            };
            _context.Students.Add(studentEntity);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = studentEntity.Id }, studentEntity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Student student)
        {
            if (student == null) return BadRequest("Student cannot be null.");
            if (id != student.Id) return BadRequest();
            _context.Students.Update(student);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) return NotFound();
            _context.Students.Remove(student);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
