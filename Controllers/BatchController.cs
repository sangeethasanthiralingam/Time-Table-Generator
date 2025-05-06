using Microsoft.AspNetCore.Mvc;
using Time_Table_Generator.Models;
using Time_Table_Generator.Models.Request;
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            var response = new ResponseResult<object>(batches);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var batch = _context.Batches.Find(id);
            if (batch == null)
                return NotFound(new ResponseResult<object>(new[] { "Batch not found." }));

            var response = new ResponseResult<object>(batch);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateBatchRequest batch)
        {
            if (batch == null)
                return BadRequest(new ResponseResult<object>(new[] { "Batch cannot be null." }));

            var newBatch = new Batch
            {
                Name = batch.Name,
                ClassId = batch.ClassId
            };

            _context.Batches.Add(newBatch);
            _context.SaveChanges();

            var response = new ResponseResult<object>(newBatch);
            return CreatedAtAction(nameof(GetById), new { id = newBatch.Id }, response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Batch batch)
        {
            if (batch == null)
                return BadRequest(new ResponseResult<object>(new[] { "Batch cannot be null." }));

            if (id != batch.Id)
                return BadRequest(new ResponseResult<object>(new[] { "ID mismatch." }));

            _context.Batches.Update(batch);
            _context.SaveChanges();

            var response = new ResponseResult<object>(batch);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var batch = _context.Batches.Find(id);
            if (batch == null)
                return NotFound(new ResponseResult<object>(new[] { "Batch not found." }));

            _context.Batches.Remove(batch);
            _context.SaveChanges();

            return Ok(new ResponseResult<object>("Deleted successfully."));
        }
    }
}
