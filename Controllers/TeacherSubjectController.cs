using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            return Ok(new ResponseResult<object>(teacherSubjects));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var teacherSubject = _context.TeacherSubjects.Find(id);
            if (teacherSubject == null)
                return NotFound(new ResponseResult<object>(new[] { $"TeacherSubject with ID {id} not found." }));

            return Ok(new ResponseResult<object>(teacherSubject));
        }

        [HttpPost]
        public IActionResult Create(IEnumerable<CreateTeacherSubjectRequest> requests)
        {
            if (requests == null || !requests.Any())
                return BadRequest(new ResponseResult<object>(new[] { "TeacherSubjects cannot be null or empty." }));

            foreach (var request in requests)
            {
                var teacherSubjectEntity = new TeacherSubject()
                {
                    TeacherId = request.TeacherId,
                    SubjectId = request.SubjectId,
                    ClassId = request.ClassId
                };

                if (!_context.Teachers.Any(t => t.Id == teacherSubjectEntity.TeacherId))
                    return NotFound(new ResponseResult<object>(new[] { $"Teacher with ID {teacherSubjectEntity.TeacherId} not found." }));

                if (!_context.Subjects.Any(s => s.Id == teacherSubjectEntity.SubjectId))
                    return NotFound(new ResponseResult<object>(new[] { $"Subject with ID {teacherSubjectEntity.SubjectId} not found." }));

                _context.TeacherSubjects.Add(teacherSubjectEntity);
            }

            _context.SaveChanges();
            return Ok(new ResponseResult<object>("Successfully created."));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, TeacherSubject teacherSubject)
        {
            if (teacherSubject == null)
                return BadRequest(new ResponseResult<object>(new[] { "TeacherSubject cannot be null." }));

            if (id != teacherSubject.Id)
                return BadRequest(new ResponseResult<object>(new[] { "ID mismatch." }));

            if (!_context.Teachers.Any(t => t.Id == teacherSubject.TeacherId))
                return NotFound(new ResponseResult<object>(new[] { $"Teacher with ID {teacherSubject.TeacherId} not found." }));

            if (!_context.Subjects.Any(s => s.Id == teacherSubject.SubjectId))
                return NotFound(new ResponseResult<object>(new[] { $"Subject with ID {teacherSubject.SubjectId} not found." }));

            _context.TeacherSubjects.Update(teacherSubject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var teacherSubject = _context.TeacherSubjects.Find(id);
            if (teacherSubject == null)
                return NotFound(new ResponseResult<object>(new[] { $"TeacherSubject with ID {id} not found." }));

            _context.TeacherSubjects.Remove(teacherSubject);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
