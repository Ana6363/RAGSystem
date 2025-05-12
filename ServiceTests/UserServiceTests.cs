using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using nBanks.Application.Users;
using Domain.Models.Users;
using Application.Users;

namespace Tests.Users
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnDTO_WhenUserFound()
        {
            // Arrange
            var vatNumber = new VATNumber("VAT123");
            var user = new User { Id = "user123", VATNumber = vatNumber };
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync("user123"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync("user123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user123", result.Id);
            Assert.Equal("VAT123", result.VATNumber);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync("user123"))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _userService.GetUserByIdAsync("user123");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByVATNumberAsync_ShouldReturnDTO_WhenUserFound()
        {
            // Arrange
            var vatNumber = new VATNumber("VAT123");
            var user = new User { Id = "user123", VATNumber = vatNumber };
            _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatNumber.VATValue))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByVATNumberAsync("VAT123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user123", result.Id);
            Assert.Equal("VAT123", result.VATNumber);
        }

        [Fact]
        public async Task GetUserByVATNumberAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var vatNumber = new VATNumber("VAT123");
            _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatNumber.VATValue))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _userService.GetUserByVATNumberAsync("VAT123");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUserAndReturnDTO()
        {
            // Arrange
            var vatNumber = new VATNumber("VAT123");
            var userDTO = new UserDTO(vatNumber.VATValue);
            var user = new User { Id = "user123", VATNumber = vatNumber };

            _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatNumber.VATValue))
                .ReturnsAsync((User)null!); // No user exists

            _userRepositoryMock.Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.AddUserAsync(userDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("VAT123", result.VATNumber);
        }

        [Fact]
        public async Task AddUserAsync_ShouldThrow_WhenUserWithVATNumberExists()
        {
            // Arrange
            var vatNumber = new VATNumber("VAT123");
            var userDTO = new UserDTO(vatNumber.VATValue);
            var existingUser = new User { Id = "user123", VATNumber = vatNumber };

            _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatNumber.VATValue))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.AddUserAsync(userDTO));
        }

        [Fact]
        public async Task AddUserAsync_ShouldThrow_WhenUserIsNull()
        {
            // Arrange
            UserDTO nullUserDTO = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _userService.AddUserAsync(nullUserDTO));
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnDTO_WhenUserExists()
        {
            // Arrange
            var vatNumber = new VATNumber("VAT123");
            var user = new User { Id = "user123", VATNumber = vatNumber };

            _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatNumber.VATValue))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(repo => repo.DeleteUserAsync("user123"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.DeleteUserAsync("VAT123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("VAT123", result.VATNumber);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var vatNumber = new VATNumber("VAT123");

            _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatNumber.VATValue))
                .ReturnsAsync((User)null!);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.DeleteUserAsync("VAT123"));
        }
    }
}
