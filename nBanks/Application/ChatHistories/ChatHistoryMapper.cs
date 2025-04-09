using Domain.Models.ChatHistories;
using MongoDB.Bson;

namespace nBanks.Application.ChatHistories
{
    public class ChatHistoryMapper
    {
        public static ChatHistoryDTO MapToDTO(ChatHistory chatHistory)
        {
            if (chatHistory == null)
                throw new ArgumentNullException(nameof(chatHistory), "Chat history cannot be null.");
            return new ChatHistoryDTO(
                chatHistory.UserId,
                chatHistory.Question.ToString(),
                chatHistory.Answer.ToString()
                );
        }
        public static ChatHistory MapToDomain(ChatHistoryDTO chatHistoryDTO)
        {
            if (chatHistoryDTO == null)
                throw new ArgumentNullException(nameof(chatHistoryDTO), "Chat history DTO cannot be null.");

            var question = new Question(chatHistoryDTO.Question);
            var answer = new Answer(chatHistoryDTO.Answer);
            var userId = chatHistoryDTO.UserId;
            var chatHistory = new ChatHistory(userId, question, answer);
            chatHistory.Id = chatHistoryDTO.Id ?? ObjectId.GenerateNewId().ToString();
            
            return chatHistory;
        }
    }
}
