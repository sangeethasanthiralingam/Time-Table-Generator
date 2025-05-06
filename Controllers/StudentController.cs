using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            return Ok(new ResponseResult<object>(students));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
                return NotFound(new ResponseResult<object>(new[] { "Student not found." }));

            return Ok(new ResponseResult<object>(student));
        }

        [HttpPost]
        public IActionResult Create(CreateStudentRequest request)
        {
            if (request == null)
                return BadRequest(new ResponseResult<object>(new[] { "Student cannot be null." }));

            var studentEntity = new Student()
            {
                UserId = request.UserId,
                BatchId = request.BatchId,
                RollNumber = request.RollNumber,
                RegistrationNumber = request.RegistrationNumber,
            };

            _context.Students.Add(studentEntity);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = studentEntity.Id }, new ResponseResult<object>(studentEntity));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Student student)
        {
            if (student == null || id != student.Id)
                return BadRequest(new ResponseResult<object>(new[] { "Invalid student data or ID mismatch." }));

            _context.Students.Update(student);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>(student));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
                return NotFound(new ResponseResult<object>(new[] { "Student not found." }));

            _context.Students.Remove(student);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>("Student deleted successfully."));
        }
    }
}
