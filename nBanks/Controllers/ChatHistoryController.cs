using Domain.Models.ChatHistories;
using Infrastructure.Mongo;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using nBanks.Application.ChatHistories;

namespace nBanks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatHistoryController : ControllerBase
    {
        private readonly ChatHistoryService _chatHistoryService;

        public ChatHistoryController(ChatHistoryService context)
        {
            _chatHistoryService = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(ChatHistoryDTO dto)
        {
            var res = await _chatHistoryService.AddChatHistory(dto);
            if (res == null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpGet("getByUser")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var res = await _chatHistoryService.GetAllChatHistories(userId);
            if (res == null)
            {
                return NotFound(new { message = "Chat history not found." });
            }
            return Ok(res);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync(ChatHistoryDTO dto)
        {
            try
            {
                await _chatHistoryService.UpdateChatHistory(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                await _chatHistoryService.DeleteChatHistory(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
