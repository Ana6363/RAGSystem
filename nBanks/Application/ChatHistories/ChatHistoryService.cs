using System;
using System.Collections.Generic;
using Domain.Models.ChatHistories;

namespace nBanks.Application.ChatHistories
{
    public class ChatHistoryService
    {
        private readonly IChatHistoryRepository _chatHistoryRepository;
        public ChatHistoryService(IChatHistoryRepository chatHistoryRepository)
        {
            _chatHistoryRepository = chatHistoryRepository;
        }
        public async Task<ChatHistoryDTO> GetChatHistoryById(string id)
        {
            var chatHistory = await _chatHistoryRepository.GetChatHistoryByIdAsync(id);
            return chatHistory == null ? null : ChatHistoryMapper.MapToDTO(chatHistory);
        }

        public async Task<List<ChatHistoryDTO>> GetAllChatHistories(string userID)
        {

            var chatHistories = await _chatHistoryRepository.GetAllChatHistoriesAsync(userID);
            return chatHistories == null ? null : chatHistories.Select(ChatHistoryMapper.MapToDTO).ToList();
        }


        public async Task<ChatHistoryDTO> AddChatHistory(ChatHistoryDTO chatHistoryDTO)
        {
            var dto = new ChatHistoryDTO(chatHistoryDTO.UserId, chatHistoryDTO.Question, chatHistoryDTO.Answer);
            var chatHistory = ChatHistoryMapper.MapToDomain(dto);

            if (await _chatHistoryRepository.GetChatHistoryByUserIdAsync(chatHistory.UserId) != null)
            {
                throw new Exception("Chat history already exists for this user.");
            }

            try
            {
                await _chatHistoryRepository.AddChatHistoryAsync(chatHistory);
                return ChatHistoryMapper.MapToDTO(chatHistory);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding chat history: " + ex.Message);
            }
         
        }


        public async Task UpdateChatHistory(ChatHistoryDTO chatHistoryDTO)
        {
            var chatHistory = ChatHistoryMapper.MapToDomain(chatHistoryDTO);
            await _chatHistoryRepository.UpdateChatHistoryAsync(chatHistory);
        }
        public async Task DeleteChatHistory(string id)
        {
            var chatHistory = await _chatHistoryRepository.GetChatHistoryByIdAsync(id);
            if (chatHistory == null)
            {
                throw new Exception("Chat history not found.");
            }
            if (chatHistory.UserId != id)
            {
                throw new Exception("You cannot delete this chat history.");
            }

            try
            {
                await _chatHistoryRepository.DeleteChatHistoryAsync(chatHistory.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting chat history: " + ex.Message);
            }
        }
    }
}
