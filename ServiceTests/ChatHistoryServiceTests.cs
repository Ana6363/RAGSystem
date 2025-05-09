using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models.ChatHistories;
using Domain.Models.RagServer;
using Moq;
using nBanks.Application;
using nBanks.Application.ChatHistories;
using nBanks.Application.Documents;
using Xunit;

namespace Tests.ChatHistories
{
    public class ChatHistoryServiceTests
    {
        private readonly Mock<IChatHistoryRepository> _chatHistoryRepoMock;
        private readonly Mock<IRagServerRepository> _ragServerRepoMock;
        private readonly Mock<DocumentService> _documentServiceMock; // Mock the concrete class
        private readonly ChatHistoryService _chatHistoryService;

        public ChatHistoryServiceTests()
        {
            _chatHistoryRepoMock = new Mock<IChatHistoryRepository>();
            _ragServerRepoMock = new Mock<IRagServerRepository>();
            _documentServiceMock = new Mock<DocumentService>(); // Keep mocking DocumentService

            _chatHistoryService = new ChatHistoryService(
                _chatHistoryRepoMock.Object,
                _documentServiceMock.Object,
                _ragServerRepoMock.Object
            );
        }

        [Fact]
        public async Task GetChatHistoryById_ShouldReturnDTO_WhenFound()
        {
            // Arrange
            var chat = new ChatHistory("user123") { Id = "chat1" };
            _chatHistoryRepoMock.Setup(repo => repo.GetChatHistoryByIdAsync("chat1"))
                .ReturnsAsync(chat);

            // Act
            var result = await _chatHistoryService.GetChatHistoryById("chat1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user123", result.UserId);
            Assert.Equal("chat1", result.Id); // Add assertion for ID
        }

        [Fact]
        public async Task GetChatHistoryById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _chatHistoryRepoMock.Setup(r => r.GetChatHistoryByIdAsync("chat123")).ReturnsAsync((ChatHistory)null);

            // Act
            var result = await _chatHistoryService.GetChatHistoryById("chat123");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddChatHistory_ShouldAddAndReturnDTO()
        {
            // Arrange
            var dto = new ChatHistoryDTO { UserId = "user1" };

            _chatHistoryRepoMock
                .Setup(r => r.AddChatHistoryAsync(It.IsAny<ChatHistory>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _chatHistoryService.AddChatHistory(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user1", result.UserId);
        }

        [Fact]
        public async Task UpdateChatHistory_ShouldThrow_WhenDTOIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _chatHistoryService.UpdateChatHistory(null!));
        }

        [Fact]
        public async Task UpdateChatHistory_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IChatHistoryRepository>();
            var mockDocService = new Mock<DocumentService>(); // Keep it as the concrete class
            var mockRagRepo = new Mock<IRagServerRepository>();

            mockRepo.Setup(r => r.GetChatHistoryByIdAsync("id1")).ReturnsAsync((ChatHistory)null!);

            var service = new ChatHistoryService(mockRepo.Object, mockDocService.Object, mockRagRepo.Object);

            var dto = new ChatHistoryDTO
            {
                Id = "id1",
                Messages = new List<ChatMessage>() // Corrected from ChatMessageDTO to ChatMessage
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.UpdateChatHistory(dto));
        }

        [Fact]
        public async Task DeleteChatHistory_ShouldCallDelete_WhenExists()
        {
            // Arrange
            var chat = new ChatHistory("user123") { Id = "id123" };

            _chatHistoryRepoMock.Setup(r => r.GetChatHistoryByIdAsync("id123"))
                .ReturnsAsync(chat);

            _chatHistoryRepoMock.Setup(r => r.DeleteChatHistoryAsync("id123"))
                .Returns(Task.CompletedTask);

            // Act
            await _chatHistoryService.DeleteChatHistory("id123");

            // Assert
            _chatHistoryRepoMock.Verify(r => r.DeleteChatHistoryAsync("id123"), Times.Once);
        }

        [Fact]
        public async Task AskQuestionAsync_ShouldAddUserAndAssistantMessages()
        {
            // Arrange
            var chat = new ChatHistory("user1") { Id = "chat123", FileIds = new List<string> { "file1" } };

            _chatHistoryRepoMock.Setup(r => r.GetChatHistoryByIdAsync("chat123"))
                .ReturnsAsync(chat);

            _ragServerRepoMock.Setup(r => r.QueryAsync(chat.FileIds, "What is this?"))
                .ReturnsAsync("This is a test answer.");

            _chatHistoryRepoMock.Setup(r => r.UpdateChatHistoryAsync(It.IsAny<ChatHistory>()))
                .Returns(Task.CompletedTask);

            // Act
            var messages = await _chatHistoryService.AskQuestionAsync("chat123", "What is this?");

            // Assert
            Assert.Equal(2, messages.Count); // Ensure 2 messages (user and assistant)
            Assert.Equal("user", messages[0].Role);
            Assert.Equal("assistant", messages[1].Role);
        }
    }
}
