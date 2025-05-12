using System;
using System.Collections.Generic;
using Domain.Models.ChatHistories;
using Moq;
using Xunit;

namespace Domain.Tests.Models.ChatHistories
{
    public class ChatHistoryTest
    {
        [Fact]
        public void CreateChatHistory_WithValidUserIdAndFileIds_ShouldInitializeCorrectly()
        {
            // Arrange
            var userId = "507f1f77bcf86cd799439011";
            var fileIds = new List<string> { "609e125b8f1b2c1df0c5e2d1", "609e125b8f1b2c1df0c5e2d2" };

            // Act
            var chatHistory = new ChatHistory(userId, fileIds);

            // Assert
            Assert.Equal(userId, chatHistory.UserId);
            Assert.Equal(2, chatHistory.FileIds.Count);
            Assert.NotNull(chatHistory.Messages);
            Assert.Empty(chatHistory.Messages);
            Assert.False(string.IsNullOrEmpty(chatHistory.Id));
            Assert.True((DateTime.UtcNow - chatHistory.CreatedAt).TotalSeconds < 5);
        }


        [Fact]
        public void CreateChatHistory_WithNullFileIds_ShouldInitializeEmptyFileIdsList()
        {
            // Arrange
            var userId = "507f1f77bcf86cd799439011";

            // Act
            var chatHistory = new ChatHistory(userId, null);

            // Assert
            Assert.Equal(userId, chatHistory.UserId);
            Assert.NotNull(chatHistory.FileIds);
            Assert.Empty(chatHistory.FileIds);
        }

        [Fact]
        public void ChatHistory_DefaultConstructor_ShouldSetDefaultValues()
        {
            // Act
            var chatHistory = new ChatHistory();

            // Assert
            Assert.False(string.IsNullOrEmpty(chatHistory.Id));
            Assert.NotNull(chatHistory.FileIds);
            Assert.NotNull(chatHistory.Messages);
            Assert.Empty(chatHistory.FileIds);
            Assert.Empty(chatHistory.Messages);
            Assert.True((DateTime.UtcNow - chatHistory.CreatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void ChatHistory_ShouldAllowAddingMessages()
        {
            // Arrange
            var chatHistory = new ChatHistory("507f1f77bcf86cd799439011");

            var mockMessage = new Mock<ChatMessage>();
            chatHistory.Messages.Add(mockMessage.Object);

            // Assert
            Assert.Single(chatHistory.Messages);
            Assert.Contains(mockMessage.Object, chatHistory.Messages);
        }
    }
}
