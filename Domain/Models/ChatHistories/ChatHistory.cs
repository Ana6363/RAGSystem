using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.ChatHistories
{
    public class ChatHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<string> FileIds { get; set; } = new();

        public List<ChatMessage> Messages { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ChatHistory() { }

        public ChatHistory(string userId, List<string>? fileIds = null)
        {
            UserId = userId;
            FileIds = fileIds ?? new List<string>();
        }
    }
}
