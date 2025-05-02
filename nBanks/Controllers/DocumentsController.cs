using Domain.Models;
using Infrastructure.Mongo;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using nBanks.Application.Documents;
using Infrastructure.OpenAI;

namespace nBanks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : Controller
    {
        private readonly DocumentService _documentService;
        private readonly OpenAIService _openAiService;


        public DocumentsController(DocumentService context, OpenAIService openAiService)
        {
            _documentService = context;
            _openAiService = openAiService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentByNameAsync(string id)
        {
            var document = await _documentService.GetDocumentByNameAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return Ok(document);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync([FromForm] DocumentUploadRequest request)
        {
            try
            {
                if (request.File == null || !request.File.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Only PDF files are allowed.");
                }

                var result = await _documentService.UploadAndProcessDocumentAsync(
                    request.File, request.UserId);

                var fileId = result.Id; // This must be the MongoDB _id as string

                using (var httpClient = new HttpClient())
                {
                    var ragUrl = $"http://localhost:8000/embed?file_id={fileId}";

                    var response = await httpClient.PostAsync(ragUrl, null);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to notify RAG server. Status: {response.StatusCode}");
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteDocumentAsync(string name)
        {
            try
            {
                var document = await _documentService.DeleteDocumentAsync(name);
                if (document == null)
                {
                    return NotFound();
                }
                return Ok(document);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetDocumentsByUserIdAsync(string userId)
        {
            try
            {
                var documents = await _documentService.GetDocumentsByUserIdAsync(userId);
                if (documents == null || documents.Count == 0)
                {
                    return NotFound(new { message = "No documents found for this user." });
                }
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
