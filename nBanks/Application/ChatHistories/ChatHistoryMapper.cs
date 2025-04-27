using Domain.Models.ChatHistories;
using MongoDB.Bson;
using System.Linq;

namespace nBanks.Application.ChatHistories
{
    public class ChatHistoryMapper
    {
        public static ChatHistoryDTO MapToDTO(ChatHistory chatHistory)
        {
            if (chatHistory == null)
                throw new ArgumentNullException(nameof(chatHistory), "Chat history cannot be null.");

            // Map Questions and Answers to List<string>
            var questions = chatHistory.Questions?.Select(q => q.QuestionValue).ToList() ?? new List<string>();
            var answers = chatHistory.Answers?.Select(a => a.AnswerValue).ToList() ?? new List<string>();

            return new ChatHistoryDTO(
                chatHistory.UserId,
                questions,
                answers,
                chatHistory.Id
            );
        }

        public static ChatHistory MapToDomain(ChatHistoryDTO chatHistoryDTO)
        {
            if (chatHistoryDTO == null)
                throw new ArgumentNullException(nameof(chatHistoryDTO), "Chat history DTO cannot be null.");

            var questions = chatHistoryDTO.Questions?.Select(q => new Question(q)).ToList() ?? new List<Question>();
            var answers = chatHistoryDTO.Answers?.Select(a => new Answer(a)).ToList() ?? new List<Answer>();

            var userId = chatHistoryDTO.UserId;
            var chatHistory = new ChatHistory(userId, questions, answers);
            chatHistory.Id = chatHistoryDTO.Id ?? ObjectId.GenerateNewId().ToString();

            return chatHistory;
        }
    }
}