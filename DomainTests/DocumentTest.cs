using System;
using Domain.Models;
using Domain.Models.Documents;
using Moq;
using Xunit;

namespace Domain.Tests.Models
{
    public class DocumentTest
    {
        [Fact]
        public void CreateDocument_WithValidParameters_ShouldCreateSuccessfully()
        {
            // Arrange
            var mockContent = new Mock<Content>();
            mockContent.Setup(c => c.ToString()).Returns("Valid content");

            var mockFileName = new Mock<FileName>();
            mockFileName.Setup(f => f.ToString()).Returns("file.pdf");

            string userId = "507f1f77bcf86cd799439011";

            // Act
            var document = new Document(userId, mockFileName.Object, mockContent.Object);

            // Assert
            Assert.Equal(userId, document.UserId);
            Assert.Equal(mockContent.Object, document.content);
            Assert.Equal(mockFileName.Object, document.FileName);
            Assert.False(string.IsNullOrEmpty(document.Id));
        }

        [Fact]
        public void CreateDocument_WithEmptyContent_ShouldThrowArgumentException()
        {
            // Arrange
            var mockContent = new Mock<Content>();
            mockContent.Setup(c => c.ToString()).Returns(string.Empty);

            var mockFileName = new Mock<FileName>();
            mockFileName.Setup(f => f.ToString()).Returns("file.pdf");

            string userId = "507f1f77bcf86cd799439011";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Document(userId, mockFileName.Object, mockContent.Object));

            Assert.Contains("Content cannot be null or empty", exception.Message);
        }

        [Fact]
        public void CreateDocument_WithEmptyFileName_ShouldThrowArgumentException()
        {
            // Arrange
            var mockContent = new Mock<Content>();
            mockContent.Setup(c => c.ToString()).Returns("Valid content");

            var mockFileName = new Mock<FileName>();
            mockFileName.Setup(f => f.ToString()).Returns(string.Empty);

            string userId = "507f1f77bcf86cd799439011";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Document(userId, mockFileName.Object, mockContent.Object));

            Assert.Contains("File name cannot be null or empty", exception.Message);
        }
    }
}
