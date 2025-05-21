using Domain.Models.Documents;
using Infrastructure.OpenAI;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using nBanks.Application;

namespace nBanks.Application.Documents
{
    public class DocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public DocumentService(){
            
        }

        public async Task<DocumentDTO?> GetDocumentByNameAsync(string name)
        {
            var document = await _documentRepository.GetDocumentByNameAsync(name);
            return document == null ? null : DocumentMapper.ToDTO(document);
        }

       public async Task<List<DocumentDTO>> GetDocumentsByIdsAsync(List<string> ids)
        {
            var documents = await _documentRepository.GetDocumentsByIdsAsync(ids);
            return documents.Select(DocumentMapper.ToDTO).ToList();
        }

        public async Task<List<DocumentDTO>> GetDocumentsByUserIdAsync(string userId)
        {
            var documents = await _documentRepository.GetDocumentByUserIdAsync(userId);
            return documents.Select(DocumentMapper.ToDTO).ToList();
        }

        public async Task<DocumentDTO?> GetDocumentByIdAsync(string id)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(id);
            return document == null ? null : DocumentMapper.ToDTO(document);
        }


        public async Task<DocumentDTO> AddDocumentAsync(DocumentDTO documentDTO)
        {
            var document = DocumentMapper.ToDomain(documentDTO);

            if (document == null)
            {
                throw new ArgumentNullException(nameof(document), "Document cannot be null.");
            }

            try
            {
                await _documentRepository.AddDocumentAsync(document);
                return DocumentMapper.ToDTO(document);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding document to the database.", ex);
            }
        }


        public async Task<DocumentDTO> UploadAndProcessDocumentAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.", nameof(file));

            string rawContent;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Reset the stream for reading PDF/text
            memoryStream.Position = 0;

            if (file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                using var pdf = PdfDocument.Open(memoryStream);
                var builder = new StringBuilder();

                foreach (Page page in pdf.GetPages())
                {
                    builder.AppendLine(page.Text);
                }

                rawContent = builder.ToString();
            }
            else
            {
                memoryStream.Position = 0;
                using var reader = new StreamReader(memoryStream);
                rawContent = await reader.ReadToEndAsync();
            }

            var documentDto = new DocumentDTO(
                userId: userId,
                fileName: file.FileName,
                content: rawContent,
                fileData: fileBytes
            );

            return await AddDocumentAsync(documentDto);
        }



        public async Task<DocumentDTO?> DeleteDocumentAsync(string name)
        {
            var document = await _documentRepository.GetDocumentByNameAsync(name);
            if (document == null)
            {
                throw new InvalidOperationException("Document not found.");
            }
            try
            {
                await _documentRepository.DeleteDocumentAsync(document.Id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error deleting document from the database.", ex);
            }
            return DocumentMapper.ToDTO(document);
        }

        public async Task<List<DocumentDTO>?> GetDocumentByNameAndUserAsync(string name, string userId)
        {
            Console.WriteLine($"Name: {name}, UserId: {userId}");
            var documents = await _documentRepository.GetDocumentByNameAndUserAsync(name, userId);
            return documents == null ? null : documents.Select(DocumentMapper.ToDTO).ToList();
        }
    }
}
