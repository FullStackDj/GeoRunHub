using Microsoft.EntityFrameworkCore;
using GeoRunHub.Data;
using GeoRunHub.Interfaces;
using GeoRunHub.Models;

namespace GeoRunHub.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AppUser>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<AppUser> GetUserById(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public bool Add(AppUser user)
    {
        throw new NotImplementedException();
    }

    public bool Update(AppUser user)
    {
        _context.Users.Update(user);
        return Save();
    }

    public bool Delete(AppUser user)
    {
        throw new NotImplementedException();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }
}