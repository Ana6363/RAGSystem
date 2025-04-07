using System;
using MongoDB.Bson;
using Domain.Models;


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
            var document = new Document(documentDTO.Content, documentDTO.fileName);
            document.Id = documentDTO.Id ?? ObjectId.GenerateNewId().ToString();
            return document;
        }

    }
}
