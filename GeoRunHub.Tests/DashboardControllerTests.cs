using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CloudinaryDotNet.Actions;
using GeoRunHub.Controllers;
using GeoRunHub.Interfaces;
using GeoRunHub.Models;
using GeoRunHub.ViewModels;

namespace GeoRunHub.Tests;

public class DashboardControllerTests
{
    private readonly Mock<IDashboardRepository> _repo;
    private readonly Mock<IHttpContextAccessor> _http;
    private readonly Mock<IPhotoService> _photo;
    private readonly DashboardController _controller;

    public DashboardControllerTests()
    {
        _repo = new Mock<IDashboardRepository>();
        _http = new Mock<IHttpContextAccessor>();
        _photo = new Mock<IPhotoService>();

        var context = new DefaultHttpContext();
        context.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "user1") }
            )
        );
        _http.Setup(h => h.HttpContext).Returns(context);

        _controller = new DashboardController(_repo.Object, _http.Object, _photo.Object);
    }


    [Fact]
    public async Task IndexOk()
    {
        _repo.Setup(r => r.GetAllUserRaces()).ReturnsAsync(new List<Race>());
        _repo.Setup(r => r.GetAllUserClubs()).ReturnsAsync(new List<Club>());

        var result = await _controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<DashboardViewModel>(view.Model);
        Assert.NotNull(model.Races);
        Assert.NotNull(model.Clubs);
    }

    [Fact]
    public async Task EditGetOk()
    {
        var user = new AppUser
            { Id = "user1", Pace = 5, Mileage = 10, City = "City1", State = "State1", ProfileImageUrl = "img" };
        _repo.Setup(r => r.GetUserById("user1")).ReturnsAsync(user);

        var result = await _controller.EditUserProfile(1);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<EditUserDashboardViewModel>(view.Model);
        Assert.Equal("City1", model.City);
    }

    [Fact]
    public async Task EditGetNotFound()
    {
        _repo.Setup(r => r.GetUserById("user1")).ReturnsAsync((AppUser)null);

        var result = await _controller.EditUserProfile(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Error", view.ViewName);
    }

    [Fact]
    public async Task EditPostInvalid()
    {
        var vm = new EditUserDashboardViewModel { Id = "user1" };
        _controller.ModelState.AddModelError("err", "bad");

        var result = await _controller.EditUserProfile(vm);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("EditUserProfile", view.ViewName);
    }

    [Fact]
    public async Task EditPostOk()
    {
        var vm = new EditUserDashboardViewModel { Id = "user1", City = "City1" };
        var user = new AppUser { Id = "user1", ProfileImageUrl = "" };
        var photoResult = new ImageUploadResult { Url = new Uri("http://test.com/img") };

        _repo.Setup(r => r.GetByIdNoTracking(vm.Id)).ReturnsAsync(user);
        _photo.Setup(p => p.AddPhotoAsync(vm.Image)).ReturnsAsync(photoResult);

        var result = await _controller.EditUserProfile(vm);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
}