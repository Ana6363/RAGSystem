using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

using nBanks.Application.Users;  // UserService
using nBanks.Controllers;        // UsersController
using Domain.Models.Users;       // UserDTO, User domain models, VATNumber
using Application.Users;         // IUserRepository interface

public class UsersControllerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _userService = new UserService(_userRepositoryMock.Object);
        _controller = new UsersController(_userService);
    }

    [Fact]
    public async Task CreateAsync_ReturnsOk_WhenUserCreated()
    {
        var userDto = new UserDTO { VATNumber = "123456789" };
        var userDomain = UserMapper.ToDomain(userDto);

        _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(userDto.VATNumber))
                           .ReturnsAsync((User?)null);

        _userRepositoryMock.Setup(repo => repo.AddUserAsync(It.Is<User>(u =>
            u.VATNumber.VATValue == userDomain.VATNumber.VATValue)))
                           .Returns(Task.CompletedTask);

        var result = await _controller.CreateAsync(userDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<UserDTO>(okResult.Value);
        Assert.Equal(userDto.VATNumber, returnedDto.VATNumber);

        _userRepositoryMock.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_WhenDuplicateVat()
    {
        var userDto = new UserDTO { VATNumber = "duplicate" };
        var userDomain = UserMapper.ToDomain(userDto);

        _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(userDto.VATNumber))
                           .ReturnsAsync(userDomain);

        var result = await _controller.CreateAsync(userDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        // Safely extract message string from Value, whether plain string or object with 'message' property
        var errorMessage = badRequestResult.Value as string ??
                           (badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value) as string);

        Assert.Equal("User with this VAT number already exists.", errorMessage);

        _userRepositoryMock.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetByVatNumberAsync_ReturnsOk_WhenUserExists()
    {
        var vatString = "123456789";
        var userDomain = new User { VATNumber = new VATNumber(vatString) };
        var userDto = UserMapper.ToDTO(userDomain);

        _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatString))
                           .ReturnsAsync(userDomain);

        var result = await _controller.GetByVatNumberAsync(vatString);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<UserDTO>(okResult.Value);
        Assert.Equal(vatString, returnedDto.VATNumber);
    }

    [Fact]
    public async Task GetByVatNumberAsync_ReturnsNotFound_WhenUserNotFound()
    {
        var vatString = "notfound";

        _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatString))
                           .ReturnsAsync((User?)null);

        var result = await _controller.GetByVatNumberAsync(vatString);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

        var notFoundMessage = notFoundResult.Value as string ??
                             (notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value) as string);

        Assert.Equal("User not found.", notFoundMessage);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsOk_WhenUserDeleted()
    {
        var vatString = "123456789";
        var userDomain = new User { VATNumber = new VATNumber(vatString), Id = "someId" };

        _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatString))
                           .ReturnsAsync(userDomain);

        _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(userDomain.Id))
                           .Returns(Task.CompletedTask);

        var result = await _controller.DeleteAsync(vatString);

        Assert.IsType<OkResult>(result);

        _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(userDomain.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenUserNotDeleted()
    {
        var vatString = "failvat";

        _userRepositoryMock.Setup(repo => repo.GetUserByVATNumberAsync(vatString))
                           .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.DeleteAsync(vatString));

        _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(It.IsAny<string>()), Times.Never);
    }
}