using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.Models.ChatHistories;
using Domain.Models.RagServer;
using Moq;
using nBanks.Application;
using nBanks.Application.ChatHistories;
using nBanks.Application.Documents;
using Infrastructure.OpenAI;
using Microsoft.Extensions.Options;
using Xunit;

namespace Tests.ChatHistories
{
    public class ChatHistoryServiceTests
    {
        private readonly Mock<IChatHistoryRepository> _chatHistoryRepoMock;
        private readonly Mock<IRagServerRepository> _ragServerRepoMock;
        private readonly Mock<DocumentService> _documentServiceMock; // Mock concrete class
        private readonly ChatHistoryService _chatHistoryService;
        private readonly Mock<OpenAIService> _openAIServiceMock;

        public ChatHistoryServiceTests()
        {
            _chatHistoryRepoMock = new Mock<IChatHistoryRepository>();
            _ragServerRepoMock = new Mock<IRagServerRepository>();
            _documentServiceMock = new Mock<DocumentService>();

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();

            var settings = new OpenAISettings
            {
                ApiKey = "fake-api-key",
                Model = "gpt-4",
                ProjectId = "test-project"
            };

            // Create IOptions instance without mocking
            IOptions<OpenAISettings> options = Options.Create(settings);

            // Setup HttpClient with dummy base address (won't be used because AskChatAsync is mocked)
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };

            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Create Mock of OpenAIService calling real constructor (dependencies injected)
            _openAIServiceMock = new Mock<OpenAIService>(httpClientFactoryMock.Object, options);

            // Mock AskChatAsync to return a fixed string
            _openAIServiceMock.Setup(s => s.AskChatAsync(It.IsAny<string>()))
                              .ReturnsAsync("mocked response");

            _chatHistoryService = new ChatHistoryService(
                _chatHistoryRepoMock.Object,
                _documentServiceMock.Object,
                _ragServerRepoMock.Object,
                _openAIServiceMock.Object
            );
        }

        [Fact]
        public async Task GetChatHistoryById_ShouldReturnDTO_WhenFound()
        {
            var chat = new ChatHistory("user123") { Id = "chat1" };
            _chatHistoryRepoMock.Setup(repo => repo.GetChatHistoryByIdAsync("chat1"))
                .ReturnsAsync(chat);

            var result = await _chatHistoryService.GetChatHistoryById("chat1");

            Assert.NotNull(result);
            Assert.Equal("user123", result.UserId);
            Assert.Equal("chat1", result.Id);
        }

        [Fact]
        public async Task GetChatHistoryById_ShouldReturnNull_WhenNotFound()
        {
            _chatHistoryRepoMock.Setup(r => r.GetChatHistoryByIdAsync("chat123")).ReturnsAsync((ChatHistory)null);

            var result = await _chatHistoryService.GetChatHistoryById("chat123");

            Assert.Null(result);
        }

        [Fact]
        public async Task AddChatHistory_ShouldAddAndReturnDTO()
        {
            var dto = new ChatHistoryDTO { UserId = "user1" };

            _chatHistoryRepoMock
                .Setup(r => r.AddChatHistoryAsync(It.IsAny<ChatHistory>()))
                .Returns(Task.CompletedTask);

            var result = await _chatHistoryService.AddChatHistory(dto);

            Assert.NotNull(result);
            Assert.Equal("user1", result.UserId);
        }

