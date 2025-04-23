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

        public List<Question> Questions { get; set; } = new();
        public List<Answer> Answers { get; set; } = new();

        public ChatHistory() { }

        public ChatHistory(string userId, List<Question>? questions = null, List<Answer>? answers = null)
        {
            UserId = userId;
            Questions = questions ?? new List<Question>();
            Answers = answers ?? new List<Answer>();
        }
    }
}
