using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace nBanks.Application.Documents
{
    public class DocumentUploadRequest
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }

        [FromForm(Name = "userId")]
        public string UserId { get; set; }
    }
}
