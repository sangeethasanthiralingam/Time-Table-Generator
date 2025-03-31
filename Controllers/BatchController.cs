using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BatchController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var batches = _context.Batches.ToList();
            return Ok(batches);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var batch = _context.Batches.Find(id);
            if (batch == null) return NotFound();
            return Ok(batch);
        }

        [HttpPost]
        public IActionResult Create(CreateBatchRequest batch)
        {
            if (batch == null) return BadRequest("Batch cannot be null.");

            var newBatch = new Batch()
            {
                Name = batch.Name,
                ClassId = batch.ClassId,
            };
            _context.Batches.Add(newBatch);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = newBatch.Id }, batch);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Batch batch)
        {
            if (batch == null) return BadRequest("Batch cannot be null.");
            if (id != batch.Id) return BadRequest();
            _context.Batches.Update(batch);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var batch = _context.Batches.Find(id);
            if (batch == null) return NotFound();
            _context.Batches.Remove(batch);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
