using Infrastructure.Mongo;
using Domain.Models.Users;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;


namespace nBanks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public UsersController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create()
        {
            var user = new User();
            await _context.Users.InsertOneAsync(user);
            return Ok(new { id = user.UserId });

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _context.Users.Find(u => u.UserId.Value == id).FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }
    }
}
