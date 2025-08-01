using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CloudinaryDotNet.Actions;
using GeoRunHub.Controllers;
using GeoRunHub.Interfaces;
using GeoRunHub.Models;
using GeoRunHub.ViewModels;

namespace GeoRunHub.Tests;

public class ClubControllerTests
{
    private readonly Mock<IClubRepository> _clubRepoMock;
    private readonly Mock<IPhotoService> _photoServiceMock;
    private readonly ClubController _controller;

    public ClubControllerTests()
    {
        _clubRepoMock = new Mock<IClubRepository>();
        _photoServiceMock = new Mock<IPhotoService>();
        _controller = new ClubController(_clubRepoMock.Object, _photoServiceMock.Object);
    }

    [Fact]
    public async Task IndexShowsClubs()
    {
        var mockClubs = new List<Club>
        {
            new Club { Id = 1, Title = "Title1" }
        };
        _clubRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(mockClubs);

        var result = await _controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Club>>(view.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task DetailShowsClub()
    {
        var club = new Club { Id = 23, Title = "Title2" };
        _clubRepoMock.Setup(repo => repo.GetByIdAsync(23)).ReturnsAsync(club);

        var result = await _controller.Detail(23);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Club>(view.Model);
        Assert.Equal("Title2", model.Title);
    }

    [Fact]
    public void CreateReturnsView()
    {
        var result = _controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task CreatePostRedirectsToIndex()
    {
        var viewModel = new CreateClubViewModel
        {
            Title = "Club",
            Description = "Description",
            Address = new Address
            {
                Street = "Street",
                City = "City",
                State = "State"
            },
            Image = new FakeFormFile()
        };

        _photoServiceMock.Setup(p => p.AddPhotoAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(new ImageUploadResult
            {
                Url = new Uri("http://image.url/photo.jpg")
            });

        var result = await _controller.Create(viewModel);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task CreatePostInvalidReturnsView()
    {
        _controller.ModelState.AddModelError("Title", "Required");

        var viewModel = new CreateClubViewModel
        {
            Address = new Address(),
            Image = new FakeFormFile()
        };

        var result = await _controller.Create(viewModel);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(viewModel, view.Model);
    }
}

public class FakeFormFile : IFormFile
{
    public string ContentType => "image/jpeg";
    public string ContentDisposition => "";
    public IHeaderDictionary Headers => null;
    public long Length => 0;
    public string Name => "Image";
    public string FileName => "fake123.jpg";

    public void CopyTo(Stream target)
    {
    }

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Stream OpenReadStream() => new MemoryStream();
}