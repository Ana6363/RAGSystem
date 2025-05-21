using Domain.Models.ChatHistories;

namespace nBanks.Application.ChatHistories
{
    public class ChatHistoryDTO
    {
        public string? Id { get; set; }
        public string UserId { get; set; }
        public List<string> FileIds { get; set; } = new();
        public List<ChatMessage> Messages { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Title { get; set; } = string.Empty;

        public ChatHistoryDTO() { }

        public ChatHistoryDTO(string userId, List<string>? fileIds = null, List<ChatMessage>? messages = null, string? id = null, string? title = null)
        {
            Id = id;
            UserId = userId;
            FileIds = fileIds ?? new List<string>();
            Messages = messages ?? new List<ChatMessage>();
            CreatedAt = DateTime.UtcNow;
            Title = title ?? string.Empty;
        }
    }
}
