using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CloudinaryDotNet.Actions;
using GeoRunHub.Controllers;
using GeoRunHub.Interfaces;
using GeoRunHub.Models;
using GeoRunHub.ViewModels;

namespace GeoRunHub.Tests;

public class RaceControllerTests
{
    private readonly Mock<IRaceRepository> _mockRaceRepo;
    private readonly Mock<IPhotoService> _mockPhotoService;
    private readonly RaceController _raceController;

    public RaceControllerTests()
    {
        _mockRaceRepo = new Mock<IRaceRepository>();
        _mockPhotoService = new Mock<IPhotoService>();
        _raceController = new RaceController(_mockRaceRepo.Object, _mockPhotoService.Object);
    }

    [Fact]
    public async Task IndexShowsRaces()
    {
        var sampleRaces = new List<Race>
        {
            new Race { Id = 12, Title = "Race1" },
            new Race { Id = 34, Title = "Race2" }
        };

        _mockRaceRepo.Setup(repo => repo.GetAll()).ReturnsAsync(sampleRaces);

        var actionResult = await _raceController.Index();

        var viewResult = Assert.IsType<ViewResult>(actionResult);
        var model = Assert.IsAssignableFrom<IEnumerable<Race>>(viewResult.Model);
        Assert.Equal(2, ((List<Race>)model).Count);
    }

    [Fact]
    public async Task DetailShowsRace()
    {
        var race = new Race { Id = 56, Title = "Race3" };
        _mockRaceRepo.Setup(repo => repo.GetByIdAsync(56)).ReturnsAsync(race);

        var result = await _raceController.Detail(56);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Race>(viewResult.Model);
        Assert.Equal("Race3", model.Title);
    }

    [Fact]
    public void CreateShowsView()
    {
        var result = _raceController.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task CreatePostAddsRace()
    {
        var raceVM = new CreateRaceViewModel
        {
            Title = "Race4",
            Description = "Description",
            Address = new Address { Street = "street1", City = "city1", State = "state1" },
            Image = new StubFormFile()
        };

        _mockPhotoService.Setup(ps => ps.AddPhotoAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(new ImageUploadResult { Url = new System.Uri("http://fakeimage123.com/img.jpg") });

        var result = await _raceController.Create(raceVM);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        _mockRaceRepo.Verify(r => r.Add(It.Is<Race>(race =>
            race.Title == "Race4" &&
            race.Address.City == "city1")), Times.Once);
    }

    [Fact]
    public async Task CreatePostInvalidModelReturnsView()
    {
        _raceController.ModelState.AddModelError("Title", "Title is required");

        var raceVM = new CreateRaceViewModel
        {
            Address = new Address(),
            Image = new StubFormFile()
        };

        var result = await _raceController.Create(raceVM);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(raceVM, viewResult.Model);
    }
}

public class StubFormFile : IFormFile
{
    public string ContentType => "image/png";
    public string ContentDisposition => "";
    public IHeaderDictionary Headers => null;
    public long Length => 10;
    public string Name => "Race123";
    public string FileName => "race456.png";

    public void CopyTo(System.IO.Stream target) { }

    public Task CopyToAsync(System.IO.Stream target, System.Threading.CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public System.IO.Stream OpenReadStream() => new System.IO.MemoryStream(new byte[] { 1, 2, 3 });
}