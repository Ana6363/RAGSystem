using Domain.Models.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models
{
    public class Document
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public Content content { get; set; }
        public FileName fileName { get; set; }

        public Document() { }

        public Document(string userId, Content contentValue)
        {
            if (string.IsNullOrWhiteSpace(contentValue.ToString()))
                throw new ArgumentException("Content cannot be null or empty.", nameof(contentValue));
            content = contentValue;
            UserId = userId;
        }
    }
}
