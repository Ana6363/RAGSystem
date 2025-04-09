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

        public Question? Question { get; set; }
        public Answer? Answer { get; set; }

        public ChatHistory() { }

        public ChatHistory(string userId, Question question, Answer answer)
        {
            UserId = userId;
            if (question == null)
                throw new ArgumentNullException(nameof(question), "Question cannot be null.");
            Question = question;
            if (answer == null)
                throw new ArgumentNullException(nameof(answer), "Answer cannot be null.");
            Answer = answer;
        }
    }
}
