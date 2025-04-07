using Domain.Models;
using Infrastructure.Mongo;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using nBanks.Application.Documents;

namespace nBanks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : Controller
    {
        private readonly DocumentService _documentService;

        public DocumentsController(DocumentService context)
        {
            _documentService = context;
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
