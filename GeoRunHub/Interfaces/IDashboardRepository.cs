using GeoRunHub.Models;

namespace GeoRunHub.Interfaces;

public interface IDashboardRepository
{
    Task<List<Race>> GetAllUserRaces();
    Task<List<Club>> GetAllUserClubs();
}