using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Time_Table_Generator.Models;


namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClassController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var classes = _context.Classes.ToList();
            return Ok(classes);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var classEntity = _context.Classes.Find(id);
            if (classEntity == null) return NotFound();
            return Ok(classEntity);
        }

        [HttpPost]
        public IActionResult Create(Class classEntity)
        {
            if (classEntity == null) return BadRequest("Class cannot be null.");
            _context.Classes.Add(classEntity);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = classEntity.Id }, classEntity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Class classEntity)
        {
            if (classEntity == null) return BadRequest("Class cannot be null.");
            if (id != classEntity.Id) return BadRequest();
            _context.Classes.Update(classEntity);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var classEntity = _context.Classes
                .Include(c => c.Batches)
                .FirstOrDefault(c => c.Id == id);

            if (classEntity == null) return NotFound();

            // Remove associated batches
            _context.Batches.RemoveRange(classEntity.Batches);

            _context.Classes.Remove(classEntity);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPost("{classId}/AddBatches")]
        public IActionResult AddBatchesToClass(int classId, [FromBody] List<int> batchIds)
        {
            var classEntity = _context.Classes.Include(c => c.Batches).FirstOrDefault(c => c.Id == classId);
            if (classEntity == null) return NotFound("Class not found.");

            var batches = _context.Batches.Where(b => batchIds.Contains(b.Id)).ToList();
            if (!batches.Any()) return BadRequest("No valid batches found.");

            foreach (var batch in batches)
            {
                if (!classEntity.Batches.Contains(batch))
                {
                    classEntity.Batches.Add(batch);
                }
            }

            _context.SaveChanges();
            return Ok(classEntity);
        }

        [HttpPost("{classId}/AddSubjects")]
        public IActionResult AddSubjectsToClass(int classId, [FromBody] List<int> subjectIds)
        {
            var classEntity = _context.Classes.Include(c => c.Subjects).FirstOrDefault(c => c.Id == classId);
            if (classEntity == null) return NotFound("Class not found.");

            var subjects = _context.Subjects.Where(s => subjectIds.Contains(s.Id)).ToList();
            if (!subjects.Any()) return BadRequest("No valid subjects found.");

            foreach (var subject in subjects)
            {
                if (!classEntity.Subjects.Contains(subject))
                {
                    classEntity.Subjects.Add(subject);
                }
            }

            _context.SaveChanges();
            return Ok(classEntity);
        }

        [HttpGet("{classId}/Details")]
        public IActionResult GetClassDetails(int classId)
        {
            var classEntity = _context.Classes
                .Include(c => c.Subjects)
                .Include(c => c.Batches)
                .FirstOrDefault(c => c.Id == classId);

            if (classEntity == null) return NotFound("Class not found.");

            var result = new
            {
                Class = classEntity,
                Subjects = classEntity.Subjects,
                Batches = classEntity.Batches
            };

            return Ok(result);
        }

        [HttpGet("{classId}/Students")]
        public IActionResult GetStudentsByClass(int classId, [FromQuery] int? batchId = null)
        {
            var classEntity = _context.Classes
                .Include(c => c.Students)
                .Include(c => c.Batches)
                .ThenInclude(b => b.Students)
                .FirstOrDefault(c => c.Id == classId);

            if (classEntity == null) return NotFound("Class not found.");

            if (batchId.HasValue)
            {
                var batch = classEntity.Batches.FirstOrDefault(b => b.Id == batchId.Value);
                if (batch == null) return NotFound("Batch not found in the class.");

                var studentsInBatch = batch.Students;
                return Ok(studentsInBatch);
            }

            var students = classEntity.Students;
            return Ok(students);
        }
    }
}
