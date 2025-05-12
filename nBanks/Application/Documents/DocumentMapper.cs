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

            return new DocumentDTO(
                userId: document.UserId,
                fileName: document.FileName.ToString(),
                content: document.content.ToString(),
                id: document.Id,
                fileData: document.FileData
            );
        }

        public static Document ToDomain(DocumentDTO documentDTO)
        {
            if (documentDTO == null)
            {
                throw new ArgumentNullException(nameof(documentDTO), "DocumentDTO cannot be null.");
            }

            var fileName = new FileName(documentDTO.FileName);
            var content = new Content(documentDTO.Content);
            var document = new Document(documentDTO.UserId, fileName, content)
            {
                Id = documentDTO.Id ?? ObjectId.GenerateNewId().ToString(),
                FileData = documentDTO.FileData
            };

            return document;
        }

    }
}
