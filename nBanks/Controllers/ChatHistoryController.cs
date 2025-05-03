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
            try
            {
                var res = await _chatHistoryService.AddChatHistory(dto);
                return Ok(res); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("getByUser")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var res = await _chatHistoryService.GetAllChatHistories(userId);
            if (res == null || res.Count == 0)
            {
                return NotFound(new { message = "No chat histories found for this user." });
            }
            return Ok(res);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync(ChatHistoryDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Id) || dto.Messages == null || dto.Messages.Count == 0)
            {
                return BadRequest(new { message = "Invalid update payload: must include at least one message." });
            }

            try
            {
                await _chatHistoryService.UpdateChatHistory(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] AskQuestionDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ChatId) || string.IsNullOrWhiteSpace(dto.Question))
            {
                return BadRequest(new { message = "ChatId and Question are required." });
            }

            try
            {
                var lastMessages = await _chatHistoryService.AskQuestionAsync(dto.ChatId, dto.Question);
                return Ok(lastMessages); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
