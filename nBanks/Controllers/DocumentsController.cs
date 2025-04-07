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

        /* [HttpPost("create")]
         public async Task<IActionResult> CreateAsync()
         {

         }

         [HttpGet("user")]
         public async Task<IActionResult> GetDocumentsByUserAsync(string userId)
         {
             var documents = await _documentService.GetDocumentsByUserIdAsync(userId);
             if (documents == null)
             {
                 return NotFound();
             }
         }
        */

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
                var result = await _documentService.UploadAndProcessDocumentAsync(
                    request.File, request.UserId, _openAiService);

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
    }
}
