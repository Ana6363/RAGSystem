using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.ChatHistories;
using nBanks.Application.Documents;
using nBanks.Application;
using Domain.Models.RagServer;
using Infrastructure.OpenAI;

namespace nBanks.Application.ChatHistories
{
    public class ChatHistoryService
    {
        private readonly IChatHistoryRepository _chatHistoryRepository;
        private readonly DocumentService _documentService;
        private readonly IRagServerRepository _ragServerRepository;
        private readonly OpenAIService _openAIService;


        public ChatHistoryService(IChatHistoryRepository chatHistoryRepository,
                                DocumentService documentService,
                                IRagServerRepository ragServerRepository,
                                OpenAIService openAIService)
        {
            _chatHistoryRepository = chatHistoryRepository;
            _documentService = documentService;
            _ragServerRepository = ragServerRepository;
            _openAIService = openAIService;
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

            if (string.IsNullOrWhiteSpace(chatHistory.Title))
            {
                string fileContentSnippet = "";

                if (chatHistory.FileIds != null && chatHistory.FileIds.Any())
                {
                    var firstFileId = chatHistory.FileIds.First();
                    var document = await _documentService.GetDocumentByIdAsync(firstFileId);
                    if (document != null)
                    {
                        fileContentSnippet = document.Content.Length > 500
                            ? document.Content.Substring(0, 500)
                            : document.Content;
                    }
                }

                var prompt = $"Generate a concise, meaningful title for this chat based on the content of the first file:\n\n{fileContentSnippet}";

                var rawTitle = await _openAIService.GenerateChatTitleAsync(prompt);

                var title = rawTitle.Trim().Trim('"', '\'');

                chatHistory.Title = title;

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

        public async Task<List<ChatMessage>> AskQuestionAsync(string chatId, string question)
        {
            var chat = await _chatHistoryRepository.GetChatHistoryByIdAsync(chatId);
            if (chat == null)
            {
                throw new Exception("Chat history not found.");
            }

            chat.Messages.Add(new ChatMessage
            {
                Role = "user",
                Content = question,
                Timestamp = DateTime.UtcNow
            });

            string answer;
            try
            {
                answer = await _ragServerRepository.QueryAsync(chat.FileIds, question);

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get answer from RAG server: {ex.Message}");
            }

            chat.Messages.Add(new ChatMessage
            {
                Role = "assistant",
                Content = answer,
                Timestamp = DateTime.UtcNow
            });

            await _chatHistoryRepository.UpdateChatHistoryAsync(chat);

            var lastTwoMessages = chat.Messages
                .OrderByDescending(m => m.Timestamp)
                .Take(2)
                .OrderBy(m => m.Timestamp)
                .ToList();

            return lastTwoMessages;

        }

        public async Task AttachFileAsync(string chatId, string fileId)
        {
            var chat = await _chatHistoryRepository.GetChatHistoryByIdAsync(chatId);
            if (chat == null)
            {
                throw new Exception("Chat history not found.");
            }

            if (!chat.FileIds.Contains(fileId))
            {
                chat.FileIds.Add(fileId);
                await _chatHistoryRepository.UpdateChatHistoryAsync(chat);
            }
        }


    }
}
