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

        public async Task<DocumentDTO?> GetDocumentByNameAsync(string id)
        {
            var document = await _documentRepository.GetDocumentByNameAsync(id);
            return document == null ? null : DocumentMapper.ToDTO(document);
        }

        public async Task<List<DocumentDTO>> GetDocumentsByUserIdAsync(string userId)
        {
            var documents = await _documentRepository.GetDocumentByUserIdAsync(userId);
            return documents.Select(DocumentMapper.ToDTO).ToList();
        }


        public async Task<DocumentDTO> AddDocumentAsync(DocumentDTO documentDTO)
        {
            var dto = new DocumentDTO(documentDTO.Content, documentDTO.FileName, documentDTO.UserId);
            var document = DocumentMapper.ToDomain(documentDTO);

            var userDocuments = await _documentRepository.GetDocumentByUserIdAsync(document.UserId);

            if (userDocuments != null && userDocuments.Any(d => d.fileName.ToString() == document.fileName.ToString()))
            {
                throw new InvalidOperationException("Document with this name already exists for this user.");
            }


            if (document == null)
            {
                throw new ArgumentNullException(nameof(document), "Document cannot be null.");
            }

            try
            {
                await _documentRepository.AddDocumentAsync(document);

                // Call the new method to run the embed_and_store script with the file ID
                var fileId = document.Id;
                string relativePath = "../RAG-Server/embed_and_store.py";
                string pythonScriptPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativePath));

                await PythonRunner.EmbedAndStoreDocumentAsync(pythonScriptPath, fileId);

                return DocumentMapper.ToDTO(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                Console.WriteLine("INNER EXCEPTION: " + ex.InnerException?.Message);
                throw new InvalidOperationException("Error adding document to the database.", ex);
            }

        }



        public async Task<DocumentDTO> UploadAndProcessDocumentAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.", nameof(file));

            string rawContent;

            if (file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                using var pdfStream = file.OpenReadStream();
                using var pdf = PdfDocument.Open(pdfStream);
                var builder = new StringBuilder();

                foreach (Page page in pdf.GetPages())
                {
                    builder.AppendLine(page.Text);
                }

                rawContent = builder.ToString();
            }
            else
            {
                using var reader = new StreamReader(file.OpenReadStream());
                rawContent = await reader.ReadToEndAsync();
            }


            var documentDto = new DocumentDTO(
                content: rawContent,  // Save raw extracted text instead
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
