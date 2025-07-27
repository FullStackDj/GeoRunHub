using Microsoft.AspNetCore.Mvc;
using GeoRunHub.Data;
using GeoRunHub.Models;

namespace GeoRunHub.Controllers;

public class RaceController : Controller
{
    private readonly ApplicationDbContext _context;

    public RaceController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<Race> races = _context.Races.ToList();
        return View(races);
    }
}