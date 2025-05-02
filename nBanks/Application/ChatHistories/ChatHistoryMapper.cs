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

            return new ChatHistoryDTO
            {
                Id = chatHistory.Id,
                UserId = chatHistory.UserId,
                FileIds = chatHistory.FileIds ?? new List<string>(),
                Messages = chatHistory.Messages ?? new List<ChatMessage>(),
                CreatedAt = chatHistory.CreatedAt
            };
        }

        public static ChatHistory MapToDomain(ChatHistoryDTO chatHistoryDTO)
        {
            if (chatHistoryDTO == null)
                throw new ArgumentNullException(nameof(chatHistoryDTO), "Chat history DTO cannot be null.");

            var chatHistory = new ChatHistory
            {
                Id = chatHistoryDTO.Id ?? ObjectId.GenerateNewId().ToString(),
                UserId = chatHistoryDTO.UserId,
                FileIds = chatHistoryDTO.FileIds ?? new List<string>(),
                Messages = chatHistoryDTO.Messages ?? new List<ChatMessage>(),
                CreatedAt = chatHistoryDTO.CreatedAt
            };

            return chatHistory;
        }
    }
}