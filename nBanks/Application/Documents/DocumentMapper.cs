using System;
using MongoDB.Bson;
using Domain.Models;
using Domain.Models.Documents;


namespace nBanks.Application.Documents
{
    public class DocumentMapper
    {

        public static DocumentDTO ToDTO(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document), "Document cannot be null.");
            }
            return new DocumentDTO(document.Id, document.fileName.ToString(), document.content.ToString());
        }

        public static Document ToDomain(DocumentDTO documentDTO)
        {
            if (documentDTO == null)
            {
                throw new ArgumentNullException(nameof(documentDTO), "DocumentDTO cannot be null.");
            }
            Console.WriteLine("📦 Mapping DocumentDTO to Domain:");
            Console.WriteLine($"- DTO.FileName: {documentDTO.FileName}");
            Console.WriteLine($"- DTO.Content: {documentDTO.Content}");
            Console.WriteLine($"- DTO.UserId: {documentDTO.UserId}");
            var fileName = new FileName(documentDTO.FileName);
            var content = new Content(documentDTO.Content);
            var document = new Document(documentDTO.UserId,fileName, content);
            document.Id = documentDTO.Id ?? ObjectId.GenerateNewId().ToString();

            Console.WriteLine("✅ Document object created");
            Console.WriteLine($"- fileName = {document.fileName?.ToString() ?? "NULL"}");
            Console.WriteLine($"- content = {document.content?.ToString() ?? "NULL"}");
            return document;
        }

    }
}
