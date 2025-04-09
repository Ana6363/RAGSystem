using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ChatHistories
{
    public interface IChatHistoryRepository
    {
        Task<ChatHistory?> GetChatHistoryByUserIdAsync(string userId);
        Task<ChatHistory?> GetChatHistoryByIdAsync(string id);
        Task AddChatHistoryAsync(ChatHistory chatHistory);
        Task UpdateChatHistoryAsync(ChatHistory chatHistory);
        Task DeleteChatHistoryAsync(string id);
        Task<List<ChatHistory>> GetAllChatHistoriesAsync(string userId);

    }
}
