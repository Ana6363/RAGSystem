using Domain.Models;
using Infrastructure.Mongo;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace nBanks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatHistoryController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public ChatHistoryController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ChatHistory chat)
        {
            await _context.ChatHistory.InsertOneAsync(chat);
            return Ok(new { id = chat.Id });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var history = await _context.ChatHistory
                .Find(c => c.UserId == userId)
                .ToListAsync();

            return Ok(history);
        }
    }
}
