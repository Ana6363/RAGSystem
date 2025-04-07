using Domain.Models.Documents;
using Infrastructure.OpenAI;

namespace nBanks.Application.Documents
{
    public class DocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<DocumentDTO?> GetDocumentByNameAsync(string id)
        {
            var document = await _documentRepository.GetDocumentByNameAsync(id);
            return document == null ? null : DocumentMapper.ToDTO(document);
        }

        public async Task<DocumentDTO> AddDocumentAsync(DocumentDTO documentDTO)
        {
            var dto = new DocumentDTO(documentDTO.Content, documentDTO.FileName, documentDTO.UserId);
            var document = DocumentMapper.ToDomain(documentDTO);

            if (await _documentRepository.GetDocumentByNameAsync(document.fileName.ToString()) != null)
            {
                throw new InvalidOperationException("Document with this name already exists.");
            }

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


        public async Task<DocumentDTO> UploadAndProcessDocumentAsync(IFormFile file, string userId, OpenAIService openAiService)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.", nameof(file));

            string rawContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            var openAiResponse = await openAiService.AskChatAsync($"Extract relevant content from this document:\n\n{rawContent}");

            var documentDto = new DocumentDTO(
                content: openAiResponse,
                fileName: file.FileName,
                userId: userId
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
    }
}
