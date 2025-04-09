using Domain.Models.ChatHistories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mongo.ChatHistories
{
    public class ChatHistoryRepository : IChatHistoryRepository
    {
        private readonly IMongoCollection<ChatHistory> _chatHistoryCollection;

        public ChatHistoryRepository(MongoDbContext database)
        {
            _chatHistoryCollection = database.ChatHistories;
        }

        public async Task<ChatHistory?> GetChatHistoryByUserIdAsync(string userId)
        {
            return await _chatHistoryCollection.Find(chatHistory => chatHistory.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<ChatHistory?> GetChatHistoryByIdAsync(string id)
        {
            return await _chatHistoryCollection.Find(chatHistory => chatHistory.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddChatHistoryAsync(ChatHistory chatHistory)
        {
            await _chatHistoryCollection.InsertOneAsync(chatHistory);
        }

        public async Task UpdateChatHistoryAsync(ChatHistory chatHistory)
        {
            await _chatHistoryCollection.ReplaceOneAsync(ch => ch.Id == chatHistory.Id, chatHistory);
        }

        public async Task DeleteChatHistoryAsync(string id)
        {
            await _chatHistoryCollection.DeleteOneAsync(chatHistory => chatHistory.Id == id);
        }

        public async Task<List<ChatHistory>> GetAllChatHistoriesAsync(string userId)
        {
            return await _chatHistoryCollection.Find(chatHistory => chatHistory.UserId == userId).ToListAsync();
        }

        public async Task<List<ChatHistory>> GetAllChatHistoriesAsync()
        {
            return await _chatHistoryCollection.Find(_ => true).ToListAsync();
        }
    }
}
