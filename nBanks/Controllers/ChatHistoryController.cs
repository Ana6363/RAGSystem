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
            if (res == null)
            {
                return NotFound(new { message = "Chat history not found." });
            }
            return Ok(res);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync(ChatHistoryDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Id) ||
                dto.Questions == null || dto.Questions.Count == 0 ||
                dto.Answers == null || dto.Answers.Count == 0)
            {
                return BadRequest(new { message = "Invalid update payload: must include at least one question and answer." });
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
                return BadRequest(ex.Message);
            }
        }


    }
}