        [Fact]
        public async Task UpdateChatHistory_ShouldThrow_WhenDTOIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _chatHistoryService.UpdateChatHistory(null!));
        }

        [Fact]
        public async Task UpdateChatHistory_ShouldThrow_WhenNotFound()
        {
            var mockRepo = new Mock<IChatHistoryRepository>();
            var mockDocService = new Mock<DocumentService>();
            var mockRagRepo = new Mock<IRagServerRepository>();

            mockRepo.Setup(r => r.GetChatHistoryByIdAsync("id1")).ReturnsAsync((ChatHistory)null!);

            var service = new ChatHistoryService(mockRepo.Object, mockDocService.Object, mockRagRepo.Object, _openAIServiceMock.Object);

            var dto = new ChatHistoryDTO
            {
                Id = "id1",
                Messages = new List<ChatMessage>()
            };

            await Assert.ThrowsAsync<Exception>(() => service.UpdateChatHistory(dto));
        }

        [Fact]
        public async Task DeleteChatHistory_ShouldCallDelete_WhenExists()
        {
            var chat = new ChatHistory("user123") { Id = "id123" };

            _chatHistoryRepoMock.Setup(r => r.GetChatHistoryByIdAsync("id123"))
                .ReturnsAsync(chat);

            _chatHistoryRepoMock.Setup(r => r.DeleteChatHistoryAsync("id123"))
                .Returns(Task.CompletedTask);

            await _chatHistoryService.DeleteChatHistory("id123");

            _chatHistoryRepoMock.Verify(r => r.DeleteChatHistoryAsync("id123"), Times.Once);
        }

        [Fact]
        public async Task AskQuestionAsync_ShouldAddUserAndAssistantMessages()
        {
            var chat = new ChatHistory("user1") { Id = "chat123", FileIds = new List<string> { "file1" } };

            _chatHistoryRepoMock.Setup(r => r.GetChatHistoryByIdAsync("chat123"))
                .ReturnsAsync(chat);

            _ragServerRepoMock.Setup(r => r.QueryAsync(chat.FileIds, "What is this?"))
                .ReturnsAsync("This is a test answer.");

            _chatHistoryRepoMock.Setup(r => r.UpdateChatHistoryAsync(It.IsAny<ChatHistory>()))
                .Returns(Task.CompletedTask);

            var messages = await _chatHistoryService.AskQuestionAsync("chat123", "What is this?");

            Assert.Equal(2, messages.Count);
            Assert.Equal("user", messages[0].Role);
            Assert.Equal("assistant", messages[1].Role);
        }

        [Fact]
        public async Task GetAllChatHistories_ShouldReturnList_WhenFound()
        {
            var chats = new List<ChatHistory>
            {
                new ChatHistory("user1") { Id = "chat1" },
                new ChatHistory("user1") { Id = "chat2" }
            };

            _chatHistoryRepoMock.Setup(repo => repo.GetAllChatHistoriesAsync("user1"))
                .ReturnsAsync(chats);

            var result = await _chatHistoryService.GetAllChatHistories("user1");

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, dto => Assert.Equal("user1", dto.UserId));
        }

        [Fact]
        public async Task AttachFileAsync_ShouldAddFileId_WhenNotAlreadyAttached()
        {
            var chat = new ChatHistory("user1") { Id = "chat123", FileIds = new List<string>() };

            _chatHistoryRepoMock.Setup(r => r.GetChatHistoryByIdAsync("chat123"))
                .ReturnsAsync(chat);

            _chatHistoryRepoMock.Setup(r => r.UpdateChatHistoryAsync(It.IsAny<ChatHistory>()))
                .Returns(Task.CompletedTask);

            await _chatHistoryService.AttachFileAsync("chat123", "file123");

            _chatHistoryRepoMock.Verify(r => r.UpdateChatHistoryAsync(It.Is<ChatHistory>(
                c => c.FileIds.Contains("file123"))), Times.Once);
        }

        [Fact]
        public async Task AttachFileAsync_ShouldNotAddFileId_WhenAlreadyAttached()
        {
            var chat = new ChatHistory("user1") { Id = "chat123", FileIds = new List<string> { "file123" } };

            _chatHistoryRepoMock.Setup(r => r.GetChatHistoryByIdAsync("chat123"))
                .ReturnsAsync(chat);

            await _chatHistoryService.AttachFileAsync("chat123", "file123");

            _chatHistoryRepoMock.Verify(r => r.UpdateChatHistoryAsync(It.IsAny<ChatHistory>()), Times.Never);
        }

        [Fact]
        public async Task AttachFileAsync_ShouldThrowException_WhenChatNotFound()
        {
            _chatHistoryRepoMock.Setup(r => r.GetChatHistoryByIdAsync("chat123"))
                .ReturnsAsync((ChatHistory)null!);

            await Assert.ThrowsAsync<Exception>(() => _chatHistoryService.AttachFileAsync("chat123", "file123"));
        }
    }
}
