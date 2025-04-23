namespace nBanks.Application.ChatHistories
{
    public class ChatHistoryDTO
    {
        public string? Id { get; set; }
        public string UserId { get; set; }
        public List<string> Questions { get; set; } = new();
        public List<string> Answers { get; set; } = new();

        public ChatHistoryDTO(string userId, List<string>? questions = null, List<string>? answers = null, string? id = null)
        {
            Id = id;
            UserId = userId;
            Questions = questions ?? new List<string>();
            Answers = answers ?? new List<string>();
        }
    }
}
