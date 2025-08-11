using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GeoRunHub.Data;
using GeoRunHub.Interfaces;
using GeoRunHub.Models;

namespace GeoRunHub.Repository;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<Club>> GetAllUserClubs()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return new List<Club>();
        }

        return await _context.Clubs
            .Where(r => r.AppUser != null && r.AppUser.Id == userId)
            .ToListAsync();
    }

    public async Task<List<Race>> GetAllUserRaces()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return new List<Race>();
        }

        return await _context.Races
            .Where(r => r.AppUser != null && r.AppUser.Id == userId)
            .ToListAsync();
    }
}