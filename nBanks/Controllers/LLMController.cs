using Microsoft.AspNetCore.Mvc;
using Infrastructure.OpenAI;

[ApiController]
[Route("api/[controller]")]
public class LLMController : ControllerBase
{
    private readonly OpenAIService _openAI;

    public LLMController(OpenAIService openAI)
    {
        _openAI = openAI;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] string question)
    {
        var reply = await _openAI.AskChatAsync(question);
        return Ok(new { reply });
    }
}
