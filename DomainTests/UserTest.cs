using System;
using Domain.Models.Users;
using Moq;
using Xunit;

namespace Domain.Tests.Models.Users
{
    public class UserTest
    {
        [Fact]
        public void CreateUser_WithValidMockVATNumber_ShouldCreateSuccessfully()
        {
            // Arrange
            var mockVatNumber = new Mock<VATNumber>();
            mockVatNumber.Setup(v => v.ToString()).Returns("PT123456789");

            // Act
            var user = new User(mockVatNumber.Object);

            // Assert
            Assert.NotNull(user.Id);
            Assert.Equal(mockVatNumber.Object, user.VATNumber);
        }

        [Fact]
        public void CreateUser_WithNullVATNumber_ShouldCreateWithNullVAT()
        {
            // Act
            var user = new User(null);

            // Assert
            Assert.NotNull(user.Id);
            Assert.Null(user.VATNumber);
        }

        [Fact]
        public void DefaultConstructor_ShouldCreateUserWithNewId()
        {
            // Act
            var user = new User();

            // Assert
            Assert.NotNull(user.Id);
            Assert.Null(user.VATNumber);
        }
    }
}
