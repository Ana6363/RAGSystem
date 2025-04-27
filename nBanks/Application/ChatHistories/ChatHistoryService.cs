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
            chatHistoryDTO.Questions ??= new List<string>();
            chatHistoryDTO.Answers ??= new List<string>();

            var chatHistory = ChatHistoryMapper.MapToDomain(chatHistoryDTO);

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
            if (chatHistoryDTO == null || string.IsNullOrWhiteSpace(chatHistoryDTO.Id))
                throw new ArgumentException("Invalid chat history DTO.");

            var existingChatHistory = await _chatHistoryRepository.GetChatHistoryByIdAsync(chatHistoryDTO.Id);
            if (existingChatHistory == null)
                throw new Exception("Chat history not found.");

            if (chatHistoryDTO.Questions == null || chatHistoryDTO.Questions.Count == 0 ||
                chatHistoryDTO.Answers == null || chatHistoryDTO.Answers.Count == 0)
            {
                throw new ArgumentException("Update must include at least one new question and one new answer.");
            }

            foreach (var question in chatHistoryDTO.Questions)
            {
                if (!string.IsNullOrWhiteSpace(question))
                    existingChatHistory.Questions.Add(new Question(question));
            }

            foreach (var answer in chatHistoryDTO.Answers)
            {
                if (!string.IsNullOrWhiteSpace(answer))
                    existingChatHistory.Answers.Add(new Answer(answer));
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

        public async Task<string> AskQuestionAndStoreAsync(string chatHistoryId, string question)
        {
            var existingChatHistory = await _chatHistoryRepository.GetChatHistoryByIdAsync(chatHistoryId);
            if (existingChatHistory == null)
            {
                throw new Exception("Chat history not found.");
            }

            string userId = existingChatHistory.UserId;

            var documents = await _documentService.GetDocumentsByUserIdAsync(userId);
            var fileIds = documents?.Select(d => d.Id).ToList() ?? new List<string>();

            if (fileIds.Count == 0)
            {
                throw new Exception("No documents found for the user.");
            }

            string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "RAG-Server", "query_engine.py");

            var answer = await PythonRunner.RunPythonScriptAsync(scriptPath, fileIds, question);

            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new Exception("No answer received from the Python script.");
            }

            var updateDTO = new ChatHistoryDTO(
                userId, 
                new List<string> { question },
                new List<string> { answer },  
                chatHistoryId           
            );


            await UpdateChatHistory(updateDTO);

            return answer;
        }


    }
}
