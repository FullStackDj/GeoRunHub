using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using GeoRunHub.Controllers;
using GeoRunHub.Interfaces;
using GeoRunHub.Models;
using GeoRunHub.ViewModels;

namespace GeoRunHub.Tests;

public class HomeControllerTests
{
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        var loggerMock = new Mock<ILogger<HomeController>>();
        var clubRepoMock = new Mock<IClubRepository>();

        _controller = new HomeController(loggerMock.Object, clubRepoMock.Object);
    }

    [Fact]
    public async Task IndexShowsView()
    {
        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<HomeViewModel>(viewResult.Model);
    }

    [Fact]
    public void PrivacyShowsView()
    {
        var result = _controller.Privacy();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.Model);
    }

    [Fact]
    public void ErrorShowsErrorViewModel()
    {
        var httpContext = new DefaultHttpContext();
        var traceId = "trace-12345";
        httpContext.TraceIdentifier = traceId;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        Activity.Current = null;

        var result = _controller.Error();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);

        Assert.Equal(traceId, model.RequestId);
    }
}