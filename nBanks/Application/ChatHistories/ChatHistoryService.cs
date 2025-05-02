using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.ChatHistories;
using nBanks.Application.Documents;
using nBanks.Application;

namespace nBanks.Application.ChatHistories
{
    public class ChatHistoryService
    {
        private readonly IChatHistoryRepository _chatHistoryRepository;
        private readonly DocumentService _documentService;

        public ChatHistoryService(IChatHistoryRepository chatHistoryRepository, DocumentService documentService)
        {
            _chatHistoryRepository = chatHistoryRepository;
            _documentService = documentService;
        }

        public async Task<ChatHistoryDTO> GetChatHistoryById(string id)
        {
            var chatHistory = await _chatHistoryRepository.GetChatHistoryByIdAsync(id);
            return chatHistory == null ? null : ChatHistoryMapper.MapToDTO(chatHistory);
        }

        public async Task<List<ChatHistoryDTO>> GetAllChatHistories(string userId)
        {
            var chatHistories = await _chatHistoryRepository.GetAllChatHistoriesAsync(userId);
            return chatHistories == null ? null : chatHistories.Select(ChatHistoryMapper.MapToDTO).ToList();
        }

        public async Task<ChatHistoryDTO> AddChatHistory(ChatHistoryDTO chatHistoryDTO)
        {
            var chatHistory = ChatHistoryMapper.MapToDomain(chatHistoryDTO);

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
            if (chatHistoryDTO == null || string.IsNullOrWhiteSpace(chatHistoryDTO.Id))
                throw new ArgumentException("Invalid chat history DTO.");

            var existingChatHistory = await _chatHistoryRepository.GetChatHistoryByIdAsync(chatHistoryDTO.Id);
            if (existingChatHistory == null)
                throw new Exception("Chat history not found.");

            if (chatHistoryDTO.Messages != null && chatHistoryDTO.Messages.Count > 0)
            {
                foreach (var message in chatHistoryDTO.Messages)
                {
                    if (!string.IsNullOrWhiteSpace(message.Content))
                        existingChatHistory.Messages.Add(message);
                }
            }

            await _chatHistoryRepository.UpdateChatHistoryAsync(existingChatHistory);
        }

        public async Task DeleteChatHistory(string id)
        {
            var chatHistory = await _chatHistoryRepository.GetChatHistoryByIdAsync(id);
            if (chatHistory == null)
            {
                throw new Exception("Chat history not found.");
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
