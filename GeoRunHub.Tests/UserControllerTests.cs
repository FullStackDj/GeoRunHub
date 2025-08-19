using Microsoft.AspNetCore.Mvc;
using Moq;
using GeoRunHub.Controllers;
using GeoRunHub.Interfaces;
using GeoRunHub.Models;
using GeoRunHub.ViewModels;

namespace GeoRunHub.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserRepository> _repo;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _repo = new Mock<IUserRepository>();
        _controller = new UserController(_repo.Object);
    }

    [Fact]
    public async Task IndexOk()
    {
        var users = new List<AppUser>
        {
            new AppUser { Id = "1", UserName = "Name1", Pace = 5, Mileage = 10, ProfileImageUrl = "img1" },
            new AppUser { Id = "2", UserName = "Name2", Pace = 6, Mileage = 20, ProfileImageUrl = "img2" }
        };

        _repo.Setup(r => r.GetAllUsers()).ReturnsAsync(users);

        var result = await _controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<UserViewModel>>(view.Model);
        Assert.Equal(2, model.Count);
        Assert.Equal("Name1", model[0].UserName);
    }

    [Fact]
    public async Task DetailOk()
    {
        var user = new AppUser { Id = "1", UserName = "Name1", Pace = 5, Mileage = 10 };

        _repo.Setup(r => r.GetUserById("1")).ReturnsAsync(user);

        var result = await _controller.Detail("1");

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<UserDetailViewModel>(view.Model);
        Assert.Equal("Name1", model.UserName);
    }
}