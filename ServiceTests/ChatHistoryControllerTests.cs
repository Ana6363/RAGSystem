using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models.ChatHistories;
using Domain.Models.Documents;
using Microsoft.AspNetCore.Mvc;
using Moq;
using nBanks.Application.ChatHistories;
using nBanks.Application.Documents;
using nBanks.Controllers;
using Domain.Models.RagServer;
using Infrastructure.OpenAI;
using Xunit;
using Microsoft.Extensions.Options;

namespace Controller.IntegrationTests
{
    public class ChatHistoryControllerTests
    {
        private readonly Mock<IChatHistoryRepository> _chatRepoMock;
        private readonly Mock<IRagServerRepository> _ragRepoMock;
        private readonly Mock<OpenAIService> _openAiMock;
        private readonly Mock<IDocumentRepository> _documentRepositoryMock;
        private readonly DocumentService _docService;

        private readonly ChatHistoryService _chatService;
        private readonly ChatHistoryController _controller;


        public ChatHistoryControllerTests()
        {
            _chatRepoMock = new Mock<IChatHistoryRepository>();
            _ragRepoMock = new Mock<IRagServerRepository>();

            // Mock dependencies for OpenAIService
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var settings = new OpenAISettings
            {
                ApiKey = "fake-api-key",
                Model = "gpt-4",
                ProjectId = "test-project"
            };
            IOptions<OpenAISettings> options = Options.Create(settings);

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };

            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Properly create OpenAIService mock passing dependencies to constructor
            _openAiMock = new Mock<OpenAIService>(httpClientFactoryMock.Object, options);

            // Setup the mocked method(s)
            _openAiMock.Setup(s => s.AskChatAsync(It.IsAny<string>()))
                      .ReturnsAsync("mocked response");

            _documentRepositoryMock = new Mock<IDocumentRepository>();
            _docService = new DocumentService(_documentRepositoryMock.Object);

            _chatService = new ChatHistoryService(
                _chatRepoMock.Object,
                _docService,
                _ragRepoMock.Object,
                _openAiMock.Object
            );

            _controller = new ChatHistoryController(_chatService, _docService);
        }


        [Fact]
        public async Task CreateAsync_ReturnsOk_WhenValidChatHistory()
        {
            // Arrange
            var chatDto = new ChatHistoryDTO
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "user1",
                Title = "My Chat",
                FileIds = new List<string>(),
                Messages = new List<ChatMessage>()
            };

            _chatRepoMock.Setup(repo => repo.AddChatHistoryAsync(It.IsAny<ChatHistory>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateAsync(chatDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<ChatHistoryDTO>(okResult.Value);
            Assert.Equal(chatDto.UserId, returnedDto.UserId);
        }

        [Fact]
        public async Task GetByUser_ReturnsOk_WhenChatsExist()
        {
            // Arrange
            var userId = "user123";
            var chats = new List<ChatHistory>
            {
                new ChatHistory { Id = "1", UserId = userId },
                new ChatHistory { Id = "2", UserId = userId }
            };

            _chatRepoMock.Setup(repo => repo.GetAllChatHistoriesAsync(userId))
                        .ReturnsAsync(chats);

            // Act
            var result = await _controller.GetByUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<ChatHistoryDTO>>(okResult.Value);
            Assert.Equal(2, returnedList.Count);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk_WhenUpdateIsValid()
        {
            // Arrange
            var dto = new ChatHistoryDTO
            {
                Id = "chat123",
                Title = "Updated title",
                Messages = new List<ChatMessage>
                {
                    new ChatMessage { Content = "Hi", Role = "user", Timestamp = DateTime.UtcNow }
                }
            };

            _chatRepoMock.Setup(repo => repo.GetChatHistoryByIdAsync(dto.Id))
                        .ReturnsAsync(new ChatHistory { Id = dto.Id, Messages = new List<ChatMessage>() });

            _chatRepoMock.Setup(repo => repo.UpdateChatHistoryAsync(It.IsAny<ChatHistory>()))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateAsync(dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsOk_WhenChatExists()
        {
            // Arrange
            var id = "chat123";
            _chatRepoMock.Setup(repo => repo.GetChatHistoryByIdAsync(id))
                        .ReturnsAsync(new ChatHistory { Id = id });

            _chatRepoMock.Setup(repo => repo.DeleteChatHistoryAsync(id))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAsync(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AskQuestion_ReturnsOk_WithAnswer()
        {
            // Arrange
            var chatId = "chat123";
            var question = "What is AI?";

            var existingChat = new ChatHistory
            {
                Id = chatId,
                FileIds = new List<string>(),
                Messages = new List<ChatMessage>()
            };

            _chatRepoMock.Setup(repo => repo.GetChatHistoryByIdAsync(chatId))
                        .ReturnsAsync(existingChat);

            _ragRepoMock.Setup(rag => rag.QueryAsync(It.IsAny<List<string>>(), question))
                        .ReturnsAsync("AI stands for Artificial Intelligence.");

            _chatRepoMock.Setup(repo => repo.UpdateChatHistoryAsync(It.IsAny<ChatHistory>()))
                        .Returns(Task.CompletedTask);

            var dto = new AskQuestionDTO { ChatId = chatId, Question = question };

            // Act
            var result = await _controller.AskQuestion(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var messages = Assert.IsType<List<ChatMessage>>(okResult.Value);
            Assert.Equal(2, messages.Count); // user + assistant
        }


        [Fact]
        public async Task UpdateFileAsync_AttachesFile_WhenValid()
        {
            // Arrange
            var chatId = "chat456";
            var fileId = "file123";

            var chat = new ChatHistory
            {
                Id = chatId,
                FileIds = new List<string>()
            };

            _chatRepoMock.Setup(repo => repo.GetChatHistoryByIdAsync(chatId))
                        .ReturnsAsync(chat);

            _chatRepoMock.Setup(repo => repo.UpdateChatHistoryAsync(It.IsAny<ChatHistory>()))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateFileAsync(chatId, fileId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Use reflection or dynamic to get the message property since it's anonymous type
            var messageProperty = okResult.Value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);

            var messageValue = messageProperty.GetValue(okResult.Value) as string;
            Assert.Equal("File attached to chat successfully.", messageValue);
        }



    }
    public class MessageResponse
    {
        public string message { get; set; }
    }

}
