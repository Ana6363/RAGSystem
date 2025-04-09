namespace nBanks.Application.ChatHistories
{
    public class ChatHistoryDTO
    {
        public string? Id { get; set; }
        public string UserId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public ChatHistoryDTO(string userId, string question, string answer, string? id = null)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentNullException(nameof(question), "Question cannot be empty.");
            if (string.IsNullOrWhiteSpace(answer))
                throw new ArgumentNullException(nameof(answer), "Answer cannot be empty.");
            Id = id;
            UserId = userId;
            Question = question;
            Answer = answer;
        }
    }
}
