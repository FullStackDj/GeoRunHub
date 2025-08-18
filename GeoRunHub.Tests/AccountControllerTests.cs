using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using GeoRunHub.Controllers;
using GeoRunHub.Models;
using GeoRunHub.ViewModels;
using GeoRunHub.Data;

namespace GeoRunHub.Tests;

public class AccountControllerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<SignInManager<AppUser>> _signInManagerMock;
    private readonly Mock<ApplicationDbContext> _dbContextMock;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        var store = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();

        _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            null, null, null, null);

        _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, null);

        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _controller.TempData = tempData;
    }

    [Fact]
    public async Task LoginOk()
    {
        var loginVm = new LoginViewModel { EmailAddress = "test@test.com", Password = "123" };
        var user = new AppUser { Email = "test@test.com", UserName = "test@test.com" };

        _userManagerMock.Setup(u => u.FindByEmailAsync(loginVm.EmailAddress)).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, loginVm.Password)).ReturnsAsync(true);
        _signInManagerMock.Setup(s => s.PasswordSignInAsync(user, loginVm.Password, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var result = await _controller.Login(loginVm);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Race", redirect.ControllerName);
    }

    public async Task LoginWrong()
    {
        var loginVM = new LoginViewModel { EmailAddress = "test@test.com", Password = "wrongpass" };

        _userManagerMock.Setup(u => u.FindByEmailAsync(loginVM.EmailAddress))
            .ReturnsAsync(new AppUser { Email = loginVM.EmailAddress });

        _userManagerMock.Setup(u => u.CheckPasswordAsync(It.IsAny<AppUser>(), loginVM.Password))
            .ReturnsAsync(false);

        var result = await _controller.Login(loginVM);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(loginVM, view.Model);
        Assert.True(_controller.TempData.ContainsKey("Error"));
    }

    [Fact]
    public async Task RegisterExists()
    {
        var registerVM = new RegisterViewModel { EmailAddress = "exists@test.com", Password = "12345" };

        _userManagerMock.Setup(u => u.FindByEmailAsync(registerVM.EmailAddress))
            .ReturnsAsync(new AppUser { Email = registerVM.EmailAddress });

        var result = await _controller.Register(registerVM);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(registerVM, view.Model);
        Assert.True(_controller.TempData.ContainsKey("Error"));
    }

    [Fact]
    public async Task RegisterOk()
    {
        var registerVm = new RegisterViewModel { EmailAddress = "new@test.com", Password = "123" };

        _userManagerMock.Setup(u => u.FindByEmailAsync(registerVm.EmailAddress)).ReturnsAsync((AppUser)null);
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<AppUser>(), registerVm.Password))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _controller.Register(registerVm);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Race", redirect.ControllerName);
    }

    [Fact]
    public async Task LogoutOk()
    {
        var result = await _controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Race", redirect.ControllerName);
    }
}