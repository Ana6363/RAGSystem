using Domain.Models;
using Infrastructure.Mongo;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace nBanks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public DocumentsController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody] Document doc)
        {
            await _context.Documents.InsertOneAsync(doc);
            return Ok(new { id = doc.Id });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetDocumentsByUserAsync(string userId)
        {
            var docs = await _context.Documents
                .Find(d => d.UserId == userId)
                .ToListAsync();

            return Ok(docs);
        }
    }
}
